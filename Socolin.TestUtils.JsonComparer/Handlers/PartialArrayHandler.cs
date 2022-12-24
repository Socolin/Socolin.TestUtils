using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Handlers;

public interface IPartialArrayHandler
{
    (bool success, IList<IJsonCompareError<JToken>> errors) HandlePartialArrayObject(JToken expected, JToken actual, string path, IJsonComparer jsonComparer, JsonComparisonOptions options);
}

public class PartialArrayHandler : IPartialArrayHandler
{
    public (bool success, IList<IJsonCompareError<JToken>> errors) HandlePartialArrayObject(JToken expected, JToken actual, string path, IJsonComparer jsonComparer, JsonComparisonOptions options)
    {
        // Maybe create a version later to compare array of int/string without key ? (could be done with __partial)

        var jPartialObject = expected.Value<JToken>("__partialArray");
        if (jPartialObject.Type != JTokenType.Object)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` of __partialArray object. It should be an object")});
        if (actual.Type != JTokenType.Array)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` compared with __partialArray object. It should be an array")});
        var partialJObject = jPartialObject.Value<JObject>();
        if (partialJObject.Property("array") == null)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Missing `array` inside __partialArray object.")});
        if (partialJObject.Property("key") == null)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Missing `key` inside __partialArray object.")});

        var expectedArray = partialJObject.Value<JArray>("array");
        var keyField = partialJObject.Value<string>("key");
        var actualArray = actual.Value<JArray>();

        var errors = new List<IJsonCompareError<JToken>>();
        foreach (var expectedElement in expectedArray)
        {
            var expectedKeyValue = expectedElement.Value<JObject>().Value<JValue>(keyField);
            var actualElement = actualArray.Where(x => x.Type == JTokenType.Object).FirstOrDefault(e => e.Value<JObject>().Value<JValue>(keyField).Equals(expectedKeyValue));
            if (actualElement == null)
            {
                errors.Add(new MissingObjectInArrayComparerError(path ,expectedElement, expectedKeyValue));
                continue;
            }

            var subComparerErrors = jsonComparer.Compare(expectedElement, actualElement, options);
            errors.AddRange(subComparerErrors);
        }

        if (actual.Parent is JProperty parentProperty)
        {
            var finalActualArray = new JArray();
            foreach (var actualElement in actualArray)
            {
                var actualKeyValue = actualElement.Value<JObject>().Value<JValue>(keyField);
                var expectedElement = expectedArray.Where(x => x.Type == JTokenType.Object).FirstOrDefault(e => e.Value<JObject>().Value<JValue>(keyField).Equals(actualKeyValue));
                if (expectedElement != null)
                    finalActualArray.Add(actualElement);
            }

            // FIXME: Sort finalActualArray with same order as expectedArray

            parentProperty.Value = finalActualArray;
        }

        if (expected.Parent is JProperty expectedParentProperty)
        {
            expectedParentProperty.Value = expectedArray.DeepClone();
        }

        return (errors.Count == 0, errors);
    }
}