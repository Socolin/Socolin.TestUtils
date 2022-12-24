using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Examples;

public class CaptureExample
{
    public void Test1()
    {
        Console.WriteLine("==== CaptureExample.Test1 ==== ");

        var jsonComparer = JsonComparer.GetDefault(((captureName, token) => {
            Console.WriteLine($"Captured value: name={captureName} token={token}");
        }));

        const string expectedJson = @"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}";
        const string actualJson = @"{""a"":42, ""b"":""abc""}";

        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }

    public void Test2()
    {
        Console.WriteLine("==== CaptureExample.Test2 ==== ");

        var jsonComparer = JsonComparer.GetDefault(((captureName, token) => {
            Console.WriteLine($"Captured value: name={captureName} token={token}");
        }));

        var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}");
        var actualJson = JToken.Parse(@"{""a"":42, ""b"":""def""}");

        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }

    public void Test3()
    {
        Console.WriteLine("==== CaptureExample.Test3 ==== ");

        var jsonComparer = JsonComparer.GetDefault(((captureName, token) => {
            Console.WriteLine($"Captured value: name={captureName} token={token}");
        }));

        var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-global-capture-name"", ""regex"":""(?<localCapture>bcd)""}}, ""b"":""def""}");
        var actualJson = JToken.Parse(@"{""a"":""abcdef"", ""b"":""def""}");

        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }

    public void Test4()
    {
        Console.WriteLine("==== CaptureExample.Test4 ==== ");

        var jsonComparer = JsonComparer.GetDefault(((captureName, token) => {
            Console.WriteLine($"Captured value: name={captureName} token={token}");
        }));


        var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-global-capture-name"", ""regex"":""^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$""}}}");
        var actualJson = JToken.Parse(@"{""a"":""B6E73AF8-BDB9-41B2-BB77-28575B08A28C""}");

        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }


    public void Test5()
    {
        Console.WriteLine("==== CaptureExample.Test5 ==== ");

        var jsonComparer = JsonComparer.GetDefault(((captureName, token) => {
            Console.WriteLine($"Captured value: name={captureName} token={token}");
        }));

        var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""regex"":""(?<localCapture>bcd)""}}}");
        var actualJson = JToken.Parse(@"{""a"":""abcdef""}");

        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }

}
