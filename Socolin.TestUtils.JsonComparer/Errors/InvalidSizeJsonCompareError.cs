using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class InvalidSizeJsonCompareError : JsonCompareError<JArray>
    {
        public InvalidSizeJsonCompareError(string path, JArray expectedValue, JArray actualValue)
            : base(path, expectedValue, actualValue)
        {
        }

        public override string Message => $"Invalid array size, expected {ExpectedValue.Count} elements, but found {ActualValue.Count}";
    }
}