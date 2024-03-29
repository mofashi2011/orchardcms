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
    [NUnit.Framework.DescriptionAttribute("Setup")]
    public partial class SetupFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Setup.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Setup", "In order to install orchard\r\nAs a new user\r\nI want to setup a new site from the d" +
                    "efault screen", ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Root request shows setup form")]
        public virtual void RootRequestShowsSetupForm()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Root request shows setup form", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
 testRunner.Given("I have a clean site");
#line 8
  testRunner.And("I have module \"Orchard.Setup\"");
#line 9
  testRunner.And("I have theme \"SafeMode\"");
#line 10
 testRunner.When("I go to \"/Default.aspx\"");
#line 11
 testRunner.Then("I should see \"Welcome to Orchard\"");
#line 12
  testRunner.And("I should see \"Finish Setup\"");
#line 13
  testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Setup folder also shows setup form")]
        public virtual void SetupFolderAlsoShowsSetupForm()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Setup folder also shows setup form", ((string[])(null)));
#line 15
this.ScenarioSetup(scenarioInfo);
#line 16
 testRunner.Given("I have a clean site");
#line 17
  testRunner.And("I have module \"Orchard.Setup\"");
#line 18
  testRunner.And("I have theme \"SafeMode\"");
#line 19
 testRunner.When("I go to \"/Setup\"");
#line 20
 testRunner.Then("I should see \"Welcome to Orchard\"");
#line 21
  testRunner.And("I should see \"Finish Setup\"");
#line 22
  testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Some of the initial form values are required")]
        public virtual void SomeOfTheInitialFormValuesAreRequired()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Some of the initial form values are required", ((string[])(null)));
#line 24
this.ScenarioSetup(scenarioInfo);
#line 25
 testRunner.Given("I have a clean site");
#line 26
  testRunner.And("I have module \"Orchard.Setup\"");
#line 27
  testRunner.And("I have theme \"SafeMode\"");
#line 28
 testRunner.When("I go to \"/Setup\"");
#line 29
  testRunner.And("I hit \"Finish Setup\"");
#line 30
 testRunner.Then("I should see \"Site name is required\"");
#line 31
  testRunner.And("I should see \"Password is required\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Calling setup on a brand new install")]
        public virtual void CallingSetupOnABrandNewInstall()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Calling setup on a brand new install", ((string[])(null)));
#line 33
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "extension",
                        "names"});
            table1.AddRow(new string[] {
                        "module",
                        "Orchard.Setup, Orchard.Users, Orchard.Roles, Orchard.Pages, Orchard.Comments, Orc" +
                            "hard.Themes, TinyMce"});
            table1.AddRow(new string[] {
                        "core",
                        "Common, Dashboard, Feeds, HomePage, Navigation, Scheduling, Settings, XmlRpc"});
            table1.AddRow(new string[] {
                        "theme",
                        "SafeMode, Classic"});
#line 34
 testRunner.Given("I have a clean site with", ((string)(null)), table1);
#line 39
  testRunner.And("I am on \"/Setup\"");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table2.AddRow(new string[] {
                        "SiteName",
                        "My Site"});
            table2.AddRow(new string[] {
                        "AdminPassword",
                        "6655321"});
#line 40
 testRunner.When("I fill in", ((string)(null)), table2);
#line 44
  testRunner.And("I hit \"Finish Setup\"");
#line 45
  testRunner.And("I go to \"/Default.aspx\"");
#line 46
 testRunner.Then("I should see \"<h1>My Site</h1>\"");
#line 47
  testRunner.And("I should see \"Welcome, <strong>admin</strong>!\"");
#line 48
  testRunner.And("I should see \"you\'ve successfully set-up your Orchard site\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
    }
}
