using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Handlers;

namespace Socolin.TestUtils.JsonComparer.Tests.Unit.Handlers
{
    public class JsonCaptureHandlerTests
    {
        private JsonSpecialHandler _jsonSpecialHandler;
        private Action<string, JToken> _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = Substitute.For<Action<string, JToken>>();
            _jsonSpecialHandler = new JsonSpecialHandler(_handler);
        }

        [Test]
        public void WhenHandlingSpecial_AndExpectedIsNotACaptureObject_ReturnFalse()
        {
            var expectedJson = JToken.Parse(@"{""some-key"":""some-value""}");
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(expectedJson, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().BeNull();
            }
        }

        [Test]
        public void WhenHandlingCapture_AndCaptureObjectIsMissingNameField_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__capture = new {type = "string"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidCaptureObjectCompareError>();
            }
        }

        [Test]
        public void WhenHandlingCapture_AndCaptureObjectIsMissingTypeField_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-name"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidCaptureObjectCompareError>();
            }
        }

        [Test]
        public void WhenHandlingCapture_AndTypeMismatch_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-capture-name", type = "string"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingCapture_CallAddMethod()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-capture-name", type = "integer"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                _handler.Received(1)
                    .Invoke("some-capture-name", actualJson);
                success.Should().BeTrue();
                errors.Should().BeNull();
            }
        }

        [Test]
        public void WhenHandlingCapture_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__capture = new {name = "some-capture-name", type = "integer"}}});
            var actualJson = JObject.FromObject(new {parent = 42});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent");

            captureObject.Property("parent").Value.ToObject<int>().Should().Be(42);
        }

        [Test]
        public void WhenHandlingMatch_AndNoParameterGiven_ReturnErrors()
        {
            var captureObject = JObject.FromObject(new {__match = new {}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidMatchObjectJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexIsGiven_AndActualValueIsNotAString_ReturnError()
        {
            var captureObject = JObject.FromObject(new {__match = new {regex = "some-regex"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexMismatch_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__match = new {regex = "invalid-regex"}});
            var actualJson = JToken.Parse(@"""some-string""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<RegexMismatchMatchJsonCompareError>();
            }
        }


        [Test]
        public void WhenHandlingMatch_AndRegexMatches_ReturnsSuccess()
        {
            var captureObject = JObject.FromObject(new {__match = new {regex = "some-string"}});
            var actualJson = JToken.Parse(@"""some-string""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "");

            using (new AssertionScope())
            {
                success.Should().BeTrue();
            }
        }

        [Test]
        public void WhenHandlingMatch_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__match = new {regex = ".+"}}});
            var actualJson = JObject.FromObject(new {parent = "abc"});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent");

            captureObject.Property("parent").Value.ToObject<string>().Should().Be("abc");
        }
    }
}