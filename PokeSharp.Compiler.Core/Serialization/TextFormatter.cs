using System.Text.RegularExpressions;

namespace PokeSharp.Compiler.Core.Serialization;

public static partial class TextFormatter
{
    [GeneratedRegex(@"\s*\#.*$")]
    private static partial Regex CommentRegex { get; }
    
    
    [GeneratedRegex(@"[,\""]")]
    private static partial Regex CommaOrQuoteCharacter { get; }
    
    public static int? FindIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        int? index = null;
        var count = 0;
        foreach (var i in collection)
        {
            if (predicate(i))
            {
                index = count;
                break;
            }

            count++;
        }
        
        return index;
    }

    public static string PrepLine(string line)
    {
        var removeComments = CommentRegex.Replace(line, "");
        return removeComments.Trim();
        
    }

    public static string CsvQuote(string? str, bool always = false)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;

        if (!always && !CommaOrQuoteCharacter.IsMatch(str)) return str;

        return $"\"{str.Replace("\"", "\\\"")}\"";
    }
    
    public static string CsvQuoteAlways(string? str) => CsvQuote(str, true);
}