using System;

namespace Socolin.TestUtils.JsonComparer;

public class JsonComparisonOptions
{
    public Func<string, string, bool> IgnoreFields { get; set; }
}