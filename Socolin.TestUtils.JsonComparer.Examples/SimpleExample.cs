﻿using Socolin.TestUtils.JsonComparer.Color;

namespace Socolin.TestUtils.JsonComparer.Examples;

public class SimpleExample
{
    public void Test1()
    {
        Console.WriteLine("==== SimpleExample.Test1 ==== ");
        const string expectedJson = """
            {
                "a":1,
                "b":"abc"
            }
            """;
        const string actualJson = """
            {
                "a":42,
                "b":"abc"
            }
            """;
        var jsonComparer = JsonComparer.GetDefault();
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, JsonComparerColorOptions.DefaultColored));
    }
}
