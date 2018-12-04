using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestsUtils.JsonComparer.Errors;
using Socolin.TestsUtils.JsonComparer.Handlers;

namespace Socolin.TestsUtils.JsonComparer.Tests.Unit.Handlers
{
    public class JsonCaptureHandlerTests
    {
        private JsonCaptureHandler _jsonCaptureHandler;
        private Action<string, JToken> _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = Substitute.For<Action<string, JToken>>();
            _jsonCaptureHandler = new JsonCaptureHandler(_handler);
        }


        [Test]
        public void WhenHandlingCapture_AndExpectedIsNotACaptureObject_ReturnFalse()
        {
            var expectedJson = JToken.Parse(@"{""some-key"":""some-value""}");
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonCaptureHandler.HandleCapture(expectedJson, actualJson, "");

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

            var (success, errors) = _jsonCaptureHandler.HandleCapture(captureObject, actualJson, "");

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

            var (success, errors) = _jsonCaptureHandler.HandleCapture(captureObject, actualJson, "");

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

            var (success, errors) = _jsonCaptureHandler.HandleCapture(captureObject, actualJson, "");

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

            var (success, errors) = _jsonCaptureHandler.HandleCapture(captureObject, actualJson, "");

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

            _jsonCaptureHandler.HandleCapture(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent");

            captureObject.Property("parent").Value.ToObject<int>().Should().Be(42);
        }
    }
}