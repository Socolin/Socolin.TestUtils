using Socolin.ANSITerminalColor;

namespace Socolin.TestUtils.JsonComparer.Color;

public static class JsonComparerColorHelper
{
    private static readonly bool NoColor = Environment.GetEnvironmentVariable("NO_COLOR") != null;

    public static bool ShouldColorize(bool useColor)
    {
        if (NoColor)
            return false;

        return useColor;
    }

#if NET3_0_OR_GREATER
	[return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("text")]
#endif
    public static string? Colorize(string? text, AnsiColor color, bool useColor)
    {
        if (text == null)
            return null;
        if (!ShouldColorize(useColor))
            return text;

        return color.Colorize(text);
    }
}
