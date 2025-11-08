// See https://aka.ms/new-console-template for more information

using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;

var sampleSchema = new Dictionary<string, SchemaEntry>
{
    ["SectionName"] = new("Id", "s"),
    ["Name"] = new("Name", "s"),
    ["IconPosition"] = new("IconPosition", "u"),
    ["IsSpecialType"] = new("IconPosition", "b"),
    ["IsPseudoType"] = new("IconPosition", "b"),
    ["Weaknesses"] = new("IconPosition", "*m"),
    ["Resistances"] = new("IconPosition", "*m"),
    ["Immunities"] = new("IconPosition", "*m"),
    ["Flags"] = new("IconPosition", "*s")
};

var pbsSerializer = new PbsSerializer();
using var fileReader = new StreamReader("D:/dev/PokeSharp/PokeSharp.Compiler.Sample/PBS/types.txt");

await foreach (var (contents, sectionName) in pbsSerializer.ParseFileSections(fileReader, sampleSchema))
{
    Console.WriteLine(sectionName);
    foreach (var (key, value) in contents)
    {
        Console.WriteLine($"{key}: {value}");
    }
    Console.WriteLine();
}