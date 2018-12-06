using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Utils;

namespace Socolin.TestUtils.JsonComparer.Comparers
{
    public interface IJsonObjectComparer
    {
        IEnumerable<IJsonCompareError<JToken>> Compare(JObject expected, JObject actual, IJsonComparer jsonComparer, string path = "");
    }

    public class JsonObjectComparer : IJsonObjectComparer
    {
        public IEnumerable<IJsonCompareError<JToken>> Compare(JObject expected, JObject actual, IJsonComparer jsonComparer, string path = "")
        {
            foreach (var actualProperty in actual.Properties())
            {
                var expectedProperty = expected.Property(actualProperty.Name);
                if (expectedProperty == null)
                {
                    yield return new UnexpectedPropertyJsonComparerError(path, expected, actual, actualProperty);
                }
            }

            foreach (var expectedProperty in expected.Properties())
            {
                var actualProperty = actual.Property(expectedProperty.Name);
                var expectedJToken = expectedProperty.Value;
                if (actualProperty == null)
                {
                    yield return new MissingPropertyJsonComparerError(path, expected, actual, expectedProperty);
                    continue;
                }

                var elementPath = JsonPathUtils.Combine(path, actualProperty.Name);
                var errors = jsonComparer.Compare(expectedJToken, actualProperty.Value, elementPath);
                foreach (var jsonCompareError in errors)
                {
                    yield return jsonCompareError;
                }
            }
        }
    }
}