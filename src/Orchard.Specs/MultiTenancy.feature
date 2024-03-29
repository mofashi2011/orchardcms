﻿Feature: Multiple tenant management
	In order to host several isolated web applications
	As a root Orchard system operator
	I want to create and manage tenant configurations

Scenario: Default site is listed 
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy"
	Then I should see "List of Site's Tenants"
		And I should see "<span class="tenantName">Default</span>"
		And the status should be 200 OK

Scenario: New tenant fields are required
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I hit "Save"
	Then I should see "is required"

Scenario: A new tenant is created
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
		And I hit "Save"
		And I am redirected
	Then I should see "<span class="tenantName">Scott</span>"
		And the status should be 200 OK
		
Scenario: A new tenant is created with uninitialized state
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
		And I hit "Save"
		And I am redirected
	Then I should see "<li class="tenant Uninitialized">"
		And the status should be 200 OK

Scenario: A new tenant goes to the setup screen
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
		And I hit "Save"
		And I go to "/Setup" on host scott.example.org
	Then I should see "Welcome to Orchard"
		And I should see "Finish Setup"
		And the status should be 200 OK
		
Scenario: A new tenant with preconfigured database goes to the setup screen
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
			| DataProvider | SQLite |
		And I hit "Save"
		And I am redirected
		And I go to "/Setup" on host scott.example.org
	Then I should see "Welcome to Orchard"
		And I should see "Finish Setup"
		And I should not see "SQLite"
		And the status should be 200 OK

Scenario: A new tenant runs the setup
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
		And I hit "Save"
		And I go to "/Setup" on host scott.example.org
		And I fill in 
			| name | value |
			| SiteName | Scott Site |
			| AdminPassword | 6655321 |
		And I hit "Finish Setup"
			And I go to "/Default.aspx"
	Then I should see "<h1>Scott Site</h1>"
		And I should see "Welcome, <strong>admin</strong>!"
		
Scenario: An existing initialized tenant cannot have its database option cleared
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
		And I hit "Save"
		And I go to "/Setup" on host scott.example.org
		And I fill in 
			| name | value |
			| SiteName | Scott Site |
			| AdminPassword | 6655321 |
		And I hit "Finish Setup"
        And I go to "/Admin/MultiTenancy/Edit/Scott" on host localhost
    Then I should see "<h1>Edit Tenant</h1>"
        And I should see "<h2>Scott</h2>"
        And I should not see "Allow the tenant to set up the database"

Scenario: Default tenant cannot be disabled
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy"
    Then I should not see "<form action="/Admin/MultiTenancy/disable""

Scenario: A running tenant can be disabled
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
		And I hit "Save"
		And I go to "/Setup" on host scott.example.org
		And I fill in 
			| name | value |
			| SiteName | Scott Site |
			| AdminPassword | 6655321 |
		And I hit "Finish Setup"
        And I go to "/Admin/MultiTenancy" on host localhost
        And I hit "Suspend"
        And I am redirected
    Then I should see "<form action="/Admin/MultiTenancy/enable""

Scenario: A running tenant which is disabled can be enabled
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
	When I go to "Admin/MultiTenancy/Add"
		And I fill in 
			| name | value |
			| Name | Scott |
			| RequestUrlHost | scott.example.org |
		And I hit "Save"
		And I go to "/Setup" on host scott.example.org
		And I fill in 
			| name | value |
			| SiteName | Scott Site |
			| AdminPassword | 6655321 |
		And I hit "Finish Setup"
        And I go to "/Admin/MultiTenancy" on host localhost
        And I hit "Suspend"
        And I am redirected
        And I hit "Resume"
        And I am redirected
    Then I should see "<form action="/Admin/MultiTenancy/disable""
    
Scenario: Listing tenants from command line
	Given I have installed Orchard
		And I have installed "Orchard.MultiTenancy"
		And I have tenant "Alpha" on "example.org" as "New-site-name"
	When I execute >tenant list
	Then I should see "Name: Alpha"
		And I should see "Request Url Host: example.org"
