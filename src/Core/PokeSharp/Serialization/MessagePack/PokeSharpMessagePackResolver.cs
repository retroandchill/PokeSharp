using Injectio.Attributes;
using MessagePack;

namespace PokeSharp.Serialization.MessagePack;

[RegisterSingleton<IFormatterResolver>(Duplicate = DuplicateStrategy.Append, Factory = nameof(Instance))]
[GeneratedMessagePackResolver]
public partial class PokeSharpMessagePackResolver;
