using Socolin.TestUtils.JsonComparer.Exceptions;

namespace Socolin.TestUtils.JsonComparer.Examples;

public class InvalidJsonExample
{
    public void Test1()
    {
        Console.WriteLine("==== InvalidJsonExample.Test1 ==== ");
        const string expectedJson = """
            {
                "a":{
                    "b": "some-test-1",
                    "d": "some-test-2"
                    "c": "some-test-3"
                }
            }
            """;

        var jsonComparer = JsonComparer.GetDefault();
        try
        {
            jsonComparer.Compare(expectedJson, "{}");
        }
        catch (InvalidJsonException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
