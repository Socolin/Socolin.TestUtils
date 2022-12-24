using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors;

public class InvalidCaptureObjectCompareError : JsonCompareError
{
    private readonly string _details;

    public InvalidCaptureObjectCompareError(string path, JToken expectedValue, JToken actualValue, string details)
        : base(path, expectedValue, actualValue)
    {
        _details = details;
    }

    public override string Message => $"Invalid capture object: {_details}";
}