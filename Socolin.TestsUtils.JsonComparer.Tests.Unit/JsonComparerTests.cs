using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestsUtils.JsonComparer.Comparers;
using Socolin.TestsUtils.JsonComparer.Errors;
using Socolin.TestsUtils.JsonComparer.Handlers;
using Socolin.TestsUtils.JsonComparer.Tests.Unit.Errors;

namespace Socolin.TestsUtils.JsonComparer.Tests.Unit
{
    public class JsonComparerTests
    {
        private IJsonObjectComparer _jsonObjectComparer;
        private IJsonArrayComparer _jsonArrayComparer;
        private IJsonValueComparer _jsonValueComparer;
        private IJsonSpecialHandler _jsonSpecialHandler;
        private JsonComparer _jsonComparer;

        [SetUp]
        public void SetUp()
        {
            _jsonObjectComparer = Substitute.For<IJsonObjectComparer>();
            _jsonArrayComparer = Substitute.For<IJsonArrayComparer>();
            _jsonValueComparer = Substitute.For<IJsonValueComparer>();
            _jsonSpecialHandler = Substitute.For<IJsonSpecialHandler>();
            _jsonComparer = new JsonComparer(_jsonObjectComparer, _jsonArrayComparer, _jsonValueComparer, _jsonSpecialHandler);

            _jsonSpecialHandler.HandleSpecialObject(Arg.Any<JToken>(), Arg.Any<JToken>(), Arg.Any<string>())
                .Returns((false, null));
        }

        [Test]
        public void WhenComparingTwoEmptyJsonObject_ReturnNoError()
        {
            var actualErrors = _jsonComparer.Compare("{}", "{}");

            actualErrors.Should().BeEmpty();
        }

        [Test]
        [TestCase("{}", @"""abc""")]
        [TestCase("{}", @"42")]
        [TestCase("{}", @"42.5")]
        [TestCase("{}", @"[]")]
        [TestCase("{}", @"null")]
        [TestCase("{}", @"true")]
        public void WhenComparingTwoJsonOfDifferentType_ReturnError(string expectedJson, string actualJson)
        {
            var actualErrors = _jsonComparer.Compare(expectedJson, actualJson);

            using (new AssertionScope())
            {
                actualErrors.Should().NotBeEmpty();
                actualErrors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenComparingObjects_UseJsonObjectComparer()
        {
            var expectedJson = JObject.Parse("{}");
            var actualJson = JObject.Parse("{}");

            _jsonObjectComparer.Compare(expectedJson, actualJson, _jsonComparer)
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var actualErrors = _jsonComparer.Compare(expectedJson, actualJson);

            using (new AssertionScope())
            {
                actualErrors.Should().NotBeNullOrEmpty();
                actualErrors.First().Should().BeOfType<TestJsonCompareError>();
            }
        }

        [Test]
        public void WhenComparingArrays_UseJsonArrayComparer()
        {
            var expectedJson = JArray.Parse("[]");
            var actualJson = JArray.Parse("[]");

            _jsonArrayComparer.Compare(expectedJson, actualJson, _jsonComparer)
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var actualErrors = _jsonComparer.Compare(expectedJson, actualJson);

            using (new AssertionScope())
            {
                actualErrors.Should().NotBeNullOrEmpty();
                actualErrors.First().Should().BeOfType<TestJsonCompareError>();
            }
        }

        private static readonly TestCaseData[] JValues =
        {
            new TestCaseData(new JValue(42)).SetArgDisplayNames("int"),
            new TestCaseData(new JValue(42.5f)).SetArgDisplayNames("float"),
            new TestCaseData(new JValue(42.5d)).SetArgDisplayNames("double"),
            new TestCaseData(new JValue("abc")).SetArgDisplayNames("string"),
            new TestCaseData(new JValue(true)).SetArgDisplayNames("bool"),
            new TestCaseData(JValue.CreateNull()).SetArgDisplayNames("null"),
            new TestCaseData(JValue.CreateUndefined()).SetArgDisplayNames("undef"),
        };

        [TestCaseSource(nameof(JValues))]
        public void WhenComparingJsonValue_UseJsonValueComparer(JValue jValue)
        {
            _jsonValueComparer.Compare(jValue, jValue)
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var actualErrors = _jsonComparer.Compare(jValue, jValue);

            using (new AssertionScope())
            {
                actualErrors.Should().NotBeNullOrEmpty();
                actualErrors.First().Should().BeOfType<TestJsonCompareError>();
            }
        }

        [Test]
        public void WhenComparingJsonValue_AndTypeMismatch_AndCaptureSucceed_DoNotReturnsError()
        {
            var expectedJson = JObject.Parse("{}");
            var actualJson = JToken.Parse("42");

            _jsonSpecialHandler.HandleSpecialObject(expectedJson, actualJson, "")
                .Returns((true, null));

            var actualErrors = _jsonComparer.Compare(expectedJson, actualJson);

            actualErrors.Should().BeEmpty();
        }

        [Test]
        public void WhenComparingJsonValue_AndTypeMismatch_AndCaptureFail_ReturnCaptureErrors()
        {
            var expectedJson = JObject.Parse("{}");
            var actualJson = JToken.Parse("42");

            _jsonSpecialHandler.HandleSpecialObject(expectedJson, actualJson, "")
                .Returns((false, new List<JsonCompareError> {new TestJsonCompareError()}));

            var actualErrors = _jsonComparer.Compare(expectedJson, actualJson);

            using (new AssertionScope())
            {
                actualErrors.Should().HaveCount(1);
                actualErrors.First().Should().BeOfType<TestJsonCompareError>();
            }
        }
    }
}