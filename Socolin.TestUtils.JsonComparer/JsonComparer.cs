using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Color;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Handlers;
using Socolin.TestUtils.JsonComparer.Utils;

namespace Socolin.TestUtils.JsonComparer;

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
    private readonly IJsonDeserializer _jsonDeserializer;

    public static JsonComparer GetDefault(
        Action<string, JToken> captureHandler = null,
        bool useColor = false,
        JsonComparerColorOptions colorOptions = null,
        RegexAliasesContainer regexAliasesContainer = null
    )
    {
        return new JsonComparer(
            new JsonObjectComparer(),
            new JsonArrayComparer(),
            new JsonValueComparer(),
            new JsonSpecialHandler(captureHandler, new JsonObjectPartialComparer(), new PartialArrayHandler(), regexAliasesContainer ?? new RegexAliasesContainer()),
            new JsonDeserializerWithNiceError(colorOptions ?? (useColor ? JsonComparerColorOptions.DefaultColored : JsonComparerColorOptions.Default))
        );
    }

    public JsonComparer(
        IJsonObjectComparer jsonObjectComparer,
        IJsonArrayComparer jsonArrayComparer,
        IJsonValueComparer jsonValueComparer,
        IJsonSpecialHandler jsonSpecialHandler,
        IJsonDeserializer jsonDeserializer
    )
    {
        _jsonObjectComparer = jsonObjectComparer;
        _jsonArrayComparer = jsonArrayComparer;
        _jsonValueComparer = jsonValueComparer;
        _jsonSpecialHandler = jsonSpecialHandler;
        _jsonDeserializer = jsonDeserializer;
    }

    public IList<IJsonCompareError<JToken>> Compare(string expectedJson, string actualJson, JsonComparisonOptions options = null)
    {
        var expected = _jsonDeserializer.Deserialize<JToken>(expectedJson,
            new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            });
        var actual = _jsonDeserializer.Deserialize<JToken>(actualJson,
            new JsonSerializerSettings
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
        var (captureSucceeded, captureErrors) = _jsonSpecialHandler.HandleSpecialObject(expected, actual, path, this, options);
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
                throw new ArgumentOutOfRangeException(nameof(actual.Type), actual.Type, @"Cannot compare this type");
        }

        foreach (var jsonCompareError in errors)
        {
            yield return jsonCompareError;
        }
    }
}
