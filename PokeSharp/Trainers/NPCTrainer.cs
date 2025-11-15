using PokeSharp.Abstractions;

namespace PokeSharp.Trainers;

public class NPCTrainer(Text name, Name trainerType, int version = 0) : Trainer(name, trainerType)
{
    public int Version { get; set; } = version;

    public List<Name> Items { get; set; } = [];

    public Text? LoseText { get; set; }

    public Text? WinText { get; set; }
}
