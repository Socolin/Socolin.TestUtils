using System.Collections.Generic;
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
    public class PartialArrayHandlerTests
    {
        private PartialArrayHandler _partialArrayHandler;

        [SetUp]
        public void SetUp()
        {
            _partialArrayHandler = new PartialArrayHandler();
        }

        [Test]
        public void HandlePartialArrayObject_WhenMatch_ReturnSuccess()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partialArray = new {key = "id", array = new[] {new {id = 1}, new {id = 2}, new {id = 3}}}});
            var actualJson = JArray.FromObject(new[] {new {id = 1}, new {id = 2}, new {id = 3}, new {id = 4}, new {id = 5}});

            var (success, errors) = _partialArrayHandler.HandlePartialArrayObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            success.Should().BeTrue();
            errors.Should().BeNullOrEmpty();
        }

        [Test]
        public void HandlePartialArrayObject_ShouldReturnAnError_WhenActualIsNotAnArray()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partialArray = new {key = "id", array = new[] {new {id = 1}, new {id = 2}, new {id = 3}}}});
            var actualJson = JObject.FromObject(new {a = true});

            var (success, errors) = _partialArrayHandler.HandlePartialArrayObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Count.Should().Be(1);
                errors.First().Should().BeOfType<InvalidPartialObjectCompareError>();
            }
        }

        [Test]
        public void HandlePartialArrayObject_ShouldReturnAnError_WhenMissingElement()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partialArray = new {key = "id", array = new[] {new {id = 1}, new {id = 2}, new {id = 3}}}});
            var actualJson = JArray.FromObject(new[] {new {id = 1}, /* 2, */ new {id = 3}, new {id = 4}});

            var (success, errors) = _partialArrayHandler.HandlePartialArrayObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Count.Should().Be(1);
                errors.First().Should().BeOfType<MissingObjectInArrayComparerError>();
            }
        }

        [Test]
        public void HandlePartialArrayObject_ShouldReturnAnErrorFromComparisonBetweenEachElement()
        {
            var fakeError = Substitute.For<IJsonCompareError<JToken>>();
            var jsonComparisonOptions = new JsonComparisonOptions();
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partialArray = new {key = "id", array = new[] {new {id = 1, name = "name1"}}}});
            var actualJson = JArray.FromObject(new[] {new {id = 1, name = "name2"}});

            jsonComparer.Compare(Arg.Is<JObject>(j => j.Value<string>("name") == "name1"), Arg.Is<JObject>(j => j.Value<string>("name") == "name2"), jsonComparisonOptions)
                .Returns(new List<IJsonCompareError<JToken>> {fakeError});

            var (success, errors) = _partialArrayHandler.HandlePartialArrayObject(partialObject, actualJson, "", jsonComparer, jsonComparisonOptions);

            using (new AssertionScope())
            {
                success.Should().BeFalse();
                errors.Count.Should().Be(1);
                errors.First().Should().BeSameAs(fakeError);
            }
        }

        [Test]
        public void HandlePartialArrayObject_ShouldRemoveIgnoredElement_FromActual()
        {
            var jsonComparer = Substitute.For<IJsonComparer>();
            var partialObject = JObject.FromObject(new {__partialArray = new {key = "id", array = new[] {new {id = 1}}}});
            var actualJson = JArray.FromObject(new[] {new {id = 1}, new {id = 3}, new {id = 4}});
            var parent = new JObject {{"test", actualJson}};

            _partialArrayHandler.HandlePartialArrayObject(partialObject, actualJson, "", jsonComparer, new JsonComparisonOptions());

            using (new AssertionScope())
            {
                parent.Value<JArray>("test").Should().HaveCount(1);
            }
        }

    }
}
