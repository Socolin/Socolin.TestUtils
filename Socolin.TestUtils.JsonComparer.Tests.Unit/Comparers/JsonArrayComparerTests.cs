using System.Collections.Generic;
using System.Linq;
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
    public class JsonArrayComparerTests
    {
        private JsonArrayComparer _jsonArrayComparer;
        private IJsonComparer _jsonComparer;

        [SetUp]
        public void SetUp()
        {
            _jsonComparer = Substitute.For<IJsonComparer>();
            _jsonArrayComparer = new JsonArrayComparer();
            _jsonComparer.Compare(Arg.Any<JToken>(), Arg.Any<JToken>(), Arg.Any<string>())
                .Returns(new List<JsonCompareError>());
        }

        [Test]
        [TestCase("[1]", "[]")]
        [TestCase("[]", "[1]")]
        [TestCase("[1,2,3]", "[1,2]")]
        public void WhenComparingArray_ReturnsInvalidSize(string expectedJson, string actualJson)
        {
            var expectedJArray = JArray.Parse(expectedJson);
            var actualJArray = JArray.Parse(actualJson);

            var errors = _jsonArrayComparer.Compare(expectedJArray, actualJArray, _jsonComparer).ToList();

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError?.ActualValue.Should().BeSameAs(actualJArray);
                firstError?.ExpectedValue.Should().BeSameAs(expectedJArray);
                firstError.Should().BeOfType<InvalidSizeJsonCompareError>();
            }
        }

        [Test]
        public void WhenComparingArray_CallJsonComparerOnEachValues()
        {
            var expectedJArray = JArray.Parse("[1,2,3]");
            var actualJArray = JArray.Parse("[1,2,3]");

            _jsonComparer.Compare(expectedJArray[0], actualJArray[0], "[0]")
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});
            _jsonComparer.Compare(expectedJArray[2], actualJArray[2], "[2]")
                .Returns(new List<JsonCompareError> {new TestJsonCompareError()});

            var errors = _jsonArrayComparer.Compare(expectedJArray, actualJArray, _jsonComparer).ToList();

            using (new AssertionScope())
            {
                errors.Should().ContainItemsAssignableTo<TestJsonCompareError>();
                errors.Count.Should().Be(2);
            }
        }
    }
}