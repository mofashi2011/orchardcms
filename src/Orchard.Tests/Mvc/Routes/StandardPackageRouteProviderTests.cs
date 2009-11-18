﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using NUnit.Framework;
using Orchard.Mvc.Routes;
using Orchard.Packages;

namespace Orchard.Tests.Mvc.Routes {
    [TestFixture]
    public class StandardPackageRouteProviderTests {
        [Test]
        public void PackageDisplayNameShouldBeUsedInBothStandardRoutes() {
            var stubManager = new StubPackageManager();
            var routeProvider = new StandardPackageRouteProvider(stubManager);

            var routes = new List<RouteDescriptor>();
            routeProvider.GetRoutes(routes);

            Assert.That(routes, Has.Count.EqualTo(4));
            var fooAdmin = routes.Select(x => x.Route).OfType<Route>()
                .Single(x => x.Url == "Admin/Foo/{action}/{id}");
            var fooRoute = routes.Select(x => x.Route).OfType<Route>()
                .Single(x => x.Url == "Foo/{controller}/{action}/{id}");
            var barAdmin = routes.Select(x => x.Route).OfType<Route>()
                .Single(x => x.Url == "Admin/Bar/{action}/{id}");
            var barRoute = routes.Select(x => x.Route).OfType<Route>()
                .Single(x => x.Url == "Bar/{controller}/{action}/{id}");

            Assert.That(fooAdmin.DataTokens["area"], Is.EqualTo("Long.Name.Foo"));
            Assert.That(fooRoute.DataTokens["area"], Is.EqualTo("Long.Name.Foo"));
            Assert.That(barAdmin.DataTokens["area"], Is.EqualTo("Long.Name.Bar"));
            Assert.That(barRoute.DataTokens["area"], Is.EqualTo("Long.Name.Bar"));
        }

        public class StubPackageManager : IPackageManager {
            public IEnumerable<PackageDescriptor> AvailablePackages() {
                throw new NotImplementedException();
            }

            public IEnumerable<PackageEntry> ActivePackages() {
                yield return new PackageEntry {
                    Descriptor = new PackageDescriptor {
                        Name = "Long.Name.Foo",
                        DisplayName = "Foo",
                    }
                };
                yield return new PackageEntry {
                    Descriptor = new PackageDescriptor {
                        Name = "Long.Name.Bar",
                        DisplayName = "Bar",
                    }
                };
            }
        }
    }
}