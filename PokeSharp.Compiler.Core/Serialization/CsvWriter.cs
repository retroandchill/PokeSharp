using System.Collections;
using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Utils;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core.Serialization;

public static partial class CsvWriter
{
    [CreateSyncVersion]
    public static async Task WriteCsvRecordAsync(
        object? record,
        StreamWriter writer,
        SchemaEntry schema
    )
    {
        var recordSet = record is IEnumerable asEnumerable and not string
            ? asEnumerable.Flatten().SelectMany(DeconstructIfNecessary).ToImmutableArray()
            : DeconstructIfNecessary(record).ToImmutableArray();

        if (recordSet.IsEmpty)
            return;

        var index = -1;
        var noMoreValues = false;
        while (true)
        {
            foreach (var typeData in schema.TypeEntries)
            {
                index++;
                var value = index < recordSet.Length ? recordSet[index] : null;
                if (typeData.IsOptional)
                {
                    var laterValueFound = false;
                    for (var j = index; j < recordSet.Length; j++)
                    {
                        if (recordSet[j] is null)
                            continue;

                        laterValueFound = true;
                        break;
                    }

                    if (!laterValueFound)
                    {
                        noMoreValues = true;
                        break;
                    }
                }

                if (index > 0)
                    await writer.WriteAsync(',');
                if (value is null)
                    continue;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (typeData.Type)
                {
                    case PbsFieldType.Enumerable:
                        await WriteEnumRecordAsync(value, writer, typeData.EnumType);
                        break;
                    case PbsFieldType.EnumerableOrInteger:
                        await WriteEnumOrIntegerRecordAsync(value, writer, typeData.EnumType);
                        break;
                    default:
                        await WriteOtherRecordTypeAsync(value, writer, typeData.Type);
                        break;
                }
            }

            if ((!noMoreValues && index >= recordSet.Length - 1) || noMoreValues)
                break;
        }
    }

    private static IEnumerable<object?> DeconstructIfNecessary(object? record)
    {
        if (record is null)
            return [null];

        var recordType = record.GetType();
        if (TypeUtils.IsSimpleType(recordType))
            return [record];

        return recordType.GetProperties().Select(p => p.GetValue(record));
    }

    [CreateSyncVersion]
    private static async Task WriteEnumRecordAsync(
        object? record,
        StreamWriter writer,
        Type? enumType
    )
    {
        // TODO: This needs some more logic, but for now we're just going to write the value.
        await writer.WriteAsync(record?.ToString() ?? "");
    }

    [CreateSyncVersion]
    private static async Task WriteEnumOrIntegerRecordAsync(
        object? record,
        StreamWriter writer,
        Type? enumType
    )
    {
        if (enumType is not null && enumType.IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(enumType);
            record = Convert.ChangeType(record, underlyingType);
        }

        await writer.WriteAsync(record?.ToString() ?? "");
    }

    [CreateSyncVersion]
    private static async Task WriteOtherRecordTypeAsync(
        object? record,
        StreamWriter writer,
        PbsFieldType schema
    )
    {
        switch (record)
        {
            case string str:
                await writer.WriteAsync(
                    (schema == PbsFieldType.UnformattedText) ? str : TextFormatter.CsvQuote(str)
                );
                break;
            case Name name:
                await writer.WriteAsync(name.ToString());
                break;
            case true:
                await writer.WriteAsync("true");
                break;
            case false:
                await writer.WriteAsync("false");
                break;
            default:
                await writer.WriteAsync(record?.ToString() ?? "");
                break;
        }
    }
}
