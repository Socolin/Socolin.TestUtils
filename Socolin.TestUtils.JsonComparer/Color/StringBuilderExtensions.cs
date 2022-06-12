using System.Text;
using Socolin.ANSITerminalColor;

namespace Socolin.TestUtils.JsonComparer.Color;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendColoredLine(this StringBuilder sb, string text, bool requestColorize, AnsiColor color)
    {
        var doColorize = JsonComparerColorHelper.ShouldColorize(requestColorize);
        if (doColorize)
            color.ToString(sb);
        sb.AppendLine(text);
        if (doColorize)
            AnsiColor.Reset.ToString(sb);
        return sb;
    }
}
