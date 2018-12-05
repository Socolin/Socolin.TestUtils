using System;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class CaptureExample
    {
        public void Test1()
        {
            Console.WriteLine("==== CaptureExample.Test1 ==== ");

            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) =>
            {
                Console.WriteLine($"Captured value: name={captureName} token={token}");
            }));

            const string expectedJson = @"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}";
            const string actualJson = @"{""a"":42, ""b"":""abc""}";

            var errors = jsonComparer.Compare(expectedJson, actualJson);
            // errors.Count = 0
            Console.WriteLine("No difference found");
        }
    }
}