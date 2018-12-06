using Newtonsoft.Json.Linq;

namespace Socolin.TestUtils.JsonComparer.NUnitExtensions
{
    public class Is : NUnit.Framework.Is
    {
        public static JsonEquivalentConstraint JsonEquivalent(string expected)
        {
            return new JsonEquivalentConstraint(expected);
        }

        public static JsonEquivalentConstraint JsonEquivalent(JToken expected)
        {
            return new JsonEquivalentConstraint(expected);
        }
    }
}