using System;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.JsonComparer;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class MatchExample
    {
        public void Test1()
        {
            Console.WriteLine("==== MatchExample.Test1 ==== ");
            const string expectedJson = @"{
                ""a"":{
                    ""__match"":{
                        ""regex"": ""\\d+""
                    }
                },
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":""42"",
                ""b"":""abc""
            }";
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }

        public void Test2()
        {
            Console.WriteLine("==== MatchExample.Test2 ==== ");
            const string expectedJson = @"{
                ""a"":{
                    ""__match"":{
                        ""regex"": ""\\d+""
                    }
                },
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":""abc"",
                ""b"":""abc""
            }";
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }

        public void Test3()
        {
            Console.WriteLine("==== MatchExample.Test3 ==== ");
            var expectedJToken = JToken.Parse(@"{
                ""a"":{
                    ""__match"":{
                        ""regex"": "".+""
                    }
                },
                ""b"":""abc""
            }");
            var actualJToken = JToken.Parse(@"{
                ""a"":""abc"",
                ""b"":""def""
            }");
            var jsonComparer = TestsUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJToken, actualJToken);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
        }
    }
}