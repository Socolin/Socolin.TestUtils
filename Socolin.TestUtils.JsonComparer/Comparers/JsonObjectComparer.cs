using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Utils;

namespace Socolin.TestUtils.JsonComparer.Comparers;

public class JsonObjectComparer : IJsonObjectComparer
{
    public IEnumerable<IJsonCompareError<JToken>> Compare(JObject expected, JObject actual, IJsonComparer jsonComparer, string path = "", JsonComparisonOptions options = null)
    {
        // Copy the list (with .ToList()) so we can remove properties from JObject while iterating on it
        foreach (var actualProperty in actual.Properties().ToList())
        {
            var expectedProperty = expected.Property(actualProperty.Name);
            if (expectedProperty == null)
            {
                if (options?.IgnoreFields(path, actualProperty.Name) == true)
                    actual.Remove(actualProperty.Name);
                else
                    yield return new UnexpectedPropertyJsonComparerError(path, expected, actual, actualProperty);
            }
        }

        // Copy the list (with .ToList()) so we can remove properties from JObject while iterating on it
        foreach (var expectedProperty in expected.Properties().ToList())
        {
            var actualProperty = actual.Property(expectedProperty.Name);
            var expectedJToken = expectedProperty.Value;
            if (actualProperty == null)
            {
                if (options?.IgnoreFields(path, expectedProperty.Name) == true)
                    expected.Remove(expectedProperty.Name);
                else
                    yield return new MissingPropertyJsonComparerError(path, expected, actual, expectedProperty);
                continue;
            }

            if (options?.IgnoreFields(path, actualProperty.Name) == true)
            {
                expected.Remove(expectedProperty.Name);
                actual.Remove(expectedProperty.Name);
            }
            else
            {
                var elementPath = JsonPathUtils.Combine(path, actualProperty.Name);
                var errors = jsonComparer.Compare(expectedJToken, actualProperty.Value, elementPath, options);
                foreach (var jsonCompareError in errors)
                {
                    yield return jsonCompareError;
                }
            }
        }
    }
}