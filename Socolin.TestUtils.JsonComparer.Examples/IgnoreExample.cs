using System;
using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.Examples
{
    public class IgnoreExample
    {
        public void Test1()
        {
            Console.WriteLine("==== IgnoreExample.Test1 ==== ");
            const string expectedJson = @"{
                ""a"":{
                    ""b"": ""ignore-me-12"",
                    ""notInActual"": ""ignore-me-12"",
                    ""c"": ""compare-me"",
                },
                ""d"": ""compare-me""
            }";
            const string actualJson = @"{
                ""a"":{
                    ""b"": ""ignore-me-45"",
                    ""notInExpected"": ""ignore-me-45"",
                    ""c"": ""compare-me"",
                },
                ""d"": ""compare-me""
            }";

            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
            var expectedJToken = JToken.Parse(expectedJson);
            var actualJToken = JToken.Parse(actualJson);
            var errors = jsonComparer.Compare(expectedJToken, actualJToken, new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => (fieldPath == "a" && fieldName == "b")
                                                         || fieldName == "notInExpected"
                                                         || fieldName == "notInActual"
            });
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
        }


        public void Test2()
        {
            Console.WriteLine("==== IgnoreExample.Test2 ==== ");
            const string expectedJson = @"{
                ""a"":{
                    ""b"": ""ignore-me-12"",
                    ""notInActual"": ""ignore-me-12"",
                    ""c"": ""compare-me"",
                },
                ""d"": ""compare-me""
            }";
            const string actualJson = @"{
                ""a"":{
                    ""b"": ""ignore-me-45"",
                    ""notInExpected"": ""ignore-me-45"",
                    ""c"": ""compare-me"",
                },
                ""d"": ""compare-me""
            }";

            var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
            var expectedJToken = JToken.Parse(expectedJson);
            var actualJToken = JToken.Parse(actualJson);
            var errors = jsonComparer.Compare(expectedJToken, actualJToken, new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "notInExpected"
                                                         || fieldName == "notInActual"
            });
            Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
        }
    }
}
