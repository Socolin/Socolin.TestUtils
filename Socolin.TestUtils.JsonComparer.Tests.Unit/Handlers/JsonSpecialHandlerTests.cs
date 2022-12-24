using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Exceptions;
using Socolin.TestUtils.JsonComparer.Handlers;
using Socolin.TestUtils.JsonComparer.Tests.Unit.Errors;

namespace Socolin.TestUtils.JsonComparer.Tests.Unit.Handlers
{
    public class JsonSpecialHandlerTests
    {
        private JsonSpecialHandler _jsonSpecialHandler;
        private Action<string, JToken> _handler;
        private IJsonObjectComparer _partialComparer;
        private IPartialArrayHandler _partialArrayHandler;
        private RegexAliasesContainer _regexAliasesContainer;

        [SetUp]
        public void SetUp()
        {
            _handler = Substitute.For<Action<string, JToken>>();
            _partialComparer = Substitute.For<IJsonObjectComparer>();
            _partialArrayHandler = Substitute.For<IPartialArrayHandler>();
            _regexAliasesContainer = new RegexAliasesContainer();

            _jsonSpecialHandler = new JsonSpecialHandler(_handler, _partialComparer, _partialArrayHandler, _regexAliasesContainer);
        }

        [Test]
        public void WhenHandlingSpecial_AndExpectedIsNotACaptureObject_ReturnFalse()
        {
            var expectedJson = JToken.Parse(@"{""some-key"":""some-value""}");
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(expectedJson, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().BeNull();
            }
        }

        [Test]
        public void WhenHandlingCapture_AndCaptureObjectUseTypeAndFieldNameIsMissing_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__capture = new {type = "string"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidCaptureObjectCompareError>();
            }
        }

        [Test]
        public void WhenHandlingCapture_AndCaptureObjectIsMissingTypeOrRegexField_ReturnsError()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-name"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent", null, new JsonComparisonOptions());

            captureObject.Property("parent").Value.ToObject<int>().Should().Be(42);
        }

        [Test]
        public void WhenHandlingCapture_UsingRegex_ReturnsErrorIfRegexDoesNotMatch()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-capture-name", regex = "some-regex"}});
            var actualJson = JToken.Parse(@"""abc""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<RegexMismatchMatchJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingCapture_UsingRegex_CallAddMethod()
        {
            var captureObject = JObject.FromObject(new {__capture = new {name = "some-capture-name", regex = "abc"}});
            var actualJson = JToken.Parse(@"""abc""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());


            using (new AssertionScope())
            {
                success.Should().BeTrue();
                errors.Should().BeNull();
            }

            _handler.Received(1)
                .Invoke("some-capture-name", Arg.Is<JValue>(value => value.Value<string>() == "abc"));
        }

        [Test]
        public void WhenHandlingCapture_UsingRegex_UseRegexCaptureGroup_AndCallAddMethodWithGroupName()
        {
            var captureObject = JObject.FromObject(new {__capture = new {regex = "(?<someCaptureName>abc)"}});
            var actualJson = JToken.Parse(@"""abc""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());


            using (new AssertionScope())
            {
                success.Should().BeTrue();
                errors.Should().BeNull();
            }

            _handler.Received(1)
                .Invoke("someCaptureName", actualJson);
        }

        [Test]
        public void WhenHandlingCapture_UsingRegex_UseRegexCaptureGroup_DoNotCallAddWithGroupIndex()
        {
            var captureObject = JObject.FromObject(new {__capture = new {regex = "(?<someCaptureName>abc)"}});
            var actualJson = JToken.Parse(@"""abc""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());


            using (new AssertionScope())
            {
                success.Should().BeTrue();
                errors.Should().BeNull();
            }

            _handler.DidNotReceive().Invoke("0", actualJson);
        }
        [Test]
        public void WhenHandlingCapture_UsingRegex_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__capture = new {name = "some-capture-name", regex = "some-regex"}}});
            var actualJson = JObject.FromObject(new {parent = "some-regex"});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent", null, new JsonComparisonOptions());

            captureObject.Property("parent").Value.ToObject<string>().Should().Be("some-regex");
        }

        [Test]
        public void WhenHandlingMatch_AndNoParameterGiven_ReturnErrors()
        {
            var captureObject = JObject.FromObject(new {__match = new { }});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

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

            var (success, _) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeTrue();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexAliasMatch_ReturnsSuccess()
        {
            _regexAliasesContainer.AddAlias("some-alias", "some-string");

            var captureObject = JObject.FromObject(new {__match = new {regexAlias = "some-alias"}});
            var actualJson = JToken.Parse(@"""some-string""");

            var (success, _) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeTrue();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexAliasMatch_AndAliasDoesNotExists_Throws()
        {
            var captureObject = JObject.FromObject(new {__match = new {regexAlias = "some-non-registered-alias"}});
            var actualJson = JToken.Parse(@"""some-string""");

            var act = () => _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            act.Should().Throw<RegexAliasNotRegisteredException>();
        }

        [Test]
        public void WhenHandlingMatch_AndRegexAliasMatch_AndActualValueIsNotAString_ReturnError()
        {
            _regexAliasesContainer.AddAlias("some-alias", "some-regex");

            var captureObject = JObject.FromObject(new {__match = new {regexAlias = "some-alias"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexAliasMatch_ReturnsError()
        {
            _regexAliasesContainer.AddAlias("some-alias", "some-regex");
            var captureObject = JObject.FromObject(new {__match = new {regexAlias = "some-alias"}});

            var actualJson = JToken.Parse(@"""some-string""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<RegexMismatchMatchJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__match = new {regex = ".+"}}});
            var actualJson = JObject.FromObject(new {parent = "abc"});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent", null, new JsonComparisonOptions());

            captureObject.Property("parent").Value.ToObject<string>().Should().Be("abc");
        }

        [Test]
        public void WhenHandlingMatch_AndTypeIsGiven_SuccessIfTypeIsEqual()
        {
            var captureObject = JObject.FromObject(new {__match = new {type = "integer"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeTrue();
                errors.Should().BeNull();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndTypeIsGiven_ErrorIfTypeIsDifferent()
        {
            var captureObject = JObject.FromObject(new {__match = new {type = "integer"}});
            var actualJson = JToken.Parse(@"""some-string""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndTypeIsGiven_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__match = new {type = "string"}}});
            var actualJson = JObject.FromObject(new {parent = "abc"});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent", null, new JsonComparisonOptions());

            captureObject.Property("parent").Value.ToObject<string>().Should().Be("abc");
        }

        [Test]
        public void WhenHandlingPartial_CompareChildObjectWithPartialJsonObjectComparer()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partial = new {tested = "123"}});
            var actualJson = JObject.FromObject(new {tested = "123", ignored = "456"});

            _partialComparer.Compare(partialObject.Value<JObject>("__partial"), actualJson, jsonComparer)
                .Returns(new List<IJsonCompareError<JToken>>());

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeTrue();
                errors.Should().BeNullOrEmpty();
            }
        }

        [Test]
        public void WhenHandlingPartial_ReturnsErrorFromPartialComparer()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partial = new {tested = "123"}});
            var actualJson = JObject.FromObject(new {tested = "123", ignored = "456"});
            var expectedErrors = new List<IJsonCompareError<JToken>> {new TestJsonCompareError()};
            var jsonComparisonOptions = new JsonComparisonOptions();

            _partialComparer.Compare(partialObject.Value<JObject>("__partial"), actualJson, jsonComparer, "", jsonComparisonOptions)
                .Returns(expectedErrors);

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(partialObject, actualJson, "", jsonComparer, jsonComparisonOptions);

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().BeEquivalentTo(expectedErrors);
            }
        }

        [Test]
        public void WhenHandlingPartial_ReturnsErrorForArrays()
        {
            // TODO: This may be handle some day, when needed
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partial = new[] {1, 2, 3}});
            var actualJson = JArray.FromObject(new[] {1, 2, 3, 4, 5});

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Count.Should().Be(1);
                errors.First().Should().BeOfType<InvalidPartialObjectCompareError>();
            }
        }

        [Test]
        public void WhenHandlingPartial_AndActual_HasNotTheSameTypeAsExpected_ReturnError()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partial = new {tested = "123"}});
            var actualJson = JArray.FromObject(new[] {1, 2, 3});

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Count.Should().Be(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }


        [Test]
        public void WhenHandlingPartial_RemovePartialObjectFromExpected()
        {
            var partialObject = JObject.FromObject(new {parent = new {__partial = new {tested = "123"}}});
            var actualJson = JObject.FromObject(new {parent = new {tested = "123"}});
            var jsonComparer = Substitute.For<IJsonComparer>();

            _partialComparer.Compare(partialObject.Value<JObject>("__partial"), actualJson, jsonComparer)
                .Returns(new List<IJsonCompareError<JToken>>());

            _jsonSpecialHandler.HandleSpecialObject(partialObject.Value<JObject>("parent"), actualJson.Value<JObject>("parent"), "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                partialObject.Value<JObject>("parent").Should().ContainKey("tested");
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRangeIsGiven_RangeIsNotArray_ReturnError()
        {
            var captureObject = JObject.FromObject(new {__match = new {range = "some-regex"}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidMatchObjectJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRangeIsGiven_RangeIsNotArrayOf2Values_ReturnError()
        {
            var captureObject = JObject.FromObject(new {__match = new {range = new[] {1}}});
            var actualJson = JToken.Parse("42");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidMatchObjectJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRegexIsGiven_AndActualValueIsNotANumber_ReturnError()
        {
            var captureObject = JObject.FromObject(new {__match = new {range = new[] {10, 20}}});
            var actualJson = JToken.Parse(@"""some-string""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<InvalidTypeJsonCompareError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_AndRangeIsGiven_AndActualValueIsNotInRange_ReturnError()
        {
            var captureObject = JObject.FromObject(new {__match = new {range = new[] {1, 4}}});
            var actualJson = JToken.Parse(@"10");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Should().HaveCount(1);
                errors.First().Should().BeOfType<ValueOutOfRangeComparerError>();
            }
        }

        [Test]
        public void WhenHandlingMatch_WithRange_ReplaceExpectedWithActualIfItMatch()
        {
            var captureObject = JObject.FromObject(new {parent = new {__match = new {range = new[] {40, 45}}}});
            var actualJson = JObject.FromObject(new {parent = 42});

            _jsonSpecialHandler.HandleSpecialObject(captureObject.Value<JObject>("parent"), actualJson.Value<JToken>("parent"), "parent", null, new JsonComparisonOptions());

            captureObject.Property("parent").Value.ToObject<int>().Should().Be(42);
        }

        [Test]
        [TestCase("abc\r\ndef", "\r", "abc\ndef", true)]
        [TestCase("abc\r\ndef", "\r", "abc\nde\rf", true)]
        [TestCase("abc", "abc", "aaabbbccc", true)]
        [TestCase("abc", "abc", "", true)]
        [TestCase("", "abc", "abcaaa", true)]
        [TestCase("", "e", "", true)]
        [TestCase("eee", "e", "qqqq", false)]
        [TestCase("abc", "", "def", false)]
        public void WhenHandlingMatch_AndStingIgnoreIsGiven_CompareStringIgnoringThoseCharacters(string actual, string ignore, string expected, bool expectedSuccess)
        {
            var captureObject = JObject.FromObject(new {__match = new {ignoredCharacters = ignore, value = expected}});
            var actualJson = JToken.Parse($@"""{actual}""");

            var (success, errors) = _jsonSpecialHandler.HandleSpecialObject(captureObject, actualJson, "", null, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().Be(expectedSuccess);
                if (!expectedSuccess)
                {
                    errors.Should().HaveCount(1);
                    errors.First().Should().BeOfType<InvalidValueJsonCompareError>();
                }
            }
        }
    }
}
