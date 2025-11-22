#if POKESHARP_CORE_GENERATOR
using RhoMicro.CodeAnalysis;
#else
using PokeSharp.Core.Data;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Core.Data;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#if POKESHARP_CORE_GENERATOR
[IncludeFile]
#endif
internal class GameDataRegistrationAttribute<T>(int priority = 100) : Attribute
#if !POKESHARP_CORE_GENERATOR
    where T : IRegisteredGameDataEntity
#endif
{
    public int Priority { get; } = priority;
}

[AttributeUsage(AttributeTargets.Field)]
internal class GameDataEntityRegistrationAttribute : Attribute;
