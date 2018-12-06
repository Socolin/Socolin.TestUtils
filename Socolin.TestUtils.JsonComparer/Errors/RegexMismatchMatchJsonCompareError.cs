using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class RegexMismatchMatchJsonCompareError : JsonCompareError
    {
        private readonly string _regex;

        public RegexMismatchMatchJsonCompareError(string path, JToken expectedValue, JToken actualValue, string regex)
            : base(path, expectedValue, actualValue)
        {
            _regex = regex;
        }

        public override string Message => $"Invalid value, '{ActualValue}' should match regex '{_regex}'";
    }
}