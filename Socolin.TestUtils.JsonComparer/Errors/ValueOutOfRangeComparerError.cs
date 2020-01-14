using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class ValueOutOfRangeComparerError : JsonCompareError
    {
        private readonly decimal[] _range;

        public ValueOutOfRangeComparerError(string path, JToken expectedValue, JToken actualValue, decimal[] range) : base(path, expectedValue, actualValue)
        {
            _range = range;
        }

        public override string Message => $"Value {ActualValue} is out of range [{_range[0]}, {_range[1]}]";
    }
}
