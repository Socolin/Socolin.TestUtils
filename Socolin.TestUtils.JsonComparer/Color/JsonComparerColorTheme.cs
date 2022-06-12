using Socolin.ANSITerminalColor;

namespace Socolin.TestUtils.JsonComparer.Color;

public class JsonComparerColorTheme
{
    // Use this to change the default colors
    public static readonly JsonComparerColorTheme Default = new();
    public AnsiColor Error { get; set; } = AnsiColor.Foreground(Terminal256ColorCodes.RedC9);
    public AnsiColor DiffDeletion { get; set; } = AnsiColor.Foreground(Terminal256ColorCodes.RedC9);
    public AnsiColor DiffAddition { get; set; } = AnsiColor.Foreground(Terminal256ColorCodes.GreenC2);

    public ColorizeJsonWriter.Theme Json { get; set; } = new();
}
