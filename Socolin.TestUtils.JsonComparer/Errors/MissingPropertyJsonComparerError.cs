using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class MissingPropertyJsonComparerError : JsonCompareError<JObject>
    {
        public readonly JProperty MissingProperty;

        public MissingPropertyJsonComparerError(string path, JObject expectedValue, JObject actualValue, JProperty missingProperty)
            : base(path, expectedValue, actualValue)
        {
            MissingProperty = missingProperty;
        }

        public override string Message => $"Missing property `{MissingProperty.Name}`";
    }
}
