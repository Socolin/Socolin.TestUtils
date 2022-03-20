using System.Collections.Generic;
using FluentAssertions;
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

    [When(@"comparing both JSON")]
    public void WhenComparingBothJson()
    {
        var jsonComparer = JsonComparer.GetDefault();
        var expectedJson = JToken.Parse(_scenarioContext.Get<string>("ExpectedJson"));
        var actualJson = JToken.Parse(_scenarioContext.Get<string>("Json"));
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
