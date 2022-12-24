using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGit.Diff;
using Socolin.TestUtils.JsonComparer.Color;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer;

public class JsonComparerOutputFormatter
{
    [Obsolete("Use the version with `JsonComparerColorOptions` parameter instead of `useColor`")]
    public static string GetReadableMessage(string expectedJson, string actualJson, IEnumerable<IJsonCompareError<JToken>> errors, bool useColor)
    {
        return GetReadableMessage(JToken.Parse(expectedJson), JToken.Parse(actualJson), errors, useColor);
    }

    public static string GetReadableMessage(string expectedJson, string actualJson, IEnumerable<IJsonCompareError<JToken>> errors, JsonComparerColorOptions colorOptions = null)
    {
        return GetReadableMessage(JToken.Parse(expectedJson), JToken.Parse(actualJson), errors, colorOptions ?? JsonComparerColorOptions.Default);
    }

    [Obsolete("Use the version with `colorOptions` parameter instead of `useColor`")]
    public static string GetReadableMessage(JToken expectedJToken, JToken actualJToken, IEnumerable<IJsonCompareError<JToken>> errors, bool useColor)
    {
        return GetReadableMessage(expectedJToken,
            actualJToken,
            errors,
            new JsonComparerColorOptions
            {
                ColorizeDiff = useColor,
                Theme = JsonComparerColorTheme.Default,
            });
    }

    public static string GetReadableMessage(JToken expectedJToken, JToken actualJToken, IEnumerable<IJsonCompareError<JToken>> errors, JsonComparerColorOptions colorOptions = null)
    {
        colorOptions ??= JsonComparerColorOptions.Default;

        var compareErrors = errors.ToList();
        if (compareErrors.Count == 0)
            return "No differences found";

        var sb = new StringBuilder();

        sb.AppendLine("Given json does not match expected one:");
        foreach (var error in compareErrors)
            sb.AppendLine($"  - {error.Path}: {error.Message}");

        sb.AppendLine();
        sb.AppendColoredLine("--- expected", colorOptions.ColorizeDiff, colorOptions.Theme.DiffDeletion);
        sb.AppendColoredLine("+++ actual", colorOptions.ColorizeDiff, colorOptions.Theme.DiffAddition);

        WriteUnifiedDiffBetweenJson(sb, expectedJToken, actualJToken, colorOptions);
        sb.AppendLine();
        sb.AppendLine();

        return sb.ToString();
    }

    private static void WriteUnifiedDiffBetweenJson(StringBuilder sb, JToken expectedJToken, JToken actualJToken, JsonComparerColorOptions colorOptions)
    {
        NormalizeForTextDiffJson(expectedJToken);
        NormalizeForTextDiffJson(actualJToken);

        string actualJson;
        string expectedJson;
        if (colorOptions.ColorizeJson)
        {
            expectedJson = ColorizeJsonWriter.FormatAndColorizeJson(expectedJToken, colorOptions).Replace("\r\n", "\n");
            actualJson = ColorizeJsonWriter.FormatAndColorizeJson(actualJToken, colorOptions).Replace("\r\n", "\n");
        }
        else
        {
            expectedJson = JsonConvert.SerializeObject(expectedJToken, Formatting.Indented).Replace("\r\n", "\n");
            actualJson = JsonConvert.SerializeObject(actualJToken, Formatting.Indented).Replace("\r\n", "\n");
        }

        var expectedText = new RawText(Encoding.UTF8.GetBytes(expectedJson));
        var actualText = new RawText(Encoding.UTF8.GetBytes(actualJson));
        var diffAlgorithm = DiffAlgorithm.GetAlgorithm(DiffAlgorithm.SupportedAlgorithm.HISTOGRAM);
        var differences = diffAlgorithm.Diff(RawTextComparator.WS_IGNORE_ALL, expectedText, actualText);
        if (differences.Count == 0)
            return;

        var expectedLines = expectedJson.Split('\n');
        var actualLines = actualJson.Split('\n');

        var a = 0;
        var b = 0;
        var i = 0;
        var difference = differences[i];

        while (a < expectedLines.Length && b < actualLines.Length)
        {
            if (difference != null && a >= difference.GetBeginA())
            {
                while (a < difference.GetEndA())
                {
                    sb.AppendColoredLine("-" + expectedLines[a], colorOptions.ColorizeDiff, colorOptions.Theme.DiffDeletion);
                    a++;
                }

                while (b < difference.GetEndB())
                {
                    sb.AppendColoredLine("+" + actualLines[b], colorOptions.ColorizeDiff, colorOptions.Theme.DiffAddition);
                    b++;
                }

                i++;
                difference = i < differences.Count ? differences[i] : null;
            }
            else
            {
                sb.AppendLine(" " + actualLines[b]);
                b++;
                a++;
            }
        }
    }

    private static void NormalizeForTextDiffJson(JToken jToken)
    {
        if (jToken is JObject jObject)
        {
            var properties = jObject.Properties().ToList();
            jObject.RemoveAll();
            foreach (var jProperty in properties.OrderBy(p => p.Name))
            {
                NormalizeForTextDiffJson(jProperty.Value);
                jObject.Add(jProperty);
            }
        }
        else if (jToken is JArray jArray)
        {
            foreach (var token in jArray)
            {
                NormalizeForTextDiffJson(token);
            }
        }
    }
}