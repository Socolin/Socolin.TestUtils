using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class UnexpectedPropertyJsonComparerError : JsonCompareError<JObject>
    {
        public readonly JProperty UnexpectedProperty;

        public UnexpectedPropertyJsonComparerError(string path, JObject expectedValue, JObject actualValue, JProperty unexpectedProperty)
            : base(path, expectedValue, actualValue)
        {
            UnexpectedProperty = unexpectedProperty;
        }

        public override string Message => $"Unexpected property {UnexpectedProperty.Name}";
    }
}