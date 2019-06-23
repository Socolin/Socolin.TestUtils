using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;

namespace Socolin.TestUtils.JsonComparer.Comparers
{
    public interface IJsonValueComparer
    {
        IEnumerable<IJsonCompareError<JToken>> Compare(JValue expected, JValue actual, string path = "");
    }

    public class JsonValueComparer : IJsonValueComparer
    {
        public IEnumerable<IJsonCompareError<JToken>> Compare(JValue expected, JValue actual, string path = "")
        {
            switch (expected.Type)
            {
                case JTokenType.String:
                    if (expected.Value<string>() == actual.Value<string>())
                        yield break;
                    break;
                case JTokenType.Boolean:
                    if (expected.Value<bool>() == actual.Value<bool>())
                        yield break;
                    break;
                case JTokenType.Float:
                    if (Math.Abs(expected.Value<float>() - actual.Value<float>()) < 0.000001)
                        yield break;
                    break;
                case JTokenType.Integer:
                    if (expected.Value<int>() == actual.Value<int>())
                        yield break;
                    break;
                case JTokenType.Date:
                    if (expected.Value<DateTime>() == actual.Value<DateTime>())
                        yield break;
                    break;
                default:
                    if (expected.Value == actual.Value)
                        yield break;
                    break;
            }

            yield return new InvalidValueJsonCompareError(path, expected, actual);
        }
    }
}