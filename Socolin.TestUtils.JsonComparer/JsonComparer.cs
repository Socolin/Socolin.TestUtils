using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Handlers;

namespace Socolin.TestUtils.JsonComparer
{
    public interface IJsonComparer
    {
        IList<IJsonCompareError<JToken>> Compare(string expectedJson, string actualJson, JsonComparisonOptions options = null);
        IList<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, JsonComparisonOptions options = null);
        IEnumerable<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, string path, JsonComparisonOptions options = null);
    }

    public class JsonComparer : IJsonComparer
    {
        private readonly IJsonObjectComparer _jsonObjectComparer;
        private readonly IJsonArrayComparer _jsonArrayComparer;
        private readonly IJsonValueComparer _jsonValueComparer;
        private readonly IJsonSpecialHandler _jsonSpecialHandler;

        public static JsonComparer GetDefault(Action<string, JToken> captureHandler = null)
        {
            return new JsonComparer(
                new JsonObjectComparer(),
                new JsonArrayComparer(),
                new JsonValueComparer(),
                new JsonSpecialHandler(captureHandler, new JsonObjectPartialComparer())
            );
        }

        public JsonComparer(
            IJsonObjectComparer jsonObjectComparer,
            IJsonArrayComparer jsonArrayComparer,
            IJsonValueComparer jsonValueComparer,
            IJsonSpecialHandler jsonSpecialHandler)
        {
            _jsonObjectComparer = jsonObjectComparer;
            _jsonArrayComparer = jsonArrayComparer;
            _jsonValueComparer = jsonValueComparer;
            _jsonSpecialHandler = jsonSpecialHandler;
        }

        public IList<IJsonCompareError<JToken>> Compare(string expectedJson, string actualJson, JsonComparisonOptions options = null)
        {
            var expected = JsonConvert.DeserializeObject<JToken>(expectedJson, new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            });
            var actual = JsonConvert.DeserializeObject<JToken>(actualJson, new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            });
            return Compare(expected, actual, options);
        }

        public IList<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, JsonComparisonOptions options = null)
        {
            return Compare(expected, actual, "", options).ToList();
        }

        public IEnumerable<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, string path, JsonComparisonOptions options = null)
        {
            var (captureSucceeded, captureErrors) = _jsonSpecialHandler.HandleSpecialObject(expected, actual, path, this);
            if (captureSucceeded)
                yield break;
            if (captureErrors?.Count > 0)
            {
                foreach (var error in captureErrors)
                    yield return error;
                yield break;
            }

            if (expected.Type != actual.Type)
            {
                yield return new InvalidTypeJsonCompareError(path, expected, actual);
                yield break;
            }

            IEnumerable<IJsonCompareError<JToken>> errors;
            switch (actual.Type)
            {
                case JTokenType.Object:
                    errors = _jsonObjectComparer.Compare(expected as JObject, actual as JObject, this, path, options);
                    break;
                case JTokenType.Array:
                    errors = _jsonArrayComparer.Compare(expected as JArray, actual as JArray, this, path, options);
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                    errors = _jsonValueComparer.Compare(expected as JValue, actual as JValue, path, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actual.Type), actual.Type, "Cannot compare this type");
            }

            foreach (var jsonCompareError in errors)
            {
                yield return jsonCompareError;
            }
        }
    }
}
