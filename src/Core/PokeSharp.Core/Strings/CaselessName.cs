using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;

namespace PokeSharp.Core.Strings;

[MessagePackFormatter(typeof(CaselessNameMessagePackFormatter))]
[JsonConverter(typeof(NameJsonConverter<CaselessName>))]
[StructLayout(LayoutKind.Sequential)]
public struct CaselessName
    : IName<CaselessName>,
        IEquatable<string>,
        IEquatable<ReadOnlySpan<char>>,
        IEquatable<Name>,
        IComparable<Name>,
        IEqualityOperators<CaselessName, string?, bool>,
        IEqualityOperators<CaselessName, Name, bool>
{
    public uint ComparisonIndex { get; }
    public uint DisplayStringIndex => ComparisonIndex;
    public int Number { get; }

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="ReadOnlySpan{char}"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    /// <param name="findType">
    /// Used to determine if we should add a new name or simply try to retrieve an existing one.
    /// </param>
    public CaselessName(ReadOnlySpan<char> name, FindName findType = FindName.Add)
    {
        (ComparisonIndex, _, Number) = INameProvider.Instance.GetOrAddEntry(name, findType);
    }

    /// <summary>
    /// Construct a new <see cref="Name"/> from a <see cref="string"/>
    /// </summary>
    /// <param name="name">The characters to construct from.</param>
    /// <param name="findType">
    /// Used to determine if we should add a new name or simply try to retrieve an existing one.
    /// </param>
    public CaselessName(string name, FindName findType = FindName.Add)
        : this(name.AsSpan(), findType) { }

    internal CaselessName(uint comparisonIndex, int number)
    {
        ComparisonIndex = comparisonIndex;
        Number = number;
    }

    public static CaselessName FromString(string name, FindName findType = FindName.Add) => new(name, findType);

    public static CaselessName FromString(ReadOnlySpan<char> name, FindName findType = FindName.Add) =>
        new(name, findType);

    public static CaselessName None => new();

    public bool IsValid => INameProvider.Instance.IsValid(ComparisonIndex, DisplayStringIndex);
    public bool IsNone => this == None;

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="Name"/> values are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(CaselessName lhs, CaselessName rhs)
    {
        return lhs.ComparisonIndex == rhs.ComparisonIndex && lhs.Number == rhs.Number;
    }

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> instance to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Name"/> instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CaselessName lhs, CaselessName rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Determines whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The left-hand side <see cref="Name"/> instance.</param>
    /// <param name="rhs">The right-hand side <see cref="Name"/> instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CaselessName lhs, string? rhs)
    {
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, lhs.DisplayStringIndex, lhs.Number, rhs);
    }

    /// <summary>
    /// Compares whether two <see cref="Name"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Name"/> instance to compare.</param>
    /// <returns>Returns <c>true</c> if the two instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CaselessName lhs, string? rhs)
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
    public static bool operator ==(CaselessName lhs, ReadOnlySpan<char> rhs)
    {
        return INameProvider.Instance.Equals(lhs.ComparisonIndex, lhs.DisplayStringIndex, lhs.Number, rhs);
    }

    /// <summary>
    /// Determines inequality between a <see cref="Name"/> and a <see cref="ReadOnlySpan{char}"/>.
    /// </summary>
    /// <param name="lhs">The <see cref="Name"/> instance to compare.</param>
    /// <param name="rhs">The <see cref="ReadOnlySpan{char}"/> to compare against.</param>
    /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CaselessName lhs, ReadOnlySpan<char> rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(CaselessName left, Name right)
    {
        return left.ComparisonIndex == right.ComparisonIndex && left.Number == right.Number;
    }

    public static bool operator !=(CaselessName left, Name right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Defines the implicit conversion from a <see cref="string"/> to a <see cref="Name"/>.
    /// </summary>
    /// <param name="name">The string to convert to a <see cref="Name"/>.</param>
    /// <returns>A new <see cref="Name"/> instance containing the given string.</returns>
    public static implicit operator CaselessName(string name) => new(name);

    /// <summary>
    /// Converts a <see cref="Name"/> instance to a <see cref="string"/> implicitly.
    /// </summary>
    /// <param name="name">The <see cref="Name"/> instance to convert.</param>
    /// <returns>A <see cref="string"/> representation of the <see cref="Name"/> instance.</returns>
    public static implicit operator string(CaselessName name) => name.ToString();

    public static implicit operator Name(CaselessName name) =>
        new(name.ComparisonIndex, name.DisplayStringIndex, name.Number);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Name other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(CaselessName other)
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

    public bool Equals(Name other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public int CompareTo(CaselessName other)
    {
        return (int)(ComparisonIndex - other.ComparisonIndex);
    }

    public int CompareTo(Name other)
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
