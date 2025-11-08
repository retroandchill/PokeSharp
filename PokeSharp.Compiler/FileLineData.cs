namespace PokeSharp.Compiler;

public class FileLineData
{
    public string File { get; set; } = string.Empty;
    
    private string _lineData = string.Empty;
    
    private int _lineNumber = 0;

    private string? _section;

    private string? _key;
    
    private string? _value;

    public void Clear()
    {
        File = string.Empty;
        _lineData = string.Empty;
        _lineNumber = 0;
        _section = null;
        _key = null;
        _value = null;
    }

    public void SetSection(string? section, string? key, string? value)
    {
        _section = section;
        _key = key;
        if (value is not null && value.Length > 200)
        {
            _value = $"{value[..200]}...";
        }
        else
        {
            _value = value ?? string.Empty;
        }
    }

    public void SetLine(string line, int lineNumber)
    {
        _section = null;
        _lineData = line.Length > 200 ? $"{line[..200]}..." : line;
        _lineNumber = lineNumber;
    }

    public string LineReport
    {
        get
        {
            if (_section is null)
                return $"File {File}, line {_lineNumber}\n{_lineData}\n \n";

            return _key is null ? $"File {File}, section {_section}\n{_value}\n \n" : $"File {File}, section {_section}, key {_key}\n{_value}\n \n";
        }
    }
}