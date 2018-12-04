using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.JsonComparer.Errors;

namespace Socolin.TestsUtils.JsonComparer.Handlers
{
    public interface IJsonCaptureHandler
    {
        (bool success, IList<JsonCompareError> errors) HandleCapture(JToken expected, JToken actual, string path);
    }

    public class JsonCaptureHandler : IJsonCaptureHandler
    {
        private readonly Action<string, JToken> _captureValueHandler;

        public JsonCaptureHandler(Action<string, JToken> handler)
        {
            _captureValueHandler = handler;
        }

        public (bool success, IList<JsonCompareError> errors) HandleCapture(JToken expected, JToken actual, string path)
        {
            if (!IsCaptureObject(expected))
                return (false, null);

            var jCaptureObject = ((JObject) expected).Value<JObject>("__capture");
            if (!jCaptureObject.ContainsKey("name"))
                return (false, new List<JsonCompareError> {new InvalidCaptureObjectCompareError(path, expected, actual, "Missing `name` field on capture object")});
            if (!jCaptureObject.ContainsKey("type"))
                return (false, new List<JsonCompareError> {new InvalidCaptureObjectCompareError(path, expected, actual, "Missing `type` field on capture object")});

            var captureObject = jCaptureObject.ToObject<JsonCaptureObject>();

            if (!Enum.TryParse(captureObject.Type, true, out JTokenType type))
                return (false, new List<JsonCompareError> {new InvalidCaptureObjectCompareError(path, expected, actual, $"Invalid `type`: value '{captureObject.Type}' is not valid, see JTokenType for list of type")});

            if (type != actual.Type)
                return (false, new List<JsonCompareError> {new InvalidTypeJsonCompareError(path, expected, actual)});

            _captureValueHandler?.Invoke(captureObject.Name, actual);

            if (expected.Parent is JProperty parentProperty)
                parentProperty.Value = actual.DeepClone();

            return (true, null);
        }

        private static bool IsCaptureObject(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
                return false;
            if (!(jToken is JObject expectedJObject))
                return false;
            return expectedJObject.ContainsKey("__capture");
        }
    }
}