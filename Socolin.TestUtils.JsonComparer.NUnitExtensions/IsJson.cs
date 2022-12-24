using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions;

[PublicAPI]
public class IsJson
{
    public static JsonEquivalentConstraint EquivalentTo(string expected)
    {
        return new JsonEquivalentConstraint(expected);
    }

    public static JsonEquivalentConstraint EquivalentTo(JToken expected)
    {
        return new JsonEquivalentConstraint(expected);
    }
}
