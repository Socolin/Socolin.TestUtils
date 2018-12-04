namespace Socolin.TestsUtils.JsonComparer.Utils
{
    public class JsonPathUtils
    {
        public static string Combine(string path, string element)
        {
            if (string.IsNullOrEmpty(path))
            {
                return element;
            }

            return $"{path}.{element}";
        }
    }
}