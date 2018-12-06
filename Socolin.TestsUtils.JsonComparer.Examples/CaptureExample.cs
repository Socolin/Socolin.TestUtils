using System;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.JsonComparer;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class CaptureExample
    {
        public void Test1()
        {
            Console.WriteLine("==== CaptureExample.Test1 ==== ");

            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
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

            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
                Console.WriteLine($"Captured value: name={captureName} token={token}");
            }));

            var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}");
            var actualJson = JToken.Parse(@"{""a"":42, ""b"":""def""}");

            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }
    }
}