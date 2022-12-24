using Newtonsoft.Json;
using Socolin.TestUtils.JsonComparer.Color;

namespace Socolin.TestUtils.JsonComparer.Utils;

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
    private readonly JsonComparerColorOptions _colorOptions;


    public JsonDeserializerWithNiceError()
        : this(JsonComparerColorOptions.Default)
    {
    }

    public JsonDeserializerWithNiceError(JsonComparerColorOptions colorOptions)
    {
        _colorOptions = colorOptions;
    }

    public T Deserialize<T>(string json, JsonSerializerSettings settings)
    {
        return JsonDeserializerErrorFormatterHelper.DeserializeWithNiceErrorMessage<T>(json, settings, _colorOptions);
    }
}