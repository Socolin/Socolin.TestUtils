using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Tests.Unit.Errors
{
    internal class TestJsonCompareError : JsonCompareError
    {
        public TestJsonCompareError(string path = null, JToken actualValue = null, JToken expectedValue = null)
            : base(path, expectedValue, actualValue)
        {
        }

        public override string Message => "some-message";
    }
}