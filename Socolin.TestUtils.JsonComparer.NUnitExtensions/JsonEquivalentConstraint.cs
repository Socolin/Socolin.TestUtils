using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions
{
	public class JsonEquivalentConstraint : Constraint
	{
		private IJsonComparer _jsonComparer;
		private JsonComparisonOptions _options;

		public JsonEquivalentConstraint(string expected)
			: this(JsonConvert.DeserializeObject<JToken>(expected, new JsonSerializerSettings
			{
				DateParseHandling = DateParseHandling.None
			}))
		{
		}

		public JsonEquivalentConstraint(JToken expected)
			: base(expected)
		{
		}

		public override ConstraintResult ApplyTo<TActual>(TActual actual)
		{
			var actualJToken = actual as JToken ?? JsonConvert.DeserializeObject<JToken>(actual as string, new JsonSerializerSettings
			{
				DateParseHandling = DateParseHandling.None
			});
			var expectedJToken = (JToken) Arguments[0];
			var jsonComparer = _jsonComparer ?? JsonComparer.GetDefault();
			var errors = jsonComparer.Compare(expectedJToken, actualJToken, _options);
			var message = JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors);
			return new JsonEquivalentConstraintResult(this, actual, errors?.Count == 0, message);
		}

		public JsonEquivalentConstraint WithComparer(IJsonComparer jsonComparer)
		{
			_jsonComparer = jsonComparer;
			return this;
		}

		public JsonEquivalentConstraint WithOptions(JsonComparisonOptions options)
		{
			_options = options;
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
