using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Errors;

public interface IJsonCompareError<out TToken> where TToken : JToken
{
    string? Path { get; }
    TToken? ActualValue { get; }
    TToken? ExpectedValue { get; }
    string Message { get; }
}

[PublicAPI]
public abstract class JsonCompareError<TToken> : IJsonCompareError<TToken> where TToken : JToken
{
    public string? Path { get; }
    public TToken? ActualValue { get; }
    public TToken? ExpectedValue { get; }

    protected JsonCompareError(string? path, TToken? expectedValue, TToken? actualValue)
    {
        Path = path;
        ActualValue = actualValue;
        ExpectedValue = expectedValue;
    }

    public abstract string Message { get; }
}

public abstract class JsonCompareError : JsonCompareError<JToken>
{
    protected JsonCompareError(string? path, JToken? expectedValue, JToken? actualValue)
        : base(path, expectedValue, actualValue)
    {
    }
}
