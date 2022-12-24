using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Exceptions;

namespace Socolin.TestUtils.JsonComparer.Handlers
{
    public interface IJsonSpecialHandler
    {
        (bool success, IList<IJsonCompareError<JToken>> errors) HandleSpecialObject(JToken expected, JToken actual, string path, IJsonComparer jsonComparer, JsonComparisonOptions options);
    }

    public class JsonSpecialHandler : IJsonSpecialHandler
    {
        private readonly Action<string, JToken> _captureValueHandler;
        private readonly IJsonObjectComparer _jsonObjectPartialComparer;
        private readonly IPartialArrayHandler _partialArrayHandler;
        private readonly RegexAliasesContainer _regexAliasesContainer;

        public JsonSpecialHandler(
            Action<string, JToken> handler,
            IJsonObjectComparer jsonObjectPartialComparer,
            IPartialArrayHandler partialArrayHandler,
            RegexAliasesContainer regexAliasesContainer
        )
        {
            _captureValueHandler = handler;
            _jsonObjectPartialComparer = jsonObjectPartialComparer;
            _partialArrayHandler = partialArrayHandler;
            _regexAliasesContainer = regexAliasesContainer;
        }

        public (bool success, IList<IJsonCompareError<JToken>> errors) HandleSpecialObject(JToken expected, JToken actual, string path, IJsonComparer jsonComparer, JsonComparisonOptions options)
        {
            if (IsCaptureObject(expected))
                return HandleCaptureObject(expected, actual, path);

            if (IsMatchObject(expected))
                return HandleMatchObject(expected, actual, path);

            if (IsPartialObject(expected))
                return HandlePartialObject(expected, actual, path, jsonComparer, options);

            if (IsPartialArrayObject(expected))
                return _partialArrayHandler.HandlePartialArrayObject(expected, actual, path, jsonComparer, options);

            return (false, null);
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandleCaptureObject(JToken expected, JToken actual, string path)
        {
            var jCaptureObject = ((JObject)expected).Value<JObject>("__capture");
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
            var jMatchObject = ((JObject)expected).Value<JObject>("__match");
            if (jMatchObject.ContainsKey("regex"))
            {
                if (actual.Type != JTokenType.String)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});
                var regex = jMatchObject.Value<string>("regex");
                var actualValue = actual.Value<string>();
                if (!Regex.IsMatch(actualValue, regex, RegexOptions.CultureInvariant))
                    return (false, new List<IJsonCompareError<JToken>> {new RegexMismatchMatchJsonCompareError(path, expected, actual, regex)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            if (jMatchObject.ContainsKey("regexAlias"))
            {
                if (actual.Type != JTokenType.String)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});
                var regexAlias = jMatchObject.Value<string>("regexAlias");
                var regex = _regexAliasesContainer.GetRegex(regexAlias);
                if (regex == null)
                    throw new RegexAliasNotRegisteredException(regexAlias);
                var actualValue = actual.Value<string>();
                if (!regex.IsMatch(actualValue))
                    return (false, new List<IJsonCompareError<JToken>> {new RegexMismatchMatchJsonCompareError(path, expected, actual, regex.ToString())});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            if (jMatchObject.TryGetValue("type", out var jType))
            {
                var expectedType = jType.Value<string>();
                if (!Enum.TryParse(expectedType, true, out JTokenType type))
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidMatchObjectJsonCompareError(path, expected, actual, $"Invalid `type`: value '{expectedType}' is not valid, see JTokenType for list of type")});
                if (type != actual.Type)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            if (jMatchObject.TryGetValue("range", out var jRangeArray))
            {
                if (actual.Type != JTokenType.Integer && actual.Type != JTokenType.Float)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});
                if (jRangeArray.Type != JTokenType.Array)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidMatchObjectJsonCompareError(path, expected, actual, "Invalid `range`: range should be an array of 2 number [min, max]")});
                if (((JArray)jRangeArray).Count != 2)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidMatchObjectJsonCompareError(path, expected, actual, "Invalid `range`: range should be an array of 2 number [min, max]")});
                var range = jRangeArray.ToObject<decimal[]>();
                var actualValue = actual.Value<decimal>();
                if (actualValue < range[0] || actualValue > range[1])
                    return (false, new List<IJsonCompareError<JToken>> {new ValueOutOfRangeComparerError(path, expected, actual, range)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            if (jMatchObject.ContainsKey("ignoredCharacters"))
            {
                if (actual.Type != JTokenType.String)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, expected, actual)});

                var expectedValue = jMatchObject.Value<string>("value");
                var ignoredCharacters = jMatchObject.Value<string>("ignoredCharacters");
                for (var i = expectedValue.Length - 1; i >= 0; i--)
                {
                    if (ignoredCharacters.Contains(expectedValue[i]))
                        expectedValue = expectedValue.Remove(i, 1);
                }

                var actualValue = actual.Value<string>();
                for (var i = actualValue.Length - 1; i >= 0; i--)
                {
                    if (ignoredCharacters.Contains(actualValue[i]))
                        actualValue = actualValue.Remove(i, 1);
                }

                if (actualValue != expectedValue)
                    return (false, new List<IJsonCompareError<JToken>> {new InvalidValueJsonCompareError(path, new JValue(expectedValue), actual as JValue)});

                if (expected.Parent is JProperty parentProperty)
                    parentProperty.Value = actual.DeepClone();

                return (true, null);
            }

            return (false, new List<IJsonCompareError<JToken>> {new InvalidMatchObjectJsonCompareError(path, expected, actual, "Missing `regex`, `range` or `type` field on match object")});
        }

        private (bool success, IList<IJsonCompareError<JToken>> errors) HandlePartialObject(JToken expected, JToken actual, string path, IJsonComparer jsonComparer, JsonComparisonOptions options)
        {
            var jPartialObject = expected.Value<JToken>("__partial");

            if (jPartialObject.Type == JTokenType.Array)
                return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` of __partial object. Use __partialArray to compare array")});
            if (jPartialObject.Type != JTokenType.Object)
                return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` of __partial object. Partial comparison is only supported for JSON Object")});

            if (actual.Type != jPartialObject.Type)
                return (false, new List<IJsonCompareError<JToken>> {new InvalidTypeJsonCompareError(path, jPartialObject, actual)});

            var errors = _jsonObjectPartialComparer.Compare(jPartialObject as JObject, actual as JObject, jsonComparer, options: options).ToList();
            if (expected.Parent is JProperty parentProperty)
                parentProperty.Value = jPartialObject;

            if (errors.Count > 0)
                return (false, errors);

            return (true, null);
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

        private static bool IsPartialObject(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
                return false;
            if (!(jToken is JObject expectedJObject))
                return false;
            return expectedJObject.ContainsKey("__partial");
        }

        private static bool IsPartialArrayObject(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
                return false;
            if (!(jToken is JObject expectedJObject))
                return false;
            return expectedJObject.ContainsKey("__partialArray");
        }
    }
}
