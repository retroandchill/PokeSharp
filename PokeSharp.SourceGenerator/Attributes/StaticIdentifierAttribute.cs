#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif


namespace PokeSharp.SourceGenerator.Attributes;

/// <summary>
/// Marks a class with a static identifier that can be used for categorization and code generation.
/// </summary>
/// <typeparam name="T">
/// Specifies the structure type associated with the class.
/// The type parameter must be a struct and may have additional type constraints depending on the compilation context.
/// </typeparam>
/// <param name="keys">
/// A collection of strings representing the static keys used for identification.
/// </param>
/// <remarks>
/// This attribute is intended for internal use in facilitating source generation scenarios.
/// Classes marked with this attribute are processed to generate code for static identifiers.
/// The <typeparamref name="T"/> constraint ensures type safety during compilation.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
public class StaticIdentifierAttribute<T>(params string[] keys) : Attribute
{
    /// <summary>
    /// Gets the collection of strings representing static keys used for identification.
    /// </summary>
    /// <remarks>
    /// These keys are defined during the instantiation of the <see cref="StaticIdentifierAttribute{T}"/> and are utilized
    /// for categorization and facilitating source generation processes.
    /// </remarks>
    public string[] Values { get; } = keys;
}