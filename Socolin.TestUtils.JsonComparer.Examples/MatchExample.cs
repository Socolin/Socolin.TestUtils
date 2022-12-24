using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Color;

namespace Socolin.TestUtils.JsonComparer.Examples;

public class MatchExample
{
    public void Test1()
    {
        Console.WriteLine("==== MatchExample.Test1 ==== ");
        const string expectedJson = """
            {
                "a":{
                    "__match":{
                        "regex": "\\d+"
                    }
                },
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a":"42",
                "b":"abc"
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test2()
    {
        Console.WriteLine("==== MatchExample.Test2 ==== ");
        const string expectedJson = """
            {
                "a":{
                    "__match":{
                        "regex": "\\d+"
                    }
                },
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a":"abc",
                "b":"abc"
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test3()
    {
        Console.WriteLine("==== MatchExample.Test3 ==== ");
        var expectedJToken = JToken.Parse("""
            {
                "a":{
                    "__match":{
                        "regex": ".+"
                    }
                },
                "b":"abc"
            }
            """);
        var actualJToken = JToken.Parse("""
            {
                "a":"abc",
                "b":"def"
            }
            """);
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJToken, actualJToken);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test4()
    {
        Console.WriteLine("==== MatchExample.Test4 ==== ");
        const string expectedJson = """
            {
                "a":{
                    "__match":{
                        "type": "integer"
                    }
                },
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a":42,
                "b":"abc"
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test5()
    {
        Console.WriteLine("==== MatchExample.Test5 ==== ");
        const string expectedJson = """
            {
                "date":{
                    "__match":{
                        "regex": "^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{7}Z?"
                    }
                }
            }
            """;
        const string actualJson = """
            {
                "date": "2042-05-04T06:01:06.0000000Z"
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test6()
    {
        Console.WriteLine("==== MatchExample.Test6 ==== ");
        const string expectedJson = """
            {
                "a":{
                    "__match":{
                        "range": [95, 105]
                    }
                },
                "b": {
                    "__match":{
                        "range": [95, 105]
                    }
                }
            }
            """;
        const string actualJson = """
            {
                "a": 95,
                "b": 105
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var expectedJToken = JToken.Parse(expectedJson);
        var actualJToken = JToken.Parse(actualJson);
        var errors = jsonComparer.Compare(expectedJToken, actualJToken);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test7()
    {
        Console.WriteLine("==== MatchExample.Test7 ==== ");
        var expectedJToken = JToken.Parse("""
            {
                "a":{
                    "__match":{
                        "value": "Hello\nWorld",
                        "ignoredCharacters": "\r"
                    }
                }
            }
            """);
        var actualJToken = JToken.Parse("""
            {
                "a":"Hello\r\nWorld"
            }
            """);
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJToken, actualJToken);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors, JsonComparerColorOptions.DefaultColored));
    }

    public void Test8()
    {
        Console.WriteLine("==== MatchExample.Test8 ==== ");
        var expectedJToken = JToken.Parse("""
            {
                "a":{
                    "__match":{
                        "value": "Hello\nWorld",
                        "ignoredCharacters": "\n"
                    }
                }
            }
            """);
        var actualJToken = JToken.Parse("""
            {
                "a":"Hello\r\nWorld"
            }
            """);
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJToken, actualJToken);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors, JsonComparerColorOptions.DefaultColored));
    }
}
