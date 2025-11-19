using MessagePack;
using PokeSharp.Core;

namespace PokeSharp.State;

[AutoServiceShortcut]
[MessagePackObject(true)]
public class GameStats
{
    public int PokerusInfections { get; set; }
}
