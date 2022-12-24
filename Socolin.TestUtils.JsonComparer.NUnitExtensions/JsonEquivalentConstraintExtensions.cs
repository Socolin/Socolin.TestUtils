using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions;

[PublicAPI]
public static class JsonEquivalentConstraintExtensions
{
    public static JsonEquivalentConstraint JsonEquivalent(this ConstraintExpression expression, JToken expected)
    {
        var constraint = new JsonEquivalentConstraint(expected);
        expression.Append(constraint);
        return constraint;
    }

    public static JsonEquivalentConstraint JsonEquivalent(this ConstraintExpression expression, string expected)
    {
        var constraint = new JsonEquivalentConstraint(expected);
        expression.Append(constraint);
        return constraint;
    }
}
