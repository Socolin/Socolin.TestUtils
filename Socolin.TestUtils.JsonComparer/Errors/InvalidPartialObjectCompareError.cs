using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class InvalidPartialObjectCompareError : JsonCompareError
    {
        private readonly string _details;

        public InvalidPartialObjectCompareError(string path, JToken expectedValue, JToken actualValue, string details)
            : base(path, expectedValue, actualValue)
        {
            _details = details;
        }

        public override string Message => $"Invalid partial object: {_details}";
    }
}
