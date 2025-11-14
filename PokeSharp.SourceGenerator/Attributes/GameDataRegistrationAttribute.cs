#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#else
using PokeSharp.Core.Data;
#endif

namespace PokeSharp.SourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
internal class GameDataRegistrationAttribute<T>(int priority = 100) : Attribute
#if !POKESHARP_GENERATOR
    where T : IRegisteredGameDataEntity
#endif
{
    public int Priority { get; } = priority;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class GameDataEntityRegistrationAttribute : Attribute;
