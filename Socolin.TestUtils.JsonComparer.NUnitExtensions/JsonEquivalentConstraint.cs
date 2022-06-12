using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using Socolin.TestUtils.JsonComparer.Color;
using Socolin.TestUtils.JsonComparer.Utils;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions
{
	public class JsonEquivalentConstraint : Constraint
	{
		private IJsonComparer _jsonComparer;
		private JsonComparisonOptions _options;
		private JsonComparerColorOptions _colorOptions;

		public JsonEquivalentConstraint(string expected)
			: base(expected)
		{
		}

		public JsonEquivalentConstraint(JToken expected)
			: base(expected)
		{
		}

		public override ConstraintResult ApplyTo<TActual>(TActual actual)
		{
			var actualJToken = actual as JToken ?? JsonDeserializerErrorFormatterHelper.DeserializeWithNiceErrorMessage<JToken>(actual as string, new JsonSerializerSettings
			{
				DateParseHandling = DateParseHandling.None
			}, _colorOptions);

			if (!(Arguments[0] is JToken expectedJToken))
			{
				if (!(Arguments[0] is string))
					throw new ArgumentException("Invalid Arguments[0]");
				expectedJToken = JsonDeserializerErrorFormatterHelper.DeserializeWithNiceErrorMessage<JToken>(Arguments[0] as string, new JsonSerializerSettings
				{
					DateParseHandling = DateParseHandling.None
				}, _colorOptions);
			}

			var jsonComparer = _jsonComparer ?? JsonComparer.GetDefault(colorOptions: _colorOptions);
			var errors = jsonComparer.Compare(expectedJToken, actualJToken, _options);
			var message = JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors, _colorOptions);
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

		public JsonEquivalentConstraint WithColoredOutput(bool useColor = true)
		{
			_colorOptions = useColor ? JsonComparerColorOptions.DefaultColored : JsonComparerColorOptions.Default;
			return this;
		}

		public JsonEquivalentConstraint WithColorOptions(JsonComparerColorOptions colorOptions)
		{
            _colorOptions = colorOptions;
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
