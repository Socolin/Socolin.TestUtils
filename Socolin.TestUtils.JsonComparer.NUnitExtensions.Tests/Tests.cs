using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.ANSITerminalColor;
using Socolin.TestUtils.JsonComparer.Color;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestAssertThat()
        {
            const string expectedJson = @"{
                ""a"":1,
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc""
            }";

            Assert.That(actualJson, IsJson.EquivalentTo(expectedJson).WithColoredOutput());
        }

        [Test]
        public void TestColorizeJson()
        {
            const string expectedJson = @"{
                ""a"":1,
                ""b"":""abc"",
                ""c"":""hello"",
                ""d"": true,
                ""e"": 64,
                ""f"": 42.424242,
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc"",
                ""c"":""hello"",
                ""d"": true,
                ""e"": 64,
                ""f"": 42.424242,
            }";

            Assert.That(actualJson,
                IsJson.EquivalentTo(expectedJson).WithColorOptions(new JsonComparerColorOptions
                {
                    ColorizeDiff = true,
                    ColorizeJson = true,
                    Theme = new JsonComparerColorTheme
                    {
                        DiffAddition = AnsiColor.Background(TerminalRgbColor.FromHex("21541A")),
                        DiffDeletion = AnsiColor.Background(TerminalRgbColor.FromHex("542822")),
                    }
                }));
        }


        [Test]
        public void TestAssertThatNotSuccess()
        {
            const string expectedJson = @"{
                ""a"":1,
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc""
            }";

            Assert.That(actualJson, IsJson.EquivalentTo(expectedJson).WithColoredOutput());
        }

        [Test]
        public void TestAssertThatMatchDateWithoutParsingThem()
        {
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

            Assert.That(actualJson, IsJson.EquivalentTo(expectedJson).WithColoredOutput());
        }

        [Test]
        public void TestAssertThatWithJsonComparer()
        {
            var expectedJson = JObject.FromObject(new {parent = new {__capture = new {name = "some-capture-name", type = "integer"}}});
            var actualJson = JObject.FromObject(new {parent = 42});

            var comparerCalled = false;
            var jsonComparer = JsonComparer.GetDefault((s, token) => { comparerCalled = true; });

            Assert.That(actualJson, IsJson.EquivalentTo(expectedJson).WithComparer(jsonComparer));
            Assert.That(comparerCalled, Is.True);
        }


        [Test]
        public void TestAssertThatWithOptions()
        {
            const string expectedJson = @"{
                ""a"":1,
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc""
            }";

            Assert.That(actualJson,
                IsJson.EquivalentTo(expectedJson).WithOptions(new JsonComparisonOptions
                {
                    IgnoreFields = (fieldPath, fieldName) => fieldName == "a"
                }));
        }

        [Test]
        public void TestInvalidJsonColor()
        {
            const string expectedJson = @"{
                ""a"":1
                ""b"":""abc""
            }";
            const string actualJson = @"{
                ""a"":42,
                ""b"":""abc""
            }";

            Assert.That(actualJson, IsJson.EquivalentTo(expectedJson).WithColoredOutput());
        }
    }
}
