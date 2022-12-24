using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.TestUtils.JsonComparer.Errors;
using TechTalk.SpecFlow;

namespace Socolin.TestUtils.JsonComparer.Tests.Functional.Steps;

[Binding]
public class JsonSteps
{
    private readonly ScenarioContext _scenarioContext;

    public JsonSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"the following JSON")]
    public void GivenTheFollowingJson(string json)
    {
        _scenarioContext["Json"] = json;
    }

    [Given(@"the expected JSON")]
    public void GivenTheExpectedJson(string json)
    {
        _scenarioContext["ExpectedJson"] = json;
    }

    [Given(@"the regex """"(.*)"""" has been registered as an alias with the name ""(.*)""")]
    public void GivenTheRegexHasBeenRegisteredAsAnAliasWithTheName(string regex, string name)
    {
        if (!_scenarioContext.TryGetValue<RegexAliasesContainer>(out var regexAliasesContainer))
        {
            regexAliasesContainer = new RegexAliasesContainer();
            _scenarioContext.Set(regexAliasesContainer);
        }

        regexAliasesContainer.AddAlias(name, regex);
    }

    [When(@"comparing both JSON")]
    public void WhenComparingBothJson()
    {
        var jsonSerializerSettings = new JsonSerializerSettings {DateParseHandling = DateParseHandling.None};
        _scenarioContext.TryGetValue<RegexAliasesContainer>(out var regexAliasesContainer);
        var jsonComparer = JsonComparer.GetDefault(regexAliasesContainer: regexAliasesContainer);
        var expectedJson = JsonConvert.DeserializeObject<JToken>(_scenarioContext.Get<string>("ExpectedJson"), jsonSerializerSettings);
        var actualJson = JsonConvert.DeserializeObject<JToken>(_scenarioContext.Get<string>("Json"), jsonSerializerSettings);
        var errors = jsonComparer.Compare(expectedJson, actualJson);
        _scenarioContext["Errors"] = errors;
        _scenarioContext["Output"] = JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors);
    }

    [Then(@"the json did not match with the following output")]
    public void ThenTheJsonDidNotMatchWithTheFollowingOutput(string expected)
    {
        _scenarioContext.Get<IList<IJsonCompareError<JToken>>>("Errors").Should().NotBeEmpty();
        _scenarioContext.Get<string>("Output").TrimEnd().Replace("\r\n", "\n").Should().Be(expected.Replace("\r\n", "\n"));
    }

    [Then(@"the 2 json matched")]
    public void ThenTheJsonMatched()
    {
        _scenarioContext.Get<IList<IJsonCompareError<JToken>>>("Errors").Should().BeEmpty();
    }
}
