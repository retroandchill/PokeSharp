using MessagePack;

namespace PokeSharp.Core.Serialization.MessagePack;

[RegisterSingleton<IFormatterResolver>(Duplicate = DuplicateStrategy.Append, Factory = nameof(Instance))]
[GeneratedMessagePackResolver]
public partial class PokeSharpCoreMessagePackFormatterResolver;
