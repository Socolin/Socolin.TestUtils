using Newtonsoft.Json.Linq;

namespace Socolin.TestsUtils.JsonComparer.Errors
{
    public class InvalidValueJsonCompareError : JsonCompareError<JValue>
    {
        public InvalidValueJsonCompareError(string path, JValue expectedValue, JValue actualValue)
            : base(path, expectedValue, actualValue)
        {
        }

        public override string Message => $"Invalid value, expected '{ExpectedValue}' but found '{ActualValue}'";
    }
}