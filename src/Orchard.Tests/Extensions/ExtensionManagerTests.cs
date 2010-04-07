﻿using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Orchard.Extensions;
using Yaml.Grammar;

namespace Orchard.Tests.Extensions {
    [TestFixture]
    public class ExtensionManagerTests {
        private IContainer _container;
        private IExtensionManager _manager;
        private StubFolders _folders;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            _folders = new StubFolders();
            builder.RegisterInstance(_folders).As<IExtensionFolders>();
            builder.RegisterType<ExtensionManager>().As<IExtensionManager>();
            _container = builder.Build();
            _manager = _container.Resolve<IExtensionManager>();
        }

        public class StubFolders : IExtensionFolders {
            public StubFolders() {
                Manifests = new Dictionary<string, string>();
            }

            public IDictionary<string, string> Manifests { get; set; }

            public IEnumerable<string> ListNames() {
                return Manifests.Keys;
            }

            public ParseResult ParseManifest(string name) {
                var parser = new YamlParser();
                bool success;
                var stream = parser.ParseYamlStream(new TextInput(Manifests[name]), out success);
                if (success) {
                    return new ParseResult {
                        Location = "~/InMemory",
                        Name = name,
                        YamlDocument = stream.Documents.Single()
                    };
                }
                return null;
            }
        }


        [Test]
        public void AvailableExtensionsShouldFollowCatalogLocations() {
            _folders.Manifests.Add("foo", "name: Foo");
            _folders.Manifests.Add("bar", "name: Bar");
            _folders.Manifests.Add("frap", "name: Frap");
            _folders.Manifests.Add("quad", "name: Quad");

            var available = _manager.AvailableExtensions();

            Assert.That(available.Count(), Is.EqualTo(4));
            Assert.That(available, Has.Some.Property("Name").EqualTo("foo"));
        }

        [Test]
        public void ExtensionDescriptorsShouldHaveNameAndVersion() {

            _folders.Manifests.Add("Sample", @"
name: Sample Extension
version: 2.x
");

            var descriptor = _manager.AvailableExtensions().Single();
            Assert.That(descriptor.Name, Is.EqualTo("Sample"));
            Assert.That(descriptor.DisplayName, Is.EqualTo("Sample Extension"));
            Assert.That(descriptor.Version, Is.EqualTo("2.x"));
        }

        [Test]
        public void ExtensionDescriptorsShouldBeParsedForMinimalModuleTxt() {

            _folders.Manifests.Add("SuperWiki", @"
name: SuperWiki
version: 1.0.3
orchardversion: 1
features:
  SuperWiki: 
    Description: My super wiki module for Orchard.
");

            var descriptor = _manager.AvailableExtensions().Single();
            Assert.That(descriptor.Name, Is.EqualTo("SuperWiki"));
            Assert.That(descriptor.Version, Is.EqualTo("1.0.3"));
            Assert.That(descriptor.OrchardVersion, Is.EqualTo("1"));
            Assert.That(descriptor.Features.Count(), Is.EqualTo(1));
            Assert.That(descriptor.Features.First().Name, Is.EqualTo("SuperWiki"));
            Assert.That(descriptor.Features.First().ExtensionName, Is.EqualTo("SuperWiki"));
            Assert.That(descriptor.Features.First().Description, Is.EqualTo("My super wiki module for Orchard."));
        }

        [Test]
        public void ExtensionDescriptorsShouldBeParsedForCompleteModuleTxt() {

            _folders.Manifests.Add("AnotherWiki", @"
name: AnotherWiki
author: Coder Notaprogrammer
website: http://anotherwiki.codeplex.com
version: 1.2.3
orchardversion: 1
features:
  AnotherWiki: 
    Description: My super wiki module for Orchard.
    Dependencies: Versioning, Search
    Category: Content types
  AnotherWiki Editor:
    Description: A rich editor for wiki contents.
    Dependencies: TinyMCE, AnotherWiki
    Category: Input methods
  AnotherWiki DistributionList:
    Description: Sends e-mail alerts when wiki contents gets published.
    Dependencies: AnotherWiki, Email Subscriptions
    Category: Email
  AnotherWiki Captcha:
    Description: Kills spam. Or makes it zombie-like.
    Dependencies: AnotherWiki, reCaptcha
    Category: Spam
");

            var descriptor = _manager.AvailableExtensions().Single();
            Assert.That(descriptor.Name, Is.EqualTo("AnotherWiki"));
            Assert.That(descriptor.Author, Is.EqualTo("Coder Notaprogrammer"));
            Assert.That(descriptor.WebSite, Is.EqualTo("http://anotherwiki.codeplex.com"));
            Assert.That(descriptor.Version, Is.EqualTo("1.2.3"));
            Assert.That(descriptor.OrchardVersion, Is.EqualTo("1"));
            Assert.That(descriptor.Features.Count(), Is.EqualTo(4));
            foreach (var featureDescriptor in descriptor.Features) {
                switch (featureDescriptor.Name) {
                    case "AnotherWiki":
                        Assert.That(featureDescriptor.ExtensionName, Is.EqualTo("AnotherWiki"));
                        Assert.That(featureDescriptor.Description, Is.EqualTo("My super wiki module for Orchard."));
                        Assert.That(featureDescriptor.Category, Is.EqualTo("Content types"));
                        Assert.That(featureDescriptor.Dependencies.Count(), Is.EqualTo(2));
                        Assert.That(featureDescriptor.Dependencies.Contains("Versioning"));
                        Assert.That(featureDescriptor.Dependencies.Contains("Search"));
                        break;
                    case "AnotherWiki Editor":
                        Assert.That(featureDescriptor.ExtensionName, Is.EqualTo("AnotherWiki"));
                        Assert.That(featureDescriptor.Description, Is.EqualTo("A rich editor for wiki contents."));
                        Assert.That(featureDescriptor.Category, Is.EqualTo("Input methods"));
                        Assert.That(featureDescriptor.Dependencies.Count(), Is.EqualTo(2));
                        Assert.That(featureDescriptor.Dependencies.Contains("TinyMCE"));
                        Assert.That(featureDescriptor.Dependencies.Contains("AnotherWiki"));
                        break;
                    case "AnotherWiki DistributionList":
                        Assert.That(featureDescriptor.ExtensionName, Is.EqualTo("AnotherWiki"));
                        Assert.That(featureDescriptor.Description, Is.EqualTo("Sends e-mail alerts when wiki contents gets published."));
                        Assert.That(featureDescriptor.Category, Is.EqualTo("Email"));
                        Assert.That(featureDescriptor.Dependencies.Count(), Is.EqualTo(2));
                        Assert.That(featureDescriptor.Dependencies.Contains("AnotherWiki"));
                        Assert.That(featureDescriptor.Dependencies.Contains("Email Subscriptions"));
                        break;
                    case "AnotherWiki Captcha":
                        Assert.That(featureDescriptor.ExtensionName, Is.EqualTo("AnotherWiki"));
                        Assert.That(featureDescriptor.Description, Is.EqualTo("Kills spam. Or makes it zombie-like."));
                        Assert.That(featureDescriptor.Category, Is.EqualTo("Spam"));
                        Assert.That(featureDescriptor.Dependencies.Count(), Is.EqualTo(2));
                        Assert.That(featureDescriptor.Dependencies.Contains("AnotherWiki"));
                        Assert.That(featureDescriptor.Dependencies.Contains("reCaptcha"));
                        break;
                    default:
                        Assert.Fail("Features not parsed correctly");
                        break;
                }
            }
        }

        [Test]
        public void ExtensionManagerShouldReturnTopology() {
            var topology = _manager.GetExtensionsTopology();

            Assert.That(topology.Count(), Is.Not.EqualTo(0));
        }

        [Test]
        public void ExtensionManagerTopologyShouldContainNonAbstractClasses() {
            var topology = _manager.GetExtensionsTopology();

            foreach (var type in topology) {
                Assert.That(type.IsClass);
                Assert.That(!type.IsAbstract);
            }
        }
    }
}
