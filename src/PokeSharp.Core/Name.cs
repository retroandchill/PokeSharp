using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;
#if UNREAL_ENGINE
using UnrealSharp.Core;
#else
using System.Collections.Concurrent;
#endif

namespace PokeSharp.Core;

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
#if UNREAL_ENGINE
    private readonly FName _value;
#else
    private readonly uint _comparisonIndex;
    private readonly uint _displayStringIndex;
#endif
#if UNREAL_ENGINE
    /// <summary>
    /// Initializes a new instance of the <see cref="Name"/> struct from an Unreal Engine <see cref="FName"/>.
    /// </summary>
    public Name(FName name)
    {
        _value = name;
    }
#endif

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="ReadOnlySpan{char}"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    public Name(ReadOnlySpan<char> name)
    {
#if UNREAL_ENGINE
        _value = new FName(name);
#else
        _comparisonIndex = NameTable.Instance.GetOrAddEntry(name);
        _displayStringIndex = NameTable.DisplayStringInstance.GetOrAddEntry(name);
#endif
    }

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="string"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    public Name(string name)
        : this(name.AsSpan()) { }

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
    public static Name None
    {
        get
        {
#if UNREAL_ENGINE
            return new Name(FName.None);
#else
            return new Name();
#endif
        }
    }

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
    public bool IsValid
    {
        get
        {
#if UNREAL_ENGINE
            return _value.IsValid;
#else
            return !IsNone;
#endif
        }
    }

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
    public bool IsNone
    {
        get
        {
#if UNREAL_ENGINE
            return _value.IsNone;
#else
            return this == None;
#endif
        }
    }

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
#if UNREAL_ENGINE
        return lhs._value == rhs._value;
#else
        return lhs._comparisonIndex == rhs._comparisonIndex;
#endif
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
#if UNREAL_ENGINE
        // TODO: Add additional interop to UnrealSharp to make this comparison directly using StringViews
        return lhs._value == (rhs ?? string.Empty);
#else
        return NameTable.Instance.Equals(lhs._comparisonIndex, rhs);
#endif
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
#if UNREAL_ENGINE
        // TODO: Add additional interop to UnrealSharp to make this comparison directly using StringViews
        return lhs._value == rhs.ToString();
#else
        return NameTable.Instance.Equals(lhs._comparisonIndex, rhs);
#endif
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

#if UNREAL_ENGINE
    /// <summary>
    /// Defines the implicit conversion from an <see cref="FName"/> to a <see cref="Name"/>.
    /// </summary>
    /// <param name="name">The <see cref="FName"/> to convert to a <see cref="Name"/>.</param>
    /// <returns>A new <see cref="Name"/> instance containing the given <see cref="FName"/>.</returns>
    public static implicit operator Name(FName name) => new(name);

    /// <summary>
    /// Converts a <see cref="Name"/> instance to an <see cref="FName"/> implicitly.
    /// </summary>
    /// <param name="name">The <see cref="Name"/> instance to convert.</param>
    /// <returns>An <see cref="FName"/> representation of the <see cref="Name"/> instance.</returns>
    public static implicit operator FName(Name name) => name._value;
#endif

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Name other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(Name other)
    {
#if UNREAL_ENGINE
        return _value == other._value;
#else
        return _comparisonIndex == other._comparisonIndex;
#endif
    }

    /// <inheritdoc />
    public int CompareTo(Name other)
    {
#if UNREAL_ENGINE
        return _value.CompareTo(other._value);
#else
        return (int)(_comparisonIndex - other._comparisonIndex);
#endif
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
#if UNREAL_ENGINE
        return _value.ToString();
#else
        return NameTable.DisplayStringInstance.GetString(_displayStringIndex);
#endif
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
#if UNREAL_ENGINE
        return _value.GetHashCode();
#else
        return (int)_comparisonIndex;
#endif
    }
}

#if !UNREAL_ENGINE
internal readonly record struct NameHashEntry(uint Id, int Hash, string Value);

internal class NameTable
{
    private const int BucketCount = 1024;
    private const int BucketMask = BucketCount - 1;

    private readonly bool _caseSensitive;
    private readonly ConcurrentBag<NameHashEntry>[] _hashBuckets = new ConcurrentBag<NameHashEntry>[BucketCount];
    private readonly ConcurrentDictionary<uint, string> _idToString = new();

    private uint _nextId = 1;

    public static NameTable Instance { get; } = new();
    public static NameTable DisplayStringInstance { get; } = new(true);

    private NameTable(bool caseSensitive = false)
    {
        _caseSensitive = caseSensitive;
        for (var i = 0; i < BucketCount; i++)
        {
            _hashBuckets[i] = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeHash(ReadOnlySpan<char> span)
    {
        var hash = 0;

        foreach (var t in span)
        {
            var c = _caseSensitive ? t : char.ToLowerInvariant(t);
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
    public uint GetOrAddEntry(ReadOnlySpan<char> value)
    {
        if (IsNoneSpan(value))
            return 0;

        var hash = ComputeHash(value);
        var bucketIndex = hash & BucketMask; // Fast modulo for power of 2
        var bucket = _hashBuckets[bucketIndex];

        // Search existing entries in this bucket
        foreach (var entry in bucket)
        {
            if (entry.Hash == hash && SpanEqualsString(value, entry.Value))
            {
                return entry.Id;
            }
        }

        // Not found, need to add new entry
        // Only now do we allocate a string
        var stringValue = value.ToString();
        var newId = Interlocked.Increment(ref _nextId);
        var newEntry = new NameHashEntry(newId, hash, stringValue);

        // Add to both bucket and reverse lookup
        bucket.Add(newEntry);
        _idToString.TryAdd(newId, stringValue);

        return newId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString(uint id)
    {
        return _idToString.GetValueOrDefault(id, "None");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(uint id, ReadOnlySpan<char> span)
    {
        if (id == 0)
        {
            return IsNoneSpan(span);
        }

        return _idToString.TryGetValue(id, out var value) && SpanEqualsString(span, value);
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
#endif
