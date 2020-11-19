using Newtonsoft.Json;

namespace Socolin.TestUtils.JsonComparer.Utils
{
    public interface IJsonDeserializer
    {
        T Deserialize<T>(string json, JsonSerializerSettings settings);
    }

    public class JsonDeserializer : IJsonDeserializer
    {
        public T Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }

    public class JsonDeserializerWithNiceError : IJsonDeserializer
    {
        private readonly bool _useColor;


        public JsonDeserializerWithNiceError()
            : this(false)
        {
        }

        public JsonDeserializerWithNiceError(bool useColor)
        {
            _useColor = useColor;
        }

        public T Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            return JsonDeserializerErrorFormatterHelper.DeserializeWithNiceErrorMessage<T>(json, settings, _useColor);
        }
    }
}
