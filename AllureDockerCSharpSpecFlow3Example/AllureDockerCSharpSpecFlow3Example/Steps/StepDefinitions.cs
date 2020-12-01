using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AllureDockerCSharpSpecFlow3Example.Steps
{
    [Binding]
    public class StepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        public StepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I'm on a site")]
        public void GivenImOnASite()
        {
        }

        [When(@"I enter ""(.*)"" on the page")]
        public void WhenIEnterOnThePage(string p0)
        {
        }


        [Then(@"I verify is ""(.*)""")]
        public void ThenIVerifyIs(string status)
        {
			switch (status) {
				case "FAILED":
					Assert.Fail("FAILURE ON PURPOSE");
					break;
            }
        }
    }
}
