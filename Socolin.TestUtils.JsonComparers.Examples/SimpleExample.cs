using System;
using Socolin.TestsUtils.JsonComparer;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class SimpleExample
    {
        public void Test1()
        {
            Console.WriteLine("==== SimpleExample.Test1 ==== ");
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            const string expectedJson = @"{""a"":1, ""b"":""abc""}";
            const string actualJson = @"{""a"":42, ""b"":""abc""}";
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }
    }
}