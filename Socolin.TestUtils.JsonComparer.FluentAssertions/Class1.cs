using System;

namespace Socolin.TestUtils.JsonComparer.FluentAssertions
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfoAssertions Should(this object instance)
        {
            return new DirectoryInfoAssertions(instance);
        }
    }
}
