using System;
using Socolin.TestsUtils.JsonComparer;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class MatchExample
    {
        public void Test1()
        {
            Console.WriteLine("==== MatchExample.Test1 ==== ");
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            const string expectedJson = @"{""a"":{""__match"":{""regex"": ""\\d+""}}, ""b"":""abc""}";
            const string actualJson = @"{""a"":""42"", ""b"":""abc""}";
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine("No difference found");
            // errors.Count = 0
        }

        public void Test2()
        {
            Console.WriteLine("==== MatchExample.Test2 ==== ");
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            const string expectedJson = @"{""a"":{""__match"":{""regex"": ""\\d+""}}, ""b"":""abc""}";
            const string actualJson = @"{""a"":""abc"", ""b"":""abc""}";
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }
    }
}