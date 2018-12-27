using System;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer;

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
            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
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
            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
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
            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJToken, actualJToken);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
        }

        public void Test4()
        {
            Console.WriteLine("==== MatchExample.Test4 ==== ");
            const string expectedJson = @"{
                ""a"":{
                    ""__match"":{
                        ""type"": ""integer""
                    }
                },
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc""
            }";
            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }

        public void Test5()
        {
            Console.WriteLine("==== MatchExample.Test5 ==== ");
            const string expectedJson = @"{
                ""date"":{
                    ""__match"":{
                        ""regex"": ""^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{7}Z?""
                    }
                }
            }";
            const string actualJson = @"{
    			""date"": ""2042-05-04T06:01:06.0000000Z""
            }";
            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }
    }
}