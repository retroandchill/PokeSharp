#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

#if !POKESHARP_CORE
// ReSharper disable once CheckNamespace
namespace PokeSharp.UI;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
internal class MenuOptionRegistrationAttribute<T>(int priority = 100) : Attribute
#if !POKESHARP_GENERATOR
    where T : IMenuOption
#endif
{
    public int Priority { get; } = priority;
}

[AttributeUsage(AttributeTargets.Field)]
internal class MenuOptionAttribute(string? name = null) : Attribute
{
    public string? Name { get; } = name;
}
#endif
