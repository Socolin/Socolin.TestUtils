using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.ANSITerminalColor;

namespace Socolin.TestUtils.JsonComparer.Color;

public class ColorizeJsonWriter : JsonTextWriter
{
    public class Theme
    {
        public AnsiColor KeywordColor { get; set; } = AnsiColor.Foreground(108, 149, 235);
        public AnsiColor StringColor { get; set; } = AnsiColor.Foreground(201, 162, 109);
        public AnsiColor PropertyKeyColor { get; set; } = AnsiColor.Foreground(102, 195, 204);
        public AnsiColor NumberColor { get; set; } = AnsiColor.Foreground(237, 148, 192);
    }

    private readonly TextWriter _textWriter;
    private readonly JsonComparerColorOptions _colorOptions;

    public ColorizeJsonWriter(
        TextWriter textWriter,
        JsonComparerColorOptions colorOptions
    )
        : base(textWriter)
    {
        _textWriter = textWriter;
        _colorOptions = colorOptions;
    }

    public override void WritePropertyName(string name, bool escape)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.PropertyKeyColor.ToEscapeSequence());
        base.WritePropertyName(name, escape);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.PropertyKeyColor.ToResetSequence());
    }

    public override void WritePropertyName(string name)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.PropertyKeyColor.ToEscapeSequence());
        base.WritePropertyName(name);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.PropertyKeyColor.ToResetSequence());
    }

    public override void WriteValue(bool value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.KeywordColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.KeywordColor.ToResetSequence());
    }

    public override void WriteValue(string value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.StringColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.StringColor.ToResetSequence());
    }

    #region Number

    public override void WriteValue(byte value)
    {
        WriteIntegerValue(value);
    }

    public override void WriteValue(int value)
    {
        WriteIntegerValue(value);
    }

    public override void WriteValue(short value)
    {
        WriteIntegerValue(value);
    }

    public override void WriteValue(long value)
    {
        WriteIntegerValue(value);
    }

    private void WriteIntegerValue(long value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToResetSequence());
    }

    public override void WriteValue(decimal value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToResetSequence());
    }
    public override void WriteValue(float value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToResetSequence());
    }
    public override void WriteValue(double value)
    {
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToEscapeSequence());
        base.WriteValue(value);
        if (JsonComparerColorHelper.ShouldColorize(_colorOptions.ColorizeJson))
            _textWriter.Write(_colorOptions.Theme.Json.NumberColor.ToResetSequence());
    }

    #endregion

    public static string FormatAndColorizeJson(JToken token, JsonComparerColorOptions colorOptions)
    {
        var memoryStream = new MemoryStream();
        var writer = new ColorizeJsonWriter(new StreamWriter(memoryStream), colorOptions);
        var serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        serializer.Serialize(writer, token);
        writer.Flush();

        memoryStream.Position = 0;
        return new StreamReader(memoryStream, Encoding.UTF8).ReadToEnd();
    }
}
