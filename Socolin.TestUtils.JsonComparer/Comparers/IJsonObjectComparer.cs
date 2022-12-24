using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Comparers;

public interface IJsonObjectComparer
{
    IEnumerable<IJsonCompareError<JToken>> Compare(
        JObject expected,
        JObject actual,
        IJsonComparer? jsonComparer,
        string path = "",
        JsonComparisonOptions? options = null
    );
}
