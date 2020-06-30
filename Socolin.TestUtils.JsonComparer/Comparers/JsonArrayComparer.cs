using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Utils;

namespace Socolin.TestUtils.JsonComparer.Comparers
{
    public interface IJsonArrayComparer
    {
        IEnumerable<IJsonCompareError<JToken>> Compare(JArray expected, JArray actual, IJsonComparer jsonComparer, string path = "", JsonComparisonOptions options = null);
    }

    public class JsonArrayComparer : IJsonArrayComparer
    {
        public IEnumerable<IJsonCompareError<JToken>> Compare(JArray expected, JArray actual, IJsonComparer jsonComparer, string path = "", JsonComparisonOptions options = null)
        {
            if (expected.Count != actual.Count)
            {
                yield return new InvalidSizeJsonCompareError(path, expected, actual);
                yield break;
            }

            for (var i = 0; i < expected.Count; i++)
            {
                var expectedElement = expected[i];
                var actualElement = actual[i];

                var errors = jsonComparer.Compare(expectedElement, actualElement, JsonPathUtils.Combine(path, $"[{i}]"), options);

                foreach (var error in errors)
                {
                    yield return error;
                }
            }
        }
    }
}
