namespace Socolin.TestUtils.JsonComparer.Color;

public class JsonComparerColorOptions
{
    public static JsonComparerColorOptions DefaultColored { get; } = new() {ColorizeDiff = true, ColorizeParseError = true};
    public static JsonComparerColorOptions Default { get; } = new();
    public JsonComparerColorTheme Theme { get; set; } = JsonComparerColorTheme.Default;
    public bool ColorizeDiff { get; set; } = false;
    public bool ColorizeParseError { get; set; } = false;
    public bool ColorizeJson { get; set; } = false;
}
