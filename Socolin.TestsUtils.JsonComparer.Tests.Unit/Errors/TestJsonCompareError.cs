using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.JsonComparer.Errors;

namespace Socolin.TestsUtils.JsonComparer.Tests.Unit.Errors
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