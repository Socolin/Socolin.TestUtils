using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestsUtils.JsonComparer.Comparers;
using Socolin.TestsUtils.JsonComparer.Errors;
using Socolin.TestsUtils.JsonComparer.Tests.Unit.Errors;

namespace Socolin.TestsUtils.JsonComparer.Tests.Unit.Comparers
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
    }
}