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
    public class JsonObjectPartialComparerTests
    {
        private JsonObjectPartialComparer _jsonObjectComparer;
        private IJsonComparer _jsonComparer;

        [SetUp]
        public void SetUp()
        {
            _jsonComparer = Substitute.For<IJsonComparer>();
            _jsonObjectComparer = new JsonObjectPartialComparer();
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
        public void WhenComparingObjects_AndActualObjectHaveFieldsThatAreNotInExpected_ReturnsNoErrors()
        {
            var expectedJObject = JObject.FromObject(new {a = 1});
            var actualJObject = JObject.FromObject(new {a = 1, someUnexpectedProperty = 42});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer);

            errors.Should().BeNullOrEmpty();
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
        public void WhenComparingObjects_RemoveIgnoredPropertiesFromActual()
        {
            var expectedJObject = JObject.FromObject(new {someProperty = 42});
            var actualJObject = JObject.FromObject(new {someProperty = 42, someIgnoredProperty = 1});

            _jsonComparer.Compare(expectedJObject["someProperty"], actualJObject["someProperty"], "someProperty")
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var errors = _jsonObjectComparer.Compare(expectedJObject, actualJObject, _jsonComparer);

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError.Should().BeOfType<TestJsonCompareError>();
                actualJObject.Should().NotContainKey("someIgnoredProperty");
            }
        }
    }
}
