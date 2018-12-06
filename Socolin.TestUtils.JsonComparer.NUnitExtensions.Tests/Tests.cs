using Newtonsoft.Json.Linq;
using NUnit.Framework;

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

            Assert.That(actualJson, Is.JsonEquivalent(expectedJson));
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

            Assert.That(actualJson, Is.Not.JsonEquivalent(expectedJson));
        }

        [Test]
        public void TestAssertThatWithJsonComparer()
        {
            var expectedJson = JObject.FromObject(new {parent = new {__capture = new {name = "some-capture-name", type = "integer"}}});
            var actualJson = JObject.FromObject(new {parent = 42});

            var comparerCalled = false;
            var jsonComparer = JsonComparer.GetDefault((s, token) => { comparerCalled = true; });

            Assert.That(actualJson, Is.JsonEquivalent(expectedJson).WithComparer(jsonComparer));
            Assert.That(comparerCalled, Is.True);
        }
    }
}