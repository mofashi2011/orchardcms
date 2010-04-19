// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.2.0.0
//      Runtime Version:2.0.50727.4200
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace Orchard.Specs
{
    using TechTalk.SpecFlow;
    
    
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Web Hosting")]
    public partial class WebHostingFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "WebHosting.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Web Hosting", "In order to test orchard\r\nAs an integration runner\r\nI want to verify basic hostin" +
                    "g is working", ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning static files")]
        public virtual void ReturningStaticFiles()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning static files", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
  testRunner.Given("I have a clean site");
#line 8
  testRunner.When("I go to \"Content/Static.txt\"");
#line 9
  testRunner.Then("the status should be 200 OK");
#line 10
    testRunner.And("I should see \"Hello world!\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning static files 2")]
        public virtual void ReturningStaticFiles2()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning static files 2", ((string[])(null)));
#line 12
this.ScenarioSetup(scenarioInfo);
#line 13
  testRunner.Given("I have a clean site");
#line 14
  testRunner.When("I go to \"Content\\Static.txt\"");
#line 15
  testRunner.Then("the status should be 200 OK");
#line 16
    testRunner.And("I should see \"Hello world!\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning static files 3")]
        public virtual void ReturningStaticFiles3()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning static files 3", ((string[])(null)));
#line 18
this.ScenarioSetup(scenarioInfo);
#line 19
  testRunner.Given("I have a clean site");
#line 20
  testRunner.When("I go to \"/Static.txt\"");
#line 21
  testRunner.Then("the status should be 200 OK");
#line 22
    testRunner.And("I should see \"Hello world!\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning static files 4")]
        public virtual void ReturningStaticFiles4()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning static files 4", ((string[])(null)));
#line 24
this.ScenarioSetup(scenarioInfo);
#line 25
  testRunner.Given("I have a clean site");
#line 26
  testRunner.When("I go to \"Static.txt\"");
#line 27
  testRunner.Then("the status should be 200 OK");
#line 28
    testRunner.And("I should see \"Hello world!\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning web forms page")]
        public virtual void ReturningWebFormsPage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning web forms page", ((string[])(null)));
#line 30
this.ScenarioSetup(scenarioInfo);
#line 31
  testRunner.Given("I have a clean site");
#line 32
  testRunner.When("I go to \"Simple/Page.aspx\"");
#line 33
  testRunner.Then("the status should be 200 OK");
#line 34
    testRunner.And("I should see \"Hello again\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning web forms page 2")]
        public virtual void ReturningWebFormsPage2()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning web forms page 2", ((string[])(null)));
#line 36
this.ScenarioSetup(scenarioInfo);
#line 37
  testRunner.Given("I have a clean site");
#line 38
  testRunner.When("I go to \"Simple\\Page.aspx\"");
#line 39
  testRunner.Then("the status should be 200 OK");
#line 40
    testRunner.And("I should see \"Hello again\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Returning a routed request")]
        public virtual void ReturningARoutedRequest()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Returning a routed request", ((string[])(null)));
#line 42
this.ScenarioSetup(scenarioInfo);
#line 43
  testRunner.Given("I have a clean site");
#line 44
  testRunner.When("I go to \"hello-world\"");
#line 45
  testRunner.Then("the status should be 200 OK");
#line 46
    testRunner.And("I should see \"Hello yet again\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Following a link")]
        public virtual void FollowingALink()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Following a link", ((string[])(null)));
#line 48
this.ScenarioSetup(scenarioInfo);
#line 49
  testRunner.Given("I have a clean site");
#line 50
  testRunner.When("I go to \"/simple/page.aspx\"");
#line 51
    testRunner.And("I follow \"next page\"");
#line 52
  testRunner.Then("the status should be 200 OK");
#line 53
    testRunner.And("I should see \"Hello yet again\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
    }
}