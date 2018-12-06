using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors
{
    public class InvalidTypeJsonCompareError : JsonCompareError
    {
        public InvalidTypeJsonCompareError(string path, JToken expectedValue, JToken actualValue)
            : base(path, expectedValue, actualValue)
        {
        }

        private static bool IsRawJsonType(JTokenType expectedValueType)
        {
            return expectedValueType == JTokenType.Integer
                   || expectedValueType == JTokenType.String
                   || expectedValueType == JTokenType.Float
                   || expectedValueType == JTokenType.Guid
                   || expectedValueType == JTokenType.Boolean;
        }

        private static string FormatTypeValue(JToken jToken) => $"`{jToken.Type}`" + (IsRawJsonType(jToken.Type) ? $" ({jToken})" : string.Empty);
        public override string Message => $"Invalid type: Expected type was {FormatTypeValue(ExpectedValue)} but it was of type {FormatTypeValue(ActualValue)}";
    }
}