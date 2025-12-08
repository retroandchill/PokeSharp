using System.Text;

namespace PokeSharp.Core.Strings;

public static class StringExtensions
{
    extension(string value)
    {
        public string Escape()
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var sb = new StringBuilder(value.Length + 8);

            foreach (var c in value)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\0':
                        sb.Append("\\0");
                        break;
                    default:
                        // Optional: escape other control chars as \xNN
                        if (char.IsControl(c))
                        {
                            sb.Append("\\x");
                            sb.Append(((int)c).ToString("X2"));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        public string Unescape()
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var sb = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c != '\\')
                {
                    sb.Append(c);
                    continue;
                }

                // Backslash at end: keep as-is
                if (i + 1 >= value.Length)
                {
                    sb.Append('\\');
                    break;
                }

                var next = value[++i];
                switch (next)
                {
                    case '\\':
                        sb.Append('\\');
                        break;
                    case '\"':
                        sb.Append('\"');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 't':
                        sb.Append('\t');
                        break;
                    case '0':
                        sb.Append('\0');
                        break;

                    case 'x':
                        // Expect up to 2 hex digits (to match the escaper's \xNN)
                        var hexStart = i + 1;
                        var hexLen = 0;
                        while (hexStart + hexLen < value.Length && hexLen < 2 && IsHexDigit(value[hexStart + hexLen]))
                        {
                            hexLen++;
                        }

                        if (hexLen == 0)
                        {
                            // No valid hex digits after \x – treat "\x" literally
                            sb.Append('\\').Append('x');
                        }
                        else
                        {
                            var hex = value.Substring(hexStart, hexLen);
                            var value1 = Convert.ToInt32(hex, 16);
                            sb.Append((char)value1);
                            i += hexLen; // we've consumed hexLen more chars
                        }
                        break;

                    default:
                        // Unknown escape: keep as-is (e.g. "\q" -> "\q")
                        sb.Append('\\').Append(next);
                        break;
                }
            }

            return sb.ToString();
        }
    }

    private static bool IsHexDigit(char c)
    {
        return c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
    }
}
