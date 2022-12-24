using System;

namespace Socolin.TestUtils.JsonComparer.Exceptions;

[Serializable]
public class RegexAliasNotRegisteredException : Exception
{
    public string Alias { get; }

    public RegexAliasNotRegisteredException(string alias)
        : base($"Regex alias '{alias}' was not registered in JsonComparer")
    {
        Alias = alias;
    }
}
