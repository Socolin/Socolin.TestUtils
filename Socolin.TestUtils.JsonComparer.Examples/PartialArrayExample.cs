using System;
using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class PartialArrayExample
    {
        public void Test1()
        {
            Console.WriteLine($"==== {nameof(PartialArrayExample)}.{nameof(Test1)} ==== ");

            const string expectedJson = @"{
                ""__partialArray"":{""key"": ""id"", ""array"": [
                    {
                        ""id"": 42,
                        ""name"": ""name1""
                    }
                ]}
            }";
            const string actualJson = @"[
                {
                    ""id"": 42,
                    ""name"": ""name1""
                },
                {
                    ""id"": 44,
                    ""name"": ""name2""
                }
            ]";

            var jsonComparer = JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJson, actualJson);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
        }

        public void Test2()
        {
            Console.WriteLine($"==== {nameof(PartialArrayExample)}.{nameof(Test2)} ==== ");

            const string expectedJson = @"{
                ""field"": {
                    ""__partialArray"":{""key"": ""id"", ""array"": [
                        {
                            ""id"": 30,
                            ""name"": ""name0""
                        },
                        {
                            ""id"": 42,
                            ""name"": ""name2""
                        }
                    ]}
                }
            }";
            const string actualJson = @"{
                ""field"": [
                        {
                            ""id"": 30,
                            ""name"": ""name0""
                        },
                        {
                            ""id"": 42,
                            ""name"": ""name1""
                        },
                        {
                            ""id"": 44,
                            ""name"": ""name2""
                        }
                    ]
                }";

            var actual = JToken.Parse(actualJson);
            var expected = JToken.Parse(expectedJson);
            var jsonComparer = JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expected, actual);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expected, actual, errors, true));
        }

        public void Test3()
        {
            Console.WriteLine($"==== {nameof(PartialArrayExample)}.{nameof(Test3)} ==== ");

            const string expectedJson = @"{
                ""field"": {
                    ""__partialArray"":{""key"": ""id"", ""array"": [
                        {
                            ""id"": 30,
                            ""name"": ""name0""
                        },
                        {
                            ""id"": 42,
                            ""name"": ""name2""
                        }
                    ]}
                }
            }";
            const string actualJson = @"{
                ""field"": [
                        {
                            ""id"": 30,
                            ""name"": ""name0""
                        },
                        {
                            ""id"": 44,
                            ""name"": ""name2""
                        }
                    ]
                }";

            var actual = JToken.Parse(actualJson);
            var expected = JToken.Parse(expectedJson);
            var jsonComparer = JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expected, actual);
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expected, actual, errors, true));
        }
    }
}
