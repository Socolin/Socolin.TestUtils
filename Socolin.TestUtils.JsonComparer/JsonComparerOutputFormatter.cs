using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGit.Diff;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer
{
    public class JsonComparerOutputFormatter
    {
        private class Colors
        {
            private const string EscapeCode = "\u001b";
            public static readonly string RESET = $"{EscapeCode}[0m";
            public static readonly string RED = $"{EscapeCode}[31m";
            public static readonly string GREEN = $"{EscapeCode}[32m";
            // ReSharper disable UnusedMember.Local
            public static readonly string YELLOW = $"{EscapeCode}[33m";
            public static readonly string BLUE = $"{EscapeCode}[34m";
            public static readonly string MAGENTA = $"{EscapeCode}[35m";
            public static readonly string CYAN = $"{EscapeCode}[36m";
            public static readonly string WHITE = $"{EscapeCode}[37m";
            // ReSharper restore UnusedMember.Local
        }

        public static string GetReadableMessage(string expectedJson, string actualJson, IEnumerable<IJsonCompareError<JToken>> errors, bool useColor = false)
        {
            return GetReadableMessage(JToken.Parse(expectedJson), JToken.Parse(actualJson), errors, useColor);
        }

        public static string GetReadableMessage(JToken expectedJToken, JToken actualJToken, IEnumerable<IJsonCompareError<JToken>> errors, bool useColor = false)
        {
            var compareErrors = errors.ToList();
            if (compareErrors.Count == 0)
                return "No differences found";

            var sb = new StringBuilder();

            sb.AppendLine("Given json does not match expected one: ");
            foreach (var error in compareErrors)
                sb.AppendLine($"  - {error.Path}: {error.Message}");

            sb.AppendLine();
            if (useColor) sb.Append(Colors.RED);
            sb.AppendLine("--- expected");
            if (useColor) sb.Append(Colors.GREEN);
            sb.AppendLine("+++ actual");
            if (useColor) sb.Append(Colors.RESET);

            WriteUnifiedDiffBetweenJson(sb, expectedJToken, actualJToken, useColor);
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private static void WriteUnifiedDiffBetweenJson(StringBuilder sb, JToken expectedJToken, JToken actualJToken, bool useColor)
        {
            NormalizeForTextDiffJson(expectedJToken);
            NormalizeForTextDiffJson(actualJToken);
            var expectedJson = JsonConvert.SerializeObject(expectedJToken, Formatting.Indented).Replace("\r\n", "\n");
            var actualJson = JsonConvert.SerializeObject(actualJToken, Formatting.Indented).Replace("\r\n", "\n");
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
                        if (useColor) sb.Append(Colors.RED);
                        sb.AppendLine("-" + expectedLines[a]);
                        a++;
                    }

                    while (b < difference.GetEndB())
                    {
                        if (useColor) sb.Append(Colors.GREEN);
                        sb.AppendLine("+" + actualLines[b]);
                        b++;
                    }

                    if (useColor) sb.Append(Colors.RESET);
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
}
