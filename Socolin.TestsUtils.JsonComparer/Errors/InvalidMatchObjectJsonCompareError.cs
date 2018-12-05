using Newtonsoft.Json.Linq;

namespace Socolin.TestsUtils.JsonComparer.Errors
{
    public class InvalidMatchObjectJsonCompareError : JsonCompareError
    {
        private readonly string _details;

        public InvalidMatchObjectJsonCompareError(string path, JToken expectedValue, JToken actualValue, string details)
            : base(path, expectedValue, actualValue)
        {
            _details = details;
        }

        public override string Message => $"Invalid capture object: {_details}";
    }
}