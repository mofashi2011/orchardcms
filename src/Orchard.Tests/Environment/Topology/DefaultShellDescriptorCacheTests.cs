﻿using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Autofac;
using NUnit.Framework;
using Orchard.Environment.Topology;
using Orchard.Environment.Topology.Models;
using Orchard.FileSystems.AppData;

namespace Orchard.Tests.Environment.Topology {
    [TestFixture]
    public class DefaultShellDescriptorCacheTests {
        private IContainer _container;
        private string _tempFolder;
        private IAppDataFolder _appDataFolder;

        [SetUp]
        public void Init() {
            _tempFolder = Path.GetTempFileName();
            File.Delete(_tempFolder);
            Directory.CreateDirectory(_tempFolder);
            _appDataFolder = new AppDataFolder();
            _appDataFolder.SetBasePath(_tempFolder);
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_appDataFolder).As<IAppDataFolder>();
            builder.RegisterType<ShellDescriptorCache>().As<IShellDescriptorCache>();
            _container = builder.Build();
        }

        [TearDown]
        public void Term() {
            Directory.Delete(_tempFolder, true);
        }

        [Test]
        public void FetchReturnsNullForCacheMiss() {
            var service = _container.Resolve<IShellDescriptorCache>();
            var descriptor = service.Fetch("No such shell");
            Assert.That(descriptor, Is.Null);
        }

        [Test]
        public void StoreCanBeCalledMoreThanOnceOnTheSameName() {
            var service = _container.Resolve<IShellDescriptorCache>();
            var descriptor = new ShellDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor);
            service.Store("Hello", descriptor);
            var result = service.Fetch("Hello");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SerialNumber, Is.EqualTo(6655321));
        }

        [Test]
        public void SecondCallUpdatesData() {
            var service = _container.Resolve<IShellDescriptorCache>();
            var descriptor1 = new ShellDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor1);
            var descriptor2 = new ShellDescriptor { SerialNumber = 42 };
            service.Store("Hello", descriptor2);
            var result = service.Fetch("Hello");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SerialNumber, Is.EqualTo(42));
        }

        [Test]
        public void StoreNullWillClearEntry() {
            var service = _container.Resolve<IShellDescriptorCache>();

            var descriptor1 = new ShellDescriptor { SerialNumber = 6655321 };
            service.Store("Hello", descriptor1);
            var result1 = service.Fetch("Hello");
            Assert.That(result1, Is.Not.Null);

            service.Store("Hello", null);
            var result2 = service.Fetch("Hello");
            Assert.That(result2, Is.Null);
        }


        [Test]
        public void AllDataWillRoundTrip() {
            var service = _container.Resolve<IShellDescriptorCache>();

            var descriptor = new ShellDescriptor {
                SerialNumber = 6655321,
                EnabledFeatures = new[] { 
                    new ShellFeature { Name = "f2"},
                    new ShellFeature { Name = "f4"},
                },
                Parameters = new[] { 
                    new ShellParameter {Component = "p1", Name = "p2",Value = "p3"}, 
                    new ShellParameter {Component = "p4",Name = "p5", Value = "p6"},
                },
            };
            var descriptorInfo = descriptor.ToDataString();

            service.Store("Hello", descriptor);
            var result = service.Fetch("Hello");
            var resultInfo = result.ToDataString();

            Assert.That(descriptorInfo, Is.StringContaining("6655321"));
            Assert.That(resultInfo, Is.StringContaining("6655321"));
            Assert.That(descriptorInfo, Is.EqualTo(resultInfo));

        }
    }

    static class DataContractExtensions {
        public static string ToDataString<T>(this T obj) {
            var serializer = new DataContractSerializer(typeof(ShellDescriptor));
            var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer)) {
                serializer.WriteObject(xmlWriter, obj);
            }
            return writer.ToString();
        }
    }
}


