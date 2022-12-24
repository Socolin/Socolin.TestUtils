using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;
using Socolin.TestUtils.JsonComparer.Tests.Unit.Errors;

namespace Socolin.TestUtils.JsonComparer.Tests.Unit.Comparers
{
    public class JsonObjectComparerTests
    {
        private JsonObjectComparer _jsonObjectComparer;
        private IJsonComparer _jsonComparer;

        [SetUp]
        public void SetUp()
        {
            _jsonComparer = Substitute.For<IJsonComparer>();
            _jsonObjectComparer = new JsonObjectComparer();
            _jsonComparer.Compare(Arg.Any<JToken>(), Arg.Any<JToken>(), Arg.Any<string>())
                .Returns(new List<JsonCompareError>());
        }

        [Test]
        public void WhenComparingObjects_ReturnsMissingProperties()
        {
            var expectedJObject = JObject.FromObject(new {a = 1, someMissingProperty = 2});
            var actualJObject = JObject.FromObject(new {a = 1});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer);

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError.Should().BeOfType<MissingPropertyJsonComparerError>();
                firstError?.ActualValue.Should().BeSameAs(actualJObject);
                firstError?.ExpectedValue.Should().BeSameAs(expectedJObject);
                (firstError as MissingPropertyJsonComparerError)?.MissingProperty.Name.Should().Be("someMissingProperty");
            }
        }

        [Test]
        public void WhenComparingObjects_ReturnsUnexpectedProperties()
        {
            var expectedJObject = JObject.FromObject(new {a = 1});
            var actualJObject = JObject.FromObject(new {a = 1, someUnexpectedProperty = 42});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer);

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError.Should().BeOfType<UnexpectedPropertyJsonComparerError>();
                firstError?.ActualValue.Should().BeSameAs(actualJObject);
                firstError?.ExpectedValue.Should().BeSameAs(expectedJObject);
                (firstError as UnexpectedPropertyJsonComparerError)?.UnexpectedProperty.Name.Should().Be("someUnexpectedProperty");
            }
        }

        [Test]
        public void WhenComparingObjects_CallJsonCompareOnEachProperty()
        {
            var expectedJObject = JObject.FromObject(new {someProperty = 42});
            var actualJObject = JObject.FromObject(new {someProperty = 42});

            _jsonComparer.Compare(expectedJObject["someProperty"], actualJObject["someProperty"], "someProperty")
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer);

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError.Should().BeOfType<TestJsonCompareError>();
            }
        }

        [Test]
        public void WhenComparingObjects_AndAPropertyIsMissing_AndFieldIsIgnored_ReturnsNoErrors()
        {
            var expectedJObject = JObject.FromObject(new {a = 1, someMissingProperty = 2});
            var actualJObject = JObject.FromObject(new {a = 1});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someMissingProperty" && fieldPath == "some-path"
            });

            errors.Should().BeEmpty();
        }

        [Test]
        public void WhenComparingObjects_AndPropertyIsUnexpectedAndFieldIsIgnored_ReturnsNoErrors()
        {
            var expectedJObject = JObject.FromObject(new {a = 1});
            var actualJObject = JObject.FromObject(new {a = 1, someUnexpectedProperty = 42});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someUnexpectedProperty" && fieldPath == "some-path"
            });

            errors.Should().BeEmpty();

        }

        [Test]
        public void WhenComparingObjects_IfPropertyIsIgnored_DoNotCompareThisProperty()
        {
            var expectedJObject = JObject.FromObject(new {someProperty = 42});
            var actualJObject = JObject.FromObject(new {someProperty = 42});

            _jsonComparer.Compare(Arg.Any<JToken>(), Arg.Any<JToken>(), Arg.Any<string>(), Arg.Any<JsonComparisonOptions>())
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someProperty" && fieldPath == "some-path"
            });

            errors.Should().BeEmpty();
        }

        [Test]
        public void WhenComparingObjects_AndAPropertyIsMissing_AndFieldIsIgnored_RemoveField()
        {
            var expectedJObject = JObject.FromObject(new {a = 1, someMissingProperty = 2});
            var actualJObject = JObject.FromObject(new {a = 1});

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someMissingProperty" && fieldPath == "some-path"
            }).ToList();

            expectedJObject.Properties().Should().NotContain(e => e.Name == "someMissingProperty");
        }

        [Test]
        public void WhenComparingObjects_AndPropertyIsUnexpectedAndFieldIsIgnored_RemoveProperty()
        {
            var expectedJObject = JObject.FromObject(new {a = 1});
            var actualJObject = JObject.FromObject(new {a = 1, someUnexpectedProperty = 42});

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someUnexpectedProperty" && fieldPath == "some-path"
            }).ToList();

            actualJObject.Properties().Should().NotContain(e => e.Name == "someUnexpectedProperty");
        }

        [Test]
        public void WhenComparingObjects_IfPropertyIsIgnored_RemoveThisProperty()
        {
            var expectedJObject = JObject.FromObject(new {someProperty = 42});
            var actualJObject = JObject.FromObject(new {someProperty = 42});

            _jsonComparer.Compare(Arg.Any<JToken>(), Arg.Any<JToken>(), Arg.Any<string>(), Arg.Any<JsonComparisonOptions>())
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer, "some-path", new JsonComparisonOptions
            {
                IgnoreFields = (fieldPath, fieldName) => fieldName == "someProperty" && fieldPath == "some-path"
            }).ToList();

            expectedJObject.Properties().Should().NotContain(e => e.Name == "someProperty");
            actualJObject.Properties().Should().NotContain(e => e.Name == "someProperty");
        }

    }
}
