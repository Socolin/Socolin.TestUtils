using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions
{
    public class JsonEquivalentConstraint : Constraint
    {
        private IJsonComparer _jsonComparer;

        public JsonEquivalentConstraint(string expected)
            : this(JToken.Parse(expected))
        {
        }

        public JsonEquivalentConstraint(JToken expected)
            : base(expected)
        {
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var actualJToken = actual as JToken ?? JToken.Parse(actual as string);
            var expectedJToken = (JToken) Arguments[0];
            var jsonComparer = _jsonComparer ?? JsonComparer.GetDefault();
            var errors = jsonComparer.Compare(expectedJToken, actualJToken);
            var message = JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors);
            return new JsonEquivalentConstraintResult(this, actual, errors?.Count == 0, message);
        }

        public JsonEquivalentConstraint WithComparer(IJsonComparer jsonComparer)
        {
            _jsonComparer = jsonComparer;
            return this;
        }

        public override string Description { get; protected set; }

        private class JsonEquivalentConstraintResult : ConstraintResult
        {
            private readonly string _message;

            public JsonEquivalentConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, string message)
                : base(constraint, actualValue, isSuccess)
            {
                _message = message;
            }

            public override void WriteMessageTo(MessageWriter writer)
            {
                writer.WriteLine(_message);
            }
        }
    }
}