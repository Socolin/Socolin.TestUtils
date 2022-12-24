#nullable enable
using System.Text.RegularExpressions;

namespace Socolin.TestUtils.JsonComparer;

public class RegexAliasesContainer
{
    private readonly Dictionary<string, Regex> _regexesByAlias = new();

    public Regex? GetRegex(string alias)
    {
        return _regexesByAlias.TryGetValue(alias, out var regex) ? regex : null;
    }

    public void AddAlias(string alias, string regex)
    {
        _regexesByAlias[alias] = new Regex(regex, RegexOptions.Compiled);
    }

    public void AddAlias(string alias, Regex regex)
    {
        _regexesByAlias[alias] = regex;
    }

    public void RemoveAlias(string alias)
    {
        _regexesByAlias.Remove(alias);
    }

    public void Clear()
    {
        _regexesByAlias.Clear();
    }
}
