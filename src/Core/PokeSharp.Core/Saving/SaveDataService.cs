using System.Collections.Immutable;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Core.Strings;
using PokeSharp.Core.Versioning;
using Semver;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Saving;

using ConversionDict = Dictionary<ConversionTriggerType, Dictionary<SemVersion, ImmutableArray<ISaveDataConversion>>>;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed partial class SaveDataService(
    ILogger<SaveDataService> logger,
    IOptionsMonitor<SaveDataConfig> saveDataConfig,
    ISaveSystem saveSystem,
    VersioningService versioningService,
    IEnumerable<ISaveDataValue> saveDataValues,
    IEnumerable<ISaveDataConversion> conversions,
    MessagePackSerializerOptions messagePackSerializerOptions
)
{
    public static readonly Name PokeSharpVersion = "PokeSharpVersion";
    public static readonly Name GameVersion = "GameVersion";

    private readonly ImmutableArray<ISaveDataValue> _values = [.. saveDataValues];
    private readonly ConversionDict _conversions = conversions
        .GroupBy(x => x.TriggerType)
        .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Version).ToDictionary(y => y.Key, y => y.ToImmutableArray()));

    public string FilePath => saveDataConfig.CurrentValue.SaveFileName;

    public bool Exists => saveSystem.Exists(FilePath);

    [CreateSyncVersion]
    public async ValueTask<Dictionary<Name, object>> GetDataFromFileAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        await using var saveDataStream = saveSystem.OpenRead(filePath);
        return await MessagePackSerializer.DeserializeAsync<Dictionary<Name, object>>(
            saveDataStream.Stream,
            messagePackSerializerOptions,
            cancellationToken
        );
    }

    [CreateSyncVersion]
    public async ValueTask<Dictionary<Name, object>> ReadDataFromFileAsync(
        string filepath,
        CancellationToken cancellationToken = default
    )
    {
        var saveData = await GetDataFromFileAsync(filepath, cancellationToken);

        if (saveData.Count <= 0 || !await RunConversionsAsync(saveData, cancellationToken))
            return saveData;

        await using var saveDataStream = await saveSystem.OpenWriteAsync(filepath, cancellationToken);
        await MessagePackSerializer.SerializeAsync(
            saveDataStream.Stream,
            saveData,
            messagePackSerializerOptions,
            cancellationToken
        );
        await saveDataStream.CommitAsync(cancellationToken);
        return saveData;
    }

    [CreateSyncVersion]
    public async ValueTask SaveToFileAsync(string filepath, CancellationToken cancellationToken = default)
    {
        var saveData = CompileSaveDictionary();
        await using var saveDataStream = await saveSystem.OpenWriteAsync(filepath, cancellationToken);
        await MessagePackSerializer.SerializeAsync(
            saveDataStream.Stream,
            saveData,
            messagePackSerializerOptions,
            cancellationToken
        );
        await saveDataStream.CommitAsync(cancellationToken);
    }

    [CreateSyncVersion]
    public async ValueTask DeleteFileAsync(CancellationToken cancellationToken = default)
    {
        await saveSystem.DeleteAsync(FilePath, cancellationToken);
    }

    public bool IsValid(Dictionary<Name, object> saveData)
    {
        return _values.All(x => saveData.TryGetValue(x.Id, out var value) && x.IsValid(value));
    }

    public void LoadValues(Dictionary<Name, object> saveData, Func<ISaveDataValue, bool>? predicate = null)
    {
        foreach (var value in _values.Where(value => predicate is null || !predicate(value)))
        {
            if (saveData.TryGetValue(value.Id, out var savedValue))
            {
                value.Load(savedValue);
            }
            else if (value.HasNewGameValue)
            {
                value.LoadNewGameValue();
            }
        }
    }

    public void LoadAllValues(Dictionary<Name, object> saveData)
    {
        LoadValues(saveData, value => !value.Loaded);
    }

    public void MarkValuesAsUnloaded()
    {
        foreach (var value in _values.Where(value => !value.LoadInBootup || value.ResetOnNewGame))
        {
            value.MarkAsUnloaded();
        }
    }

    public void LoadBootupValues(Dictionary<Name, object> saveData)
    {
        LoadValues(saveData, value => value is { Loaded: false, LoadInBootup: true });
    }

    public void InitializeBootupValues()
    {
        foreach (var value in _values.Where(x => x is { LoadInBootup: true, HasNewGameValue: true, Loaded: false }))
        {
            value.LoadNewGameValue();
        }
    }

    public void LoadNewGameValues()
    {
        foreach (var value in _values.Where(x => x.HasNewGameValue && (!x.Loaded || x.ResetOnNewGame)))
        {
            value.LoadNewGameValue();
        }
    }

    public Dictionary<Name, object> CompileSaveDictionary()
    {
        return _values.ToDictionary(value => value.Id, value => value.Save());
    }

    public IEnumerable<ISaveDataConversion> GetConversions(Dictionary<Name, object> saveData)
    {
        var versions = new Dictionary<ConversionTriggerType, SemVersion>
        {
            [ConversionTriggerType.Framework] = (SemVersion)saveData[PokeSharpVersion],
            [ConversionTriggerType.Game] = (SemVersion)saveData[GameVersion],
        };

        foreach (var (triggerType, _) in versions)
        {
            if (!_conversions.TryGetValue(triggerType, out var data))
                continue;

            foreach (var (_, conversions) in data.OrderBy(x => x.Key, SemVersion.PrecedenceComparer))
            {
                foreach (var conversion in conversions.Where(conversion => conversion.ShouldRun(versions[triggerType])))
                {
                    yield return conversion;
                }
            }
        }
    }

    [CreateSyncVersion]
    public async ValueTask<bool> RunConversionsAsync(
        Dictionary<Name, object> saveData,
        CancellationToken cancellationToken = default
    )
    {
        var conversionsToRun = GetConversions(saveData).ToImmutableArray();
        if (conversionsToRun.Length == 0)
            return false;

        await saveSystem.CopyAsync(FilePath, $"{FilePath}.bak", cancellationToken);
        logger.LogInformation("Converting save file");

        foreach (var conversion in conversionsToRun)
        {
            logger.LogConversionTitle(conversion.Title);
            conversion.Run(saveData);
        }

        logger.LogConversionsRun(conversionsToRun.Length);
        saveData[PokeSharpVersion] = versioningService.FrameworkVersion;
        saveData[GameVersion] = versioningService.GameVersion;
        return true;
    }

    public void RunSingleConversions(object value, Name key, Dictionary<Name, object> saveData)
    {
        foreach (var conversion in GetConversions(saveData))
        {
            conversion.RunSingle(value, key);
        }
    }
}

internal static partial class SaveDataServiceExtensions
{
    [LoggerMessage(LogLevel.Information, "{Title}...")]
    public static partial void LogConversionTitle(this ILogger logger, string title);

    [LoggerMessage(LogLevel.Information, "Successfully applied {ConversionCount} save file conversion(s)")]
    public static partial void LogConversionsRun(this ILogger logger, int conversionCount);
}
