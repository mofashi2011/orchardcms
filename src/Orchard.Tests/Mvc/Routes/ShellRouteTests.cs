﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Web;
using NUnit.Framework;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Mvc.Routes;
using Orchard.Tests.Stubs;
using Orchard.Tests.Utility;

namespace Orchard.Tests.Mvc.Routes {
    [TestFixture]
    public class ShellRouteTests {
        private RouteCollection _routes;
        private ILifetimeScope _containerA;
        private ILifetimeScope _containerB;
        private ShellSettings _settingsA;
        private ShellSettings _settingsB;
        private IContainer _rootContainer;

        [SetUp]
        public void Init() {
            _settingsA = new ShellSettings { Name = "Alpha" };
            _settingsB = new ShellSettings { Name = "Beta", };
            _routes = new RouteCollection();

            var rootBuilder = new ContainerBuilder();
            rootBuilder.Register(ctx => _routes);
            rootBuilder.RegisterType<ShellRoute>().InstancePerDependency();
            rootBuilder.RegisterType<RunningShellTable>().As<IRunningShellTable>().SingleInstance();

            _rootContainer = rootBuilder.Build();

            _containerA = _rootContainer.BeginLifetimeScope(
                builder => {
                    builder.Register(ctx => _settingsA);
                    builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().InstancePerLifetimeScope();
                });

            _containerB = _rootContainer.BeginLifetimeScope(
                builder => {
                    builder.Register(ctx => _settingsB);
                    builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().InstancePerLifetimeScope();
                });
        }

        [Test]
        public void FactoryMethodWillCreateShellRoutes() {
            var settings = new ShellSettings { Name = "Alpha" };
            var builder = new ContainerBuilder();
            builder.RegisterType<ShellRoute>().InstancePerDependency();
            builder.RegisterAutoMocking();
            builder.Register(ctx => settings);

            var container = builder.Build();
            var buildShellRoute = container.Resolve<Func<RouteBase, ShellRoute>>();

            var routeA = new Route("foo", new MvcRouteHandler());
            var route1 = buildShellRoute(routeA);

            var routeB = new Route("bar", new MvcRouteHandler()) {
                DataTokens = new RouteValueDictionary { { "area", "Beta" } }
            };
            var route2 = buildShellRoute(routeB);

            Assert.That(route1, Is.Not.SameAs(route2));

            Assert.That(route1.ShellSettingsName, Is.EqualTo("Alpha"));
            Assert.That(route1.Area, Is.Null);

            Assert.That(route2.ShellSettingsName, Is.EqualTo("Alpha"));
            Assert.That(route2.Area, Is.EqualTo("Beta"));
        }


        [Test]
        public void RoutePublisherReplacesOnlyNamedShellsRoutes() {

            var routeA = new Route("foo", new MvcRouteHandler());
            var routeB = new Route("bar", new MvcRouteHandler());
            var routeC = new Route("quux", new MvcRouteHandler());

            _containerA.Resolve<IRoutePublisher>().Publish(
                new[] {new RouteDescriptor {Priority = 0, Route = routeA}});

            _containerB.Resolve<IRoutePublisher>().Publish(
                new[] {new RouteDescriptor {Priority = 0, Route = routeB}});

            Assert.That(_routes.Count(), Is.EqualTo(2));

            _containerA.Resolve<IRoutePublisher>().Publish(
                new[] {new RouteDescriptor {Priority = 0, Route = routeC}});

            Assert.That(_routes.Count(), Is.EqualTo(2));

            _containerB.Resolve<IRoutePublisher>().Publish(
                new[] {
                          new RouteDescriptor {Priority = 0, Route = routeA},
                          new RouteDescriptor {Priority = 0, Route = routeB},
                      });

            Assert.That(_routes.Count(), Is.EqualTo(3));
        }

        [Test]
        public void MatchingRouteToActiveShellTableWillLimitTheAbilityToMatchRoutes() {
   
            var routeFoo = new Route("foo", new MvcRouteHandler());

            _settingsA.RequestUrlHost = "a.example.com";
            _containerA.Resolve<IRoutePublisher>().Publish(
                new[] {new RouteDescriptor {Priority = 0, Route = routeFoo}});
            _rootContainer.Resolve<IRunningShellTable>().Add(_settingsA);

            _settingsB.RequestUrlHost = "b.example.com";
            _containerB.Resolve<IRoutePublisher>().Publish(
                new[] {new RouteDescriptor {Priority = 0, Route = routeFoo}});
            _rootContainer.Resolve<IRunningShellTable>().Add(_settingsB);

            var httpContext = new StubHttpContext("~/foo");
            var routeData = _routes.GetRouteData(httpContext);
            Assert.That(routeData, Is.Null);

            var httpContextA = new StubHttpContext("~/foo", "a.example.com");
            var routeDataA = _routes.GetRouteData(httpContextA);
            Assert.That(routeDataA, Is.Not.Null);
            Assert.That(routeDataA.DataTokens.ContainsKey("IContainerProvider"), Is.True);
            var routeContainerProviderA = (IContainerProvider)routeDataA.DataTokens["IContainerProvider"];
            Assert.That(routeContainerProviderA.ApplicationContainer.Resolve<IRoutePublisher>(), Is.SameAs(_containerA.Resolve<IRoutePublisher>()));
            Assert.That(routeContainerProviderA.ApplicationContainer.Resolve<IRoutePublisher>(), Is.Not.SameAs(_containerB.Resolve<IRoutePublisher>()));

            var httpContextB = new StubHttpContext("~/foo", "b.example.com");
            var routeDataB = _routes.GetRouteData(httpContextB);
            Assert.That(routeDataB, Is.Not.Null);
            Assert.That(routeDataB.DataTokens.ContainsKey("IContainerProvider"), Is.True);
            var routeContainerProviderB = (IContainerProvider)routeDataB.DataTokens["IContainerProvider"];
            Assert.That(routeContainerProviderB.ApplicationContainer.Resolve<IRoutePublisher>(), Is.SameAs(_containerB.Resolve<IRoutePublisher>()));
            Assert.That(routeContainerProviderB.ApplicationContainer.Resolve<IRoutePublisher>(), Is.Not.SameAs(_containerA.Resolve<IRoutePublisher>()));
        }
        
    }
}