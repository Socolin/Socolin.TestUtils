using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors;

public class MissingValueInArrayComparerError : JsonCompareError
{
    private readonly object _value;

    public MissingValueInArrayComparerError(string path, JToken expectedValue, object value)
        : base(path, expectedValue, null)
    {
        _value = value;
    }

    public override string Message => $"Missing value {_value} in array";
}
