using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;

namespace PokeSharp.Core.Strings;

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

public interface IName<T> : IEquatable<T>, IComparable<T>, IEqualityOperators<T, T, bool>
    where T : struct, IName<T>
{
    uint ComparisonIndex { get; }

    uint DisplayStringIndex { get; }

    int Number { get; }

    static abstract T FromString(string name, FindName findType = FindName.Add);

    static abstract T FromString(ReadOnlySpan<char> name, FindName findType = FindName.Add);

    static abstract T None { get; }

    bool IsValid { get; }

    bool IsNone { get; }
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
[JsonConverter(typeof(NameJsonConverter<Name>))]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Name
    : IName<Name>,
        IEquatable<string>,
        IEquatable<ReadOnlySpan<char>>,
        IEquatable<CaselessName>,
        IComparable<CaselessName>,
        IEqualityOperators<Name, string?, bool>,
        IEqualityOperators<Name, CaselessName, bool>
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
    /// Gets the number of the name. A number that is greater than 0 indicates that the name is a numbered name, which
    /// means calles to <see cref="ToString"/> will return the one less than the number as a suffix.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the display string index for this <see cref="Name"/>. This is the way in which Name is able to get back
    /// the correct case-sensitive string representation of the name.
    /// </summary>
    public uint DisplayStringIndex { get; }

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

    internal Name(uint comparisonIndex, uint displayStringIndex, int number)
    {
        ComparisonIndex = comparisonIndex;
        DisplayStringIndex = displayStringIndex;
        Number = number;
    }

    public static Name FromString(string name, FindName findType = FindName.Add) => new(name, findType);

    public static Name FromString(ReadOnlySpan<char> name, FindName findType = FindName.Add) => new(name, findType);

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
    public bool IsValid => INameProvider.Instance.IsValid(ComparisonIndex, DisplayStringIndex);

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
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, lhs.DisplayStringIndex, lhs.Number, rhs);
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
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, lhs.DisplayStringIndex, lhs.Number, rhs);
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

    public static bool operator ==(Name left, CaselessName right)
    {
        return left.ComparisonIndex == right.ComparisonIndex && left.Number == right.Number;
    }

    public static bool operator !=(Name left, CaselessName right)
    {
        return !(left == right);
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

    public static implicit operator CaselessName(Name name) => new(name.ComparisonIndex, name.Number);

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
    public bool Equals(string? other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public bool Equals(ReadOnlySpan<char> other)
    {
        return this == other;
    }

    public bool Equals(CaselessName other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public int CompareTo(Name other)
    {
        return (int)(ComparisonIndex - other.ComparisonIndex);
    }

    public int CompareTo(CaselessName other)
    {
        return (int)(ComparisonIndex - other.ComparisonIndex);
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
