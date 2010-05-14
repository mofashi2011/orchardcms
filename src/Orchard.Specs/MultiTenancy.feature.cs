// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.3.0.0
//      Runtime Version:2.0.50727.4927
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace Orchard.Specs
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.3.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Multiple tenant management")]
    public partial class MultipleTenantManagementFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "MultiTenancy.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Multiple tenant management", "In order to host several isolated web applications\r\nAs a root Orchard system oper" +
                    "ator\r\nI want to create and manage tenant configurations", ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Default site is listed")]
        public virtual void DefaultSiteIsListed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Default site is listed", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
testRunner.Given("I have installed Orchard");
#line 8
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 9
testRunner.When("I go to \"Admin/MultiTenancy\"");
#line 10
testRunner.Then("I should see \"List of Site\'s Tenants\"");
#line 11
testRunner.And("I should see \"<span class=\"tenantName\">Default</span>\"");
#line 12
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("New tenant fields are required")]
        public virtual void NewTenantFieldsAreRequired()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("New tenant fields are required", ((string[])(null)));
#line 14
this.ScenarioSetup(scenarioInfo);
#line 15
testRunner.Given("I have installed Orchard");
#line 16
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 17
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line 18
testRunner.And("I hit \"Save\"");
#line 19
testRunner.Then("I should see \"is required\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A new tenant is created")]
        public virtual void ANewTenantIsCreated()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A new tenant is created", ((string[])(null)));
#line 21
this.ScenarioSetup(scenarioInfo);
#line 22
testRunner.Given("I have installed Orchard");
#line 23
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 24
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table1.AddRow(new string[] {
                        "Name",
                        "Scott"});
#line 25
testRunner.And("I fill in", ((string)(null)), table1);
#line 28
testRunner.And("I hit \"Save\"");
#line 29
testRunner.And("I am redirected");
#line 30
testRunner.Then("I should see \"<span class=\"tenantName\">Scott</span>\"");
#line 31
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A new tenant is created with uninitialized state")]
        public virtual void ANewTenantIsCreatedWithUninitializedState()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A new tenant is created with uninitialized state", ((string[])(null)));
#line 33
this.ScenarioSetup(scenarioInfo);
#line 34
testRunner.Given("I have installed Orchard");
#line 35
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 36
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table2.AddRow(new string[] {
                        "Name",
                        "Scott"});
#line 37
testRunner.And("I fill in", ((string)(null)), table2);
#line 40
testRunner.And("I hit \"Save\"");
#line 41
testRunner.And("I am redirected");
#line 42
testRunner.Then("I should see \"<li class=\"tenant Uninitialized\">\"");
#line 43
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A new tenant goes to the setup screen")]
        public virtual void ANewTenantGoesToTheSetupScreen()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A new tenant goes to the setup screen", ((string[])(null)));
#line 45
this.ScenarioSetup(scenarioInfo);
#line 46
testRunner.Given("I have installed Orchard");
#line 47
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 48
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table3.AddRow(new string[] {
                        "Name",
                        "Scott"});
            table3.AddRow(new string[] {
                        "RequestUrlHost",
                        "scott.example.org"});
#line 49
testRunner.And("I fill in", ((string)(null)), table3);
#line 53
testRunner.And("I hit \"Save\"");
#line 54
testRunner.And("I go to \"/Setup\" on host scott.example.org");
#line 55
testRunner.Then("I should see \"Welcome to Orchard\"");
#line 56
testRunner.And("I should see \"Finish Setup\"");
#line 57
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A new tenant with preconfigured database goes to the setup screen")]
        public virtual void ANewTenantWithPreconfiguredDatabaseGoesToTheSetupScreen()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A new tenant with preconfigured database goes to the setup screen", ((string[])(null)));
#line 59
this.ScenarioSetup(scenarioInfo);
#line 60
testRunner.Given("I have installed Orchard");
#line 61
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 62
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table4.AddRow(new string[] {
                        "Name",
                        "Scott"});
            table4.AddRow(new string[] {
                        "RequestUrlHost",
                        "scott.example.org"});
            table4.AddRow(new string[] {
                        "DataProvider",
                        "SQLite"});
#line 63
testRunner.And("I fill in", ((string)(null)), table4);
#line 68
testRunner.And("I hit \"Save\"");
#line 69
testRunner.And("I am redirected");
#line 70
testRunner.And("I go to \"/Setup\" on host scott.example.org");
#line 71
testRunner.Then("I should see \"Welcome to Orchard\"");
#line 72
testRunner.And("I should see \"Finish Setup\"");
#line 73
testRunner.And("I should not see \"SQLite\"");
#line 74
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A new tenant runs the setup")]
        public virtual void ANewTenantRunsTheSetup()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A new tenant runs the setup", ((string[])(null)));
#line 76
this.ScenarioSetup(scenarioInfo);
#line 77
testRunner.Given("I have installed Orchard");
#line 78
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 79
testRunner.When("I go to \"Admin/MultiTenancy/Add\"");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table5.AddRow(new string[] {
                        "Name",
                        "Scott"});
            table5.AddRow(new string[] {
                        "RequestUrlHost",
                        "scott.example.org"});
#line 80
testRunner.And("I fill in", ((string)(null)), table5);
#line 84
testRunner.And("I hit \"Save\"");
#line 85
testRunner.And("I go to \"/Setup\" on host scott.example.org");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "name",
                        "value"});
            table6.AddRow(new string[] {
                        "SiteName",
                        "Scott Site"});
            table6.AddRow(new string[] {
                        "AdminPassword",
                        "6655321"});
#line 86
testRunner.And("I fill in", ((string)(null)), table6);
#line 90
testRunner.And("I hit \"Finish Setup\"");
#line 91
testRunner.And("I go to \"/Default.aspx\"");
#line 92
testRunner.Then("I should see \"<h1>Scott Site</h1>\"");
#line 93
testRunner.And("I should see \"Welcome, <strong>admin</strong>!\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Listing tenants from command line")]
        public virtual void ListingTenantsFromCommandLine()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Listing tenants from command line", ((string[])(null)));
#line 95
this.ScenarioSetup(scenarioInfo);
#line 96
testRunner.Given("I have installed Orchard");
#line 97
testRunner.And("I have installed \"Orchard.MultiTenancy\"");
#line 98
testRunner.And("I have tenant \"Alpha\" on \"example.org\" as \"New-site-name\"");
#line 99
testRunner.When("I execute >tenant list");
#line 100
testRunner.Then("I should see \"Name: Alpha\"");
#line 101
testRunner.And("I should see \"Request Url Host: example.org\"");
#line hidden
            testRunner.CollectScenarioErrors();
        }
    }
}
#endregion
