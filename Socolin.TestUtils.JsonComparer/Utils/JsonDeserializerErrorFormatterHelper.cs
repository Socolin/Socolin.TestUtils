#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Socolin.TestUtils.JsonComparer.Exceptions;

namespace Socolin.TestUtils.JsonComparer.Utils
{
    public static class JsonDeserializerErrorFormatterHelper
    {
        public static T DeserializeWithNiceErrorMessage<T>(string? json, JsonSerializerSettings? jsonSerializerSettings = null, bool useColor = false)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
            }
            catch (JsonReaderException ex)
            {
                var splitJson = json!.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
                var jsonWithErrorMarker = BuildJsonWithErrorMarker(ex, splitJson, useColor);

                throw new InvalidJsonException($"Invalid JSON found. At line {ex.LineNumber} at position: {ex.LinePosition}" +
                                               $"\n\n{jsonWithErrorMarker}", ex);
            }
        }

        private static StringBuilder BuildJsonWithErrorMarker(JsonReaderException ex, IReadOnlyList<string> lines, bool useColor)
        {
            var jsonWithErrorMarker = new StringBuilder();
            var contextSize = 3;
            for (var i = -contextSize; i <= contextSize; i++)
            {
                if (ex.LineNumber + i < 1)
                    continue;
                if (ex.LineNumber + i > lines.Count)
                    continue;

                var line = lines[ex.LineNumber - 1 + i];
                jsonWithErrorMarker.Append((ex.LineNumber + i).ToString().PadRight(4, ' ')).Append('|');
                jsonWithErrorMarker.AppendLine(line);
                if (i == 0)
                {
                    if (useColor)
                        jsonWithErrorMarker.Append(AnsiConsoleColors.RED);
                    jsonWithErrorMarker.AppendLine("".PadLeft(ex.LinePosition + 5) + "^-- " + ex.Message);
                    if (useColor)
                        jsonWithErrorMarker.Append(AnsiConsoleColors.RESET);
                }
            }

            return jsonWithErrorMarker;
        }
    }
}
