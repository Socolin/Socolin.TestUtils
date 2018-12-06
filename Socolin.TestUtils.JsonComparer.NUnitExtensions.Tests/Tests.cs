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
    }
}