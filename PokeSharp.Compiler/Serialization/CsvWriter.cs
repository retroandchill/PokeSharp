using System.Collections;
using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Schema;

namespace PokeSharp.Compiler.Serialization;

public static class CsvWriter
{
    public static async Task<object?> WriteCsvRecord(object? record, StreamWriter writer, SchemaEntry schema)
    {
        var recordSet = record is IEnumerable asEnumerable ? asEnumerable.Flatten().ToImmutableArray() : [record];
        var start = schema.TypeString[0] is '*' or '^' ? 1 : 0;
        var index = -1;
        while (true)
        {
            for (var i = start; i < schema.TypeString.Length; i++)
            {
                index++;
                var value = recordSet[index];
                if (char.IsUpper(schema.TypeString[i]))
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
                        start = -1;
                        break;
                    }
                }

                if (index > 0) await writer.WriteAsync(',');
                if (value is null) continue;

                switch (char.ToLower(schema.TypeString[i]))
                {
                    case SchemaValues.Enum:
                        await WriteEnumRecord(value, writer, schema.EnumTypes[i - start]);
                        break;
                    case SchemaValues.EnumOrInteger:
                        await WriteEnumOrIntegerRecord(value, writer, schema.EnumTypes[i - start]);
                        break;
                    default:
                        await WriteOtherRecordType(value, writer, schema.TypeString[i]);
                        break;
                }
            }

            if (start > 0 && index >= recordSet.Length - 1) break;
            if (start <= 0) break;
        }

        return record;
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

    private static async Task WriteOtherRecordType(object? record, StreamWriter writer, char schema)
    {
        switch (record)
        {
            case string str:
                await writer.WriteAsync((schema == SchemaValues.Unformatted) ? str : TextFormatter.CsvQuote(str));
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