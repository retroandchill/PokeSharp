using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;

namespace PokeSharp.Core;

/// <summary>
/// Enumeration used to determine whether to add a new name or simply try to retrieve an existing one.
/// </summary>
public enum FindName : byte
{
    /// <summary>
    /// Simply find an existing name, but do not try to add a new one.
    /// </summary>
    Find,

    /// <summary>
    /// Will add a new name if it does not already exist.
    /// </summary>
    Add,
}

/// <summary>
/// Represents an immutable and compact string-like type with efficient comparison,
/// serialization, and interop capabilities.
/// </summary>
/// <remarks>
/// The <c>Name</c> struct offers efficient equality and comparison operations,
/// as well as interop between <see cref="string"/> and <see cref="ReadOnlySpan{char}"/>.
/// It also provides serialization support for MessagePack and JSON via custom formatters.
/// </remarks>
/// <threadsafety>
/// This type is thread-safe due to its immutable nature.
/// </threadsafety>
[MessagePackFormatter(typeof(NameMessagePackFormatter))]
[JsonConverter(typeof(NameJsonConverter))]
public readonly struct Name : IEquatable<Name>, IEquatable<string>, IEquatable<ReadOnlySpan<char>>, IComparable<Name>
{
    /// <summary>
    /// Represents a "none" or null-like state for <see cref="Name"/> instances.
    /// </summary>
    public const int NoNumber = 0;

    /// <summary>
    /// Gets the comparison index for this <see cref="Name"/>. This is the primary value used for equality comparisons.
    /// </summary>
    public uint ComparisonIndex { get; }

    /// <summary>
    /// Gets the display string index for this <see cref="Name"/>. This is the way in which Name is able to get back
    /// the correct case-sensitive string representation of the name.
    /// </summary>
    public uint DisplayStringIndex { get; }

    /// <summary>
    /// Gets the number of the name. A number that is greater than 0 indicates that the name is a numbered name, which
    /// means calles to <see cref="ToString"/> will return the one less than the number as a suffix.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="ReadOnlySpan{char}"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    /// <param name="findType">
    /// Used to determine if we should add a new name or simply try to retrieve an existing one.
    /// </param>
    public Name(ReadOnlySpan<char> name, FindName findType = FindName.Add)
    {
        (ComparisonIndex, DisplayStringIndex, Number) = INameProvider.Instance.GetOrAddEntry(name, findType);
    }

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="string"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    /// <param name="findType">
    /// Used to determine if we should add a new name or simply try to retrieve an existing one.
    /// </param>
    public Name(string name, FindName findType = FindName.Add)
        : this(name.AsSpan(), findType) { }

    /// <summary>
    /// Gets a predefined, immutable <see cref="Name"/> instance representing a "none" or null-like state.
    /// </summary>
    /// <remarks>
    /// This property serves as a default or neutral value for instances of <see cref="Name"/>.
    /// It is particularly useful in scenarios where a valid, meaningful value is not applicable or has not been assigned.
    /// </remarks>
    /// <returns>
    /// A <see cref="Name"/> instance configured to represent the "none" state.
    /// </returns>
    /// <threadsafety>
    /// This property is thread-safe due to the immutable nature of the <see cref="Name"/> struct.
    /// </threadsafety>
    public static Name None => new();

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Name"/> instance represents a valid state.
    /// </summary>
    /// <remarks>
    /// This property checks the underlying value of the <see cref="Name"/> instance to determine if it represents
    /// a meaningful or valid state. An instance may be invalid if it is in a "none" state or has not been initialized
    /// properly, depending on the platform-specific implementation.
    /// </remarks>
    /// <returns>
    /// A boolean value; <c>true</c> if the <see cref="Name"/> instance is valid, otherwise <c>false</c>.
    /// </returns>
    /// <threadsafety>
    /// This property is thread-safe due to the immutable nature of the <see cref="Name"/> struct.
    /// </threadsafety>
    public bool IsValid => !IsNone;

    /// <summary>
    /// Indicates whether the current <see cref="Name"/> instance represents a "none" or null-like state.
    /// </summary>
    /// <remarks>
    /// This property allows checking if the <see cref="Name"/> instance is equivalent to the predefined "none" state,
    /// providing a reliable mechanism to determine neutrality or absence of a meaningful value.
    /// The evaluation is based on internal comparisons with the static <see cref="Name.None"/> property.
    /// </remarks>
    /// <returns>
    /// <c>true</c> if the current instance represents the "none" state; otherwise, <c>false</c>.
    /// </returns>
    /// <threadsafety>
    /// This property is thread-safe due to the immutable nature of the <see cref="Name"/> struct.
    /// </threadsafety>
    public bool IsNone => this == None;

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="Name"/> values are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Name lhs, Name rhs)
    {
        return lhs.ComparisonIndex == rhs.ComparisonIndex && lhs.Number == rhs.Number;
    }

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> instance to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Name"/> instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Name lhs, Name rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The left-hand side <see cref="Name"/> instance.</param>
    /// <param name="rhs">The right-hand side <see cref="Name"/> instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Name lhs, string? rhs)
    {
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, rhs);
    }

    /// <summary>
    /// Compares whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> instance to compare.</param>
    /// <returns>Returns <c>true</c> if the two instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Name lhs, string? rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Compares a <see cref="Name"/> instance with a <see cref="ReadOnlySpan{char}"/> for equality.
    /// </summary>
    /// <param name="lhs">The <see cref="Name"/> on the left-hand side of the equality operator.</param>
    /// <param name="rhs">The <see cref="ReadOnlySpan{char}"/> on the right-hand side of the equality operator.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="lhs"/> and <paramref name="rhs"/> are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Name lhs, ReadOnlySpan<char> rhs)
    {
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, rhs);
    }

    /// <summary>
    /// Determines inequality between a <see cref="Name"/> and a <see cref="ReadOnlySpan{char}"/>.
    /// </summary>
    /// <param name="lhs">The <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The <see cref="ReadOnlySpan{char}"/> to compare against.</param>
    /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Name lhs, ReadOnlySpan<char> rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Defines the implicit conversion from a <see cref="string"/> to a <see cref="Name"/>.
    /// </summary>
    /// <param name="name">The string to convert to a <see cref="Name"/>.</param>
    /// <returns>A new <see cref="Name"/> instance containing the given string.</returns>
    public static implicit operator Name(string name) => new(name);

    /// <summary>
    /// Converts a <see cref="Name"/> instance to a <see cref="string"/> implicitly.
    /// </summary>
    /// <param name="name">The <see cref="Name"/> instance to convert.</param>
    /// <returns>A <see cref="string"/> representation of the <see cref="Name"/> instance.</returns>
    public static implicit operator string(Name name) => name.ToString();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Name other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(Name other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public int CompareTo(Name other)
    {
        return (int)(ComparisonIndex - other.ComparisonIndex);
    }

    /// <inheritdoc />
    public bool Equals(string? other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public bool Equals(ReadOnlySpan<char> other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return INameProvider.Instance.GetString(ComparisonIndex, DisplayStringIndex, Number);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(ComparisonIndex, Number);
    }
}

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

    /// <summary>
    /// Gets the comparison and display string indices for the given name.
    /// </summary>
    /// <param name="value">The character span to use for the string.</param>
    /// <param name="findType"></param>
    /// <returns>A tuple of the comparison and display index.</returns>
    (uint ComparisonIndex, uint DisplayIndex, int Number) GetOrAddEntry(ReadOnlySpan<char> value, FindName findType);

    /// <summary>
    /// Checks whether the given span is equal to the name at the specified index.
    /// </summary>
    /// <param name="comparisonIndex">The comparison index</param>
    /// <param name="span">The character span to compare to.</param>
    /// <returns>Are the two equal.</returns>
    bool Equals(uint comparisonIndex, ReadOnlySpan<char> span);

    /// <summary>
    /// Gets the display string for the given index.
    /// </summary>
    /// <param name="comparisonIndex"></param>
    /// <param name="displayStringId">The ID of the display string.</param>
    /// <param name="number"></param>
    /// <returns>The string that can be displayed.</returns>
    string GetString(uint comparisonIndex, uint displayStringId, int number);
}

internal class DefaultNameProvider : INameProvider
{
    private readonly NameTable _nameTable = new();

    public (uint ComparisonIndex, uint DisplayIndex, int Number) GetOrAddEntry(
        ReadOnlySpan<char> value,
        FindName findType
    )
    {
        if (value.Length == 0)
            return (0, 0, Name.NoNumber);

        var (internalNumber, newLength) = ParseNumber(value);
        var newSlice = value[..newLength];
        var indices = _nameTable.GetOrAddEntry(newSlice, findType);
        return !indices.IsNone
            ? (indices.ComparisonIndex, indices.DisplayStringIndex, internalNumber)
            : (0, 0, Name.NoNumber);
    }

    public bool Equals(uint comparisonIndex, ReadOnlySpan<char> span)
    {
        return _nameTable.EqualsComparison(comparisonIndex, span);
    }

    public string GetString(uint comparisonIndex, uint displayStringId, int number)
    {
        var displayString = _nameTable.GetDisplayString(displayStringId);
        return number != Name.NoNumber ? $"{displayString}_{number - 1}" : displayString;
    }

    private static (int Number, int Length) ParseNumber(ReadOnlySpan<char> name)
    {
        var digits = 0;
        for (var i = name.Length - 1; i >= 0; i--)
        {
            var character = name[i];
            if (character is < '0' or > '9')
                break;

            digits++;
        }

        var firstDigit = name.Length - digits;
        if (firstDigit == 0)
            return (Name.NoNumber, name.Length);

        const int maxDigits = 10;
        if (
            digits <= 0
            || digits >= name.Length
            || name[firstDigit] != '_'
            || digits > maxDigits
            || digits != 1 && name[firstDigit] == '0'
        )
            return (Name.NoNumber, name.Length);

        return int.TryParse(name.Slice(firstDigit, digits), out var number)
            ? (number, name.Length - (digits + 1))
            : (Name.NoNumber, name.Length);
    }
}

internal readonly record struct NameHashEntry(uint Id, int Hash, string Value);

internal readonly record struct NameIndices(uint ComparisonIndex, uint DisplayStringIndex)
{
    public bool IsNone => ComparisonIndex == 0;

    public static readonly NameIndices None = new(0, 0);
}

internal class NameTable
{
    private const int BucketCount = 1024;
    private const int BucketMask = BucketCount - 1;

    private readonly ConcurrentBag<NameHashEntry>[] _comparisonBuckets = new ConcurrentBag<NameHashEntry>[BucketCount];
    private readonly ConcurrentDictionary<uint, string> _comparisonIdToString = new();

    private readonly ConcurrentBag<NameHashEntry>[] _displayBuckets = new ConcurrentBag<NameHashEntry>[BucketCount];
    private readonly ConcurrentDictionary<uint, string> _displayIdToString = new();

    private uint _nextComparisonId = 1;
    private uint _nextDisplayId = 1;

    public NameTable()
    {
        for (var i = 0; i < BucketCount; i++)
        {
            _comparisonBuckets[i] = [];
            _displayBuckets[i] = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeHashIgnoreCase(ReadOnlySpan<char> span)
    {
        var hash = 0;

        foreach (var t in span)
        {
            var c = char.ToLowerInvariant(t);
            hash = ((hash << 5) + hash) ^ c;
        }
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeHashCaseSensitive(ReadOnlySpan<char> span)
    {
        var hash = 0;

        foreach (var c in span)
        {
            hash = ((hash << 5) + hash) ^ c;
        }
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SpanEqualsString(ReadOnlySpan<char> span, string str, bool caseSensitive = false)
    {
        if (span.Length != str.Length)
            return false;

        for (var i = 0; i < span.Length; i++)
        {
            if (caseSensitive)
            {
                if (span[i] != str[i])
                    return false;

                continue;
            }

            if (char.ToLowerInvariant(span[i]) != char.ToLowerInvariant(str[i]))
                return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NameIndices GetOrAddEntry(ReadOnlySpan<char> value, FindName findType)
    {
        if (IsNoneSpan(value))
            return NameIndices.None;

        var hashIgnore = ComputeHashIgnoreCase(value);
        var hashCase = ComputeHashCaseSensitive(value);

        var cmpBucketIdx = hashIgnore & BucketMask;
        var dispBucketIdx = hashCase & BucketMask;

        var cmpBucket = _comparisonBuckets[cmpBucketIdx];
        var dispBucket = _displayBuckets[dispBucketIdx];

        uint comparisonId = 0;
        uint displayId = 0;

        // ---- Comparison lookup (case-insensitive) ----
        foreach (var entry in cmpBucket)
        {
            if (entry.Hash != hashIgnore)
                continue;

            if (SpanEqualsString(value, entry.Value, caseSensitive: false))
            {
                comparisonId = entry.Id;
                break;
            }
        }

        // ---- Display lookup (case-sensitive) ----
        foreach (var entry in dispBucket)
        {
            if (entry.Hash != hashCase)
                continue;

            if (SpanEqualsString(value, entry.Value, caseSensitive: true))
            {
                displayId = entry.Id;
                break;
            }
        }

        // If both found, we're done.
        if (comparisonId != 0 && displayId != 0)
            return new NameIndices(comparisonId, displayId);

        if (findType == FindName.Find)
        {
            // For strict "find only", require that both indices exist.
            // If you want looser semantics (e.g., comparison exists but
            // display doesn't), this is the place to tweak.
            return new NameIndices(0, 0);
        }

        // ---- Add path ----
        // We only allocate the string once here.
        var str = value.ToString();

        // (1) Ensure comparison entry exists
        if (comparisonId == 0)
        {
            comparisonId = Interlocked.Increment(ref _nextComparisonId);
            var cmpEntry = new NameHashEntry(comparisonId, hashIgnore, str);
            cmpBucket.Add(cmpEntry);
            _comparisonIdToString.TryAdd(comparisonId, str);
        }

        // (2) Ensure display entry exists
        if (displayId != 0)
            return new NameIndices(comparisonId, displayId);

        displayId = Interlocked.Increment(ref _nextDisplayId);
        var dispEntry = new NameHashEntry(displayId, hashCase, str);
        dispBucket.Add(dispEntry);
        _displayIdToString.TryAdd(displayId, str);

        return new NameIndices(comparisonId, displayId);
    }

    /// <summary>
    /// Get display string for a display id (case-preserving).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetDisplayString(uint displayId) => _displayIdToString.GetValueOrDefault(displayId, "None");

    /// <summary>
    /// Get comparison string (canonical) for a comparison id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetComparisonString(uint comparisonId) =>
        _comparisonIdToString.GetValueOrDefault(comparisonId, "None");

    /// <summary>
    /// Compare display id against a span, case-sensitive.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsDisplay(uint displayId, ReadOnlySpan<char> span)
    {
        if (displayId == 0)
            return IsNoneSpan(span);

        return _displayIdToString.TryGetValue(displayId, out var value)
            && SpanEqualsString(span, value, caseSensitive: true);
    }

    /// <summary>
    /// Compare comparison id against a span, case-insensitive.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsComparison(uint comparisonId, ReadOnlySpan<char> span)
    {
        if (comparisonId == 0)
            return IsNoneSpan(span);

        return _comparisonIdToString.TryGetValue(comparisonId, out var value)
            && SpanEqualsString(span, value, caseSensitive: false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNoneSpan(ReadOnlySpan<char> span)
    {
        return span.IsEmpty
            || (
                span.Length == 4
                && (span[0] == 'N' || span[0] == 'n')
                && (span[1] == 'o' || span[1] == 'O')
                && (span[2] == 'n' || span[2] == 'N')
                && (span[3] == 'e' || span[3] == 'E')
            );
    }
}
