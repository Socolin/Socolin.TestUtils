using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.TestUtils.JsonComparer.Comparers;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Tests.Unit.Comparers
{
    public class JsonValueComparerTests
    {
        private JsonValueComparer _jsonValueComparer;

        [SetUp]
        public void SetUp()
        {
            _jsonValueComparer = new JsonValueComparer();
        }

        private static readonly TestCaseData[] JValues =
        {
            new TestCaseData(new JValue(48), new JValue(42)).SetArgDisplayNames("int"),
            new TestCaseData(new JValue(42.2f), new JValue(42.5f)).SetArgDisplayNames("float"),
            new TestCaseData(new JValue("some-string-1"), new JValue("some-string-2")).SetArgDisplayNames("string"),
            new TestCaseData(new JValue(false), new JValue(true)).SetArgDisplayNames("bool"),
        };

        [TestCaseSource(nameof(JValues))]
        public void WhenComparingValues_ReturnErrorWhenNotEquals(JValue expectedJValue, JValue actualJValue)
        {
            var errors = _jsonValueComparer.Compare(expectedJValue, actualJValue);

            using (new AssertionScope())
            {
                var firstError = errors.FirstOrDefault();
                firstError.Should().NotBeNull();
                firstError.Should().BeOfType<InvalidValueJsonCompareError>();
                firstError?.ActualValue.Should().BeSameAs(actualJValue);
                firstError?.ExpectedValue.Should().BeSameAs(expectedJValue);
            }
        }


        private static readonly TestCaseData[] EqualJValues =
        {
            new TestCaseData(new JValue(42), new JValue(42)).SetArgDisplayNames("int"),
            new TestCaseData(new JValue(42.5f), new JValue(42.5f)).SetArgDisplayNames("float"),
            new TestCaseData(new JValue("some-string-1"), new JValue("some-string-1")).SetArgDisplayNames("string"),
            new TestCaseData(new JValue(true), new JValue(true)).SetArgDisplayNames("bool"),
        };

        [TestCaseSource(nameof(EqualJValues))]
        public void WhenComparingValues_DoNotReturnErrorForDuplicateValues(JValue expectedJValue, JValue actualJValue)
        {
            var errors = _jsonValueComparer.Compare(expectedJValue, actualJValue);

            errors.Should().BeEmpty();
        }
    }
}