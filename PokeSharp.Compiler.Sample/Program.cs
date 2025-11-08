// See https://aka.ms/new-console-template for more information

using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Model;

var pbsSerializer = new PbsSerializer();

await foreach (var type in pbsSerializer.ReadFromFile<PokemonTypeInfo>(
                   ["D:/dev/PokeSharp/PokeSharp.Compiler.Sample/PBS/types.txt"]))
{
    Console.WriteLine(type);
}