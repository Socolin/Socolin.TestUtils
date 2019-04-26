using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Handlers
{
    public interface IJsonSpecialHandler
    {
        (bool success, IList<IJsonCompareError<JToken>> errors) HandleSpecialObject(JToken expected, JToken actual, string path);
    }

    public class JsonSpecialHandler : IJsonSpecialHandler
    {
        private readonly Action<string, JToken> _captureValueHandler;

        public JsonSpecialHandler(Action<string, JToken> handler)
        {
            _captureValueHandler = handler;
        }

        public (bool success, IList<IJsonCompareError<JToken>> errors) HandleSpecialObject(JToken expected, JToken actual, string path)
        {
            if (IsCaptureObject(expected))
                return HandleCaptureObject(expected, actual, path);

            if (IsMatchObject(expected))
                return HandleMatchObject(expected, actual, path);

            return (false, null);
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandleCaptureObject(JToken expected, JToken actual, string path)
        {
            var jCaptureObject = ((JObject) expected).Value<JObject>("__capture");
            if (jCaptureObject.ContainsKey("type"))
                return HandleCaptureObjectWithType(expected, actual, path, jCaptureObject);
            if (jCaptureObject.ContainsKey("regex"))
                return HandleCaptureObjectWithRegex(expected, actual, path, jCaptureObject);
            return (false, new List<IJsonCompareError<JToken>> {new InvalidCaptureObjectCompareError(path, expected, actual, "Missing `type` or `regex` field on capture object")});
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandleCaptureObjectWithRegex(JToken expected, JToken actual, string path, JObject jCaptureObject)
        {
            var captureObject = jCaptureObject.ToObject<JsonCaptureObject>();

            if (string.IsNullOrEmpty(captureObject.Regex))
                return (false, new List<IJsonCompareError<JToken>> {new InvalidCaptureObjectCompareError(path, expected, actual, "Empty `regex` field on capture object")});

            if (actual.Type != JTokenType.String)
                return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});

            var regex = new Regex(captureObject.Regex, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var match = regex.Match(actual.Value<string>());
            if (!match.Success)
                return (false, new List<IJsonCompareError<JToken>> {new RegexMismatchMatchJsonCompareError(path, expected, actual, captureObject.Regex)});

            if (captureObject.Name != null)
                _captureValueHandler?.Invoke(captureObject.Name, actual);

            var groupNumbers = regex.GetGroupNumbers();
            foreach (var groupNumber in groupNumbers)
            {
                var groupName = regex.GroupNameFromNumber(groupNumber);
                if (!string.IsNullOrEmpty(groupName) && groupName != groupNumber.ToString())
                    _captureValueHandler?.Invoke(groupName, JValue.CreateString(match.Groups[groupNumber].Value));
            }

            if (expected.Parent is JProperty parentProperty)
                parentProperty.Value = actual.DeepClone();

            return (true, null);
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandleCaptureObjectWithType(JToken expected, JToken actual, string path, JObject jCaptureObject)
        {
            if (!jCaptureObject.ContainsKey("name"))
                return (false, new List<IJsonCompareError<JToken>> {new InvalidCaptureObjectCompareError(path, expected, actual, "Missing `name` field on capture object")});

            var captureObject = jCaptureObject.ToObject<JsonCaptureObject>();

            if (!Enum.TryParse(captureObject.Type, true, out JTokenType type))
                return (false, new List<IJsonCompareError<JToken>> {new InvalidCaptureObjectCompareError(path, expected, actual, $"Invalid `type`: value '{captureObject.Type}' is not valid, see JTokenType for list of type")});

            if (type != actual.Type)
                return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});

            _captureValueHandler?.Invoke(captureObject.Name, actual);

            if (expected.Parent is JProperty parentProperty)
                parentProperty.Value = actual.DeepClone();

            return (true, null);
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandleMatchObject(JToken expected, JToken actual, string path)
        {
            var jCaptureObject = ((JObject) expected).Value<JObject>("__match");
            if (jCaptureObject.ContainsKey("regex"))
            {
                if (actual.Type != JTokenType.String)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});
                var regex = jCaptureObject.Value<string>("regex");
                var actualValue = actual.Value<string>();
                if (!Regex.IsMatch(actualValue, regex, RegexOptions.CultureInvariant))
                    return (false, new List<IJsonCompareError<JToken>> {new RegexMismatchMatchJsonCompareError(path, expected, actual, regex)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            if (jCaptureObject.ContainsKey("type"))
            {
                var expectedType = jCaptureObject.Value<string>("type");
                if (!Enum.TryParse(expectedType, true, out JTokenType type))
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidCaptureObjectCompareError(path, expected, actual, $"Invalid `type`: value '{expectedType}' is not valid, see JTokenType for list of type")});
                if (type != actual.Type)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            return (false, new List<IJsonCompareError<JToken>> {new InvalidMatchObjectJsonCompareError(path, expected, actual, "Missing `regex` field on capture object")});
        }

        private static bool IsCaptureObject(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
                return false;
            if (!(jToken is JObject expectedJObject))
                return false;
            return expectedJObject.ContainsKey("__capture");
        }

        private static bool IsMatchObject(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
                return false;
            if (!(jToken is JObject expectedJObject))
                return false;
            return expectedJObject.ContainsKey("__match");
        }
    }
}
