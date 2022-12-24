using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors;

public class MissingObjectInArrayComparerError: JsonCompareError
{
    private readonly JValue? _key;

    public MissingObjectInArrayComparerError(string path, JToken expectedValue, JValue? key)
        : base(path, expectedValue, null)
    {
        _key = key;
    }

    public override string Message => $"Missing element identified by the key {_key}";
}
