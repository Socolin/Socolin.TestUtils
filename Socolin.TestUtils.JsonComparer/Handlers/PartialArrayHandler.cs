using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Handlers;

public interface IPartialArrayHandler
{
    (bool success, IList<IJsonCompareError<JToken>> errors) HandlePartialArrayObject(JToken expected, JToken actual, string path, IJsonComparer? jsonComparer, JsonComparisonOptions? options);
}

public class PartialArrayHandler : IPartialArrayHandler
{
    public (bool success, IList<IJsonCompareError<JToken>> errors) HandlePartialArrayObject(JToken expected, JToken actual, string path, IJsonComparer? jsonComparer, JsonComparisonOptions? options)
    {
        var jPartialObject = expected.Value<JToken>("__partialArray");
        if (jPartialObject?.Type != JTokenType.Object)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` of __partialArray object. It should be an object")});
        if (actual.Type != JTokenType.Array)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` compared with __partialArray object. It should be an array")});
        var actualArray = actual.Value<JArray>();
        if (actualArray == null)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Invalid `type` compared with __partialArray object. It should not be null")});
        var partialJObject = jPartialObject.Value<JObject>();
        var expectedArray = partialJObject?.Value<JArray>("array");
        if (partialJObject == null || expectedArray == null)
            return (false, new List<IJsonCompareError<JToken>> {new InvalidPartialObjectCompareError(path, expected, actual, $"Missing `array` inside __partialArray object.")});

        var errors = new List<IJsonCompareError<JToken>>();

        var keyField = partialJObject.Value<string>("key");
        if (keyField == null)
        {
            foreach (var expectedElement in expectedArray)
            {
                var expectedValue = expectedElement.Value<JValue>();
                if (!actualArray.Any(x => Equals(x, expectedValue)))
                    errors.Add(new MissingObjectInArrayComparerError(path, expectedElement, expectedValue));
            }
        }
        else
        {
            foreach (var expectedElement in expectedArray)
            {
                var expectedKeyValue = expectedElement.Value<JObject>()?.Value<JValue>(keyField);
                var actualElement = actualArray.Where(x => x.Type == JTokenType.Object).FirstOrDefault(e => e.Value<JObject>()?.Value<JValue>(keyField)?.Equals(expectedKeyValue) == true);
                if (actualElement == null)
                {
                    errors.Add(new MissingObjectInArrayComparerError(path, expectedElement, expectedKeyValue));
                    continue;
                }

                if (jsonComparer == null)
                    throw new NullReferenceException("jsonComparer is not available");

                var subComparerErrors = jsonComparer.Compare(expectedElement, actualElement, options);
                errors.AddRange(subComparerErrors);
            }

        }

        if (actual.Parent is JProperty parentProperty)
        {
            var finalActualArray = new JArray();
            foreach (var actualElement in actualArray)
            {
                if (keyField == null)
                {
                    if (expectedArray.Any(e => Equals(actualElement, e)))
                        finalActualArray.Add(actualElement);
                }
                else
                {
                    var actualKeyValue = actualElement.Value<JObject>()?.Value<JValue>(keyField);
                    var expectedElement = expectedArray.Where(x => x.Type == JTokenType.Object).FirstOrDefault(e => e.Value<JObject>()?.Value<JValue>(keyField)?.Equals(actualKeyValue) == true);
                    if (expectedElement != null)
                        finalActualArray.Add(actualElement);
                }
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
