using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Examples;

public class PartialExample
{
    public void Test1()
    {
        Console.WriteLine($"==== {nameof(PartialExample)}.{nameof(Test1)} ==== ");

        const string expectedJson = """
            {
                "a":{
                    "__partial":{
                        "tested": "123"
                    }
                },
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a": {
                    "tested": "123",
                    "ignored": "42"
                },
                "b":"abc"
            }
            """;

        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
    }

    public void Test2()
    {
        Console.WriteLine($"==== {nameof(PartialExample)}.{nameof(Test2)} ==== ");

        const string expectedJson = """
            {
                "a":{
                    "__partial":{
                        "tested": "123",
                        "missing": "123"
                    }
                },
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a": {
                    "tested": "123",
                    "ignored": "42"
                },
                "b":"abc"
            }
            """;

        var expectedJToken = JToken.Parse(expectedJson);
        var actualJToken = JToken.Parse(actualJson);

        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJToken, actualJToken);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
    }
}
