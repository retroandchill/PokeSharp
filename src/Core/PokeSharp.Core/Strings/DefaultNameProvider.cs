namespace PokeSharp.Core.Strings;

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

    public bool IsValid(uint comparisonIndex, uint displayIndex)
    {
        return _nameTable.IsValid(comparisonIndex, displayIndex);
    }

    public bool Equals(uint comparisonIndex, uint displayIndex, int number, ReadOnlySpan<char> span)
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
