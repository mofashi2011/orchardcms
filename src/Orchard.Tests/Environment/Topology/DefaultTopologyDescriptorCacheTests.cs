﻿using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Autofac;
using NUnit.Framework;
using Orchard.Environment.Configuration;
using Orchard.Environment.Topology;
using Orchard.Environment.Topology.Models;

namespace Orchard.Tests.Environment.Topology {
    [TestFixture]
    public class DefaultTopologyDescriptorCacheTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterType<DefaultTopologyDescriptorCache>().As<ITopologyDescriptorCache>();
            _container = builder.Build();
        }

        [Test]
        public void FetchReturnsNullForCacheMiss() {
            var service = _container.Resolve<ITopologyDescriptorCache>();
            var descriptor = service.Fetch("No such shell");
            Assert.That(descriptor, Is.Null);
        }

        [Test]
        public void StoreCanBeCalledMoreThanOnceOnTheSameName() {
            var service = _container.Resolve<ITopologyDescriptorCache>();
            var descriptor = new ShellTopologyDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor);
            service.Store("Hello", descriptor);
            var result = service.Fetch("Hello");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SerialNumber, Is.EqualTo(6655321));
        }

        [Test]
        public void SecondCallUpdatesData() {
            var service = _container.Resolve<ITopologyDescriptorCache>();
            var descriptor1 = new ShellTopologyDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor1);
            var descriptor2 = new ShellTopologyDescriptor { SerialNumber = 42 };
            service.Store("Hello", descriptor2);
            var result = service.Fetch("Hello");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SerialNumber, Is.EqualTo(42));
        }

        [Test]
        public void StoreNullWillClearEntry() {
            var service = _container.Resolve<ITopologyDescriptorCache>();

            var descriptor1 = new ShellTopologyDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor1);
            var result1 = service.Fetch("Hello");
            Assert.That(result1, Is.Not.Null);

            service.Store("Hello", null);
            var result2 = service.Fetch("Hello");
            Assert.That(result2, Is.Null);
        }


        [Test]
        public void AllDataWillRoundTrip() {
            var service = _container.Resolve<ITopologyDescriptorCache>();

            var descriptor = new ShellTopologyDescriptor {
                SerialNumber = 6655321,
                Settings = new ShellSettings {
                    Name = "Testing",
                    DataProvider = "s1",
                    DataConnectionString = "s2",
                    DataPrefix = "s3"
                },
                EnabledFeatures = new[] { 
                    new TopologyFeature {ExtensionName = "f1", FeatureName = "f2"},
                    new TopologyFeature {ExtensionName = "f3", FeatureName = "f4"},
                },
                Parameters = new[] { 
                    new TopologyParameter {Component = "p1", Name = "p2",Value = "p3"}, 
                    new TopologyParameter {Component = "p4",Name = "p5", Value = "p6"},
                },
            };
            var descriptorInfo = descriptor.ToDataString();

            service.Store("Hello", descriptor);
            var result = service.Fetch("Hello");
            var resultInfo = result.ToDataString();

            Assert.That(descriptorInfo, Is.StringContaining("Testing"));
            Assert.That(resultInfo, Is.StringContaining("Testing"));
            Assert.That(descriptorInfo, Is.EqualTo(resultInfo));

        }
    }

    static class DataContractExtensions {
        public static string ToDataString<T>(this T obj) {
            var serializer = new DataContractSerializer(typeof(ShellTopologyDescriptor));
            var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer)) {
                serializer.WriteObject(xmlWriter, obj);
            }
            return writer.ToString();
        }
    }
}


