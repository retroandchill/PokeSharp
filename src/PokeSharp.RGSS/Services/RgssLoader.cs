using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using PokeSharp.RGSS.RPG;
using PokeSharp.SourceGenerator.Attributes;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.RGSS.Services;

[RegisterSingleton]
[AutoServiceShortcut]
public partial class RgssLoader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private Dictionary<int, MapInfo>? _mapInfos;

    public IReadOnlyDictionary<int, MapInfo> MapInfos
    {
        get
        {
            if (_mapInfos is null)
            {
                LoadMapInfos();
            }

            return _mapInfos;
        }
    }

    [CreateSyncVersion]
    [MemberNotNull(nameof(_mapInfos))]
    public async Task LoadMapInfosAsync(CancellationToken cancellationToken = default)
    {
        await using var streamReader = File.OpenRead("Data/MapInfos.json");
        _mapInfos = await JsonSerializer.DeserializeAsync<Dictionary<int, MapInfo>>(
            streamReader,
            _jsonOptions,
            cancellationToken
        );
    }
}
