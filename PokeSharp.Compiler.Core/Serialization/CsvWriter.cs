using System.Collections;
using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;

namespace PokeSharp.Compiler.Core.Serialization;

public static class CsvWriter
{
    public static async Task WriteCsvRecord(object? record, StreamWriter writer, SchemaEntry schema)
    {
        var recordSet = record is IEnumerable asEnumerable ? asEnumerable.Flatten().ToImmutableArray() : [record];
        
        if (recordSet.IsEmpty) return;
        
        var index = -1;
        var noMoreValues = false;
        while (true)
        {
            foreach (var typeData in schema.TypeEntries)
            {
                index++;
                var value = recordSet[index];
                if (typeData.IsOptional)
                {
                    var laterValueFound = false;
                    for (var j = index; j < recordSet.Length; j++)
                    {
                        if (recordSet[j] is null) continue;
                        
                        laterValueFound = true;
                        break;
                    }

                    if (!laterValueFound)
                    {
                        noMoreValues = true;
                        break;
                    }
                }

                if (index > 0) await writer.WriteAsync(',');
                if (value is null) continue;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (typeData.Type)
                {
                    case PbsFieldType.Enumerable:
                        await WriteEnumRecord(value, writer, typeData.EnumType);
                        break;
                    case PbsFieldType.EnumerableOrInteger:
                        await WriteEnumOrIntegerRecord(value, writer, typeData.EnumType);
                        break;
                    default:
                        await WriteOtherRecordType(value, writer, typeData.Type);
                        break;
                }
            }
            
            if ((!noMoreValues && index >= recordSet.Length - 1) || noMoreValues) break;
        }
    }

    private static async Task WriteEnumRecord(object? record, StreamWriter writer, Type? enumType)
    {
        // TODO: This needs some more logic, but for now we're just going to write the value.
        await writer.WriteAsync(record?.ToString() ?? "");
    }
    
    private static async Task WriteEnumOrIntegerRecord(object? record, StreamWriter writer, Type? enumType)
    {
        // TODO: This needs some more logic, but for now we're just going to write the value.
        await writer.WriteAsync(record?.ToString() ?? "");
    }

    private static async Task WriteOtherRecordType(object? record, StreamWriter writer, PbsFieldType schema)
    {
        switch (record)
        {
            case string str:
                await writer.WriteAsync((schema == PbsFieldType.UnformattedText) ? str : TextFormatter.CsvQuote(str));
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