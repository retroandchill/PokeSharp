namespace PokeSharp.Compiler.Core.Serialization;

public record FileLineData(string File)
{
    private string LineData { get; init; } = string.Empty;

    private int LineNumber { get; init; }

    private string? Section { get; init; }

    private string? Key { get; init; }

    private string? Value { get; init; }

    public FileLineData WithSection(string? section, string? key, string? value)
    {
        return this with
        {
            Section = section,
            Key = key,
            Value = value is not null && value.Length > 200 ? $"{value[..200]}..." : value ?? string.Empty,
        };
    }

    public FileLineData WithLine(string line, int lineNumber)
    {
        return this with
        {
            Section = null,
            LineData = line.Length > 200 ? $"{line[..200]}..." : line,
            LineNumber = lineNumber,
        };
    }

    public string LineReport
    {
        get
        {
            if (Section is null)
                return $"File {File}, line {LineNumber}\n{LineData}\n \n";

            return Key is null
                ? $"File {File}, section {Section}\n{Value}\n \n"
                : $"File {File}, section {Section}, key {Key}\n{Value}\n \n";
        }
    }
}
