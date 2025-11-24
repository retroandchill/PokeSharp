namespace PokeSharp.Core.Strings;

/// <summary>
/// Interface for getting the comparison and display string indices for a given name.
/// By default, this uses a C#-provided hash table to store the indices, but it can be changed to
/// use a different implementation.
/// </summary>
public interface INameProvider
{
    /// <summary>
    /// Gets the current instance of the name provider.
    /// </summary>
    static INameProvider Instance { get; internal set; } = new DefaultNameProvider();

    /// <summary>
    /// Sets the current instance of the name provider to the specified provider.
    /// </summary>
    /// <param name="provider">The new provider to use</param>
    /// <remarks>
    /// This method should not be called after the application has started processing as it may
    /// break existing name instances.
    /// </remarks>
    static void UseCustomProvider(INameProvider provider) => Instance = provider;

    static void ResetNameProvider() => Instance = new DefaultNameProvider();

    /// <summary>
    /// Gets the comparison and display string indices for the given name.
    /// </summary>
    /// <param name="value">The character span to use for the string.</param>
    /// <param name="findType"></param>
    /// <returns>A tuple of the comparison and display index.</returns>
    (uint ComparisonIndex, uint DisplayIndex, int Number) GetOrAddEntry(ReadOnlySpan<char> value, FindName findType);

    bool IsValid(uint comparisonIndex, uint displayIndex);

    /// <summary>
    /// Checks whether the given span is equal to the name at the specified index.
    /// </summary>
    /// <param name="comparisonIndex">The comparison index</param>
    /// <param name="displayIndex"></param>
    /// <param name="number"></param>
    /// <param name="span">The character span to compare to.</param>
    /// <returns>Are the two equal.</returns>
    bool Equals(uint comparisonIndex, uint displayIndex, int number, ReadOnlySpan<char> span);

    /// <summary>
    /// Gets the display string for the given index.
    /// </summary>
    /// <param name="comparisonIndex"></param>
    /// <param name="displayStringId">The ID of the display string.</param>
    /// <param name="number"></param>
    /// <returns>The string that can be displayed.</returns>
    string GetString(uint comparisonIndex, uint displayStringId, int number);
}
