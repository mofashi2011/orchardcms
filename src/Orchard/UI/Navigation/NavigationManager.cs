﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace Orchard.UI.Navigation {
    public class NavigationManager : INavigationManager {
        private readonly IEnumerable<INavigationProvider> _providers;
        private readonly IAuthorizationService _authorizationService;
        private readonly UrlHelper _urlHelper;

        public NavigationManager(IEnumerable<INavigationProvider> providers, IAuthorizationService authorizationService, UrlHelper urlHelper) {
            _providers = providers;
            _authorizationService = authorizationService;
            _urlHelper = urlHelper;
        }

        protected virtual IUser CurrentUser { get; [UsedImplicitly] private set; }

        public IEnumerable<MenuItem> BuildMenu(string menuName) {
            return FinishMenu(Crop(Reduce(Merge(AllSources(menuName)))).ToArray());
        }

        private IEnumerable<MenuItem> FinishMenu(IEnumerable<MenuItem> menuItems) {
            foreach (var menuItem in menuItems) {
                menuItem.Href = GetUrl(menuItem.Url, menuItem.RouteValues);
                menuItem.Items = FinishMenu(menuItem.Items.ToArray());
            }

            return menuItems;
        }

        public string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary) {
            var url = string.IsNullOrEmpty(menuItemUrl) && (routeValueDictionary == null || routeValueDictionary.Count == 0)
                          ? "~/"
                          : !string.IsNullOrEmpty(menuItemUrl)
                                ? menuItemUrl
                                : _urlHelper.RouteUrl(routeValueDictionary);

            if (!string.IsNullOrEmpty(url) && _urlHelper.RequestContext.HttpContext != null &&
                !(url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/"))) {
                if (url.StartsWith("~/")) {
                    url = url.Substring(2);
                }
                var appPath = _urlHelper.RequestContext.HttpContext.Request.ApplicationPath;
                if (appPath == "/")
                    appPath = "";
                url = string.Format("{0}/{1}", appPath, url);
            }
            return url;
        }

        private IEnumerable<MenuItem> Crop(IEnumerable<MenuItem> items) {
            foreach(var item in items) {
                if (item.Items.Any() || item.RouteValues != null)
                    yield return item;
            }
        }

        private IEnumerable<MenuItem> Reduce(IEnumerable<MenuItem> items) {
            var hasDebugShowAllMenuItems = _authorizationService.TryCheckAccess(Permission.Named("DebugShowAllMenuItems"), CurrentUser, null);
            foreach (var item in items) {
                if (hasDebugShowAllMenuItems ||
                    !item.Permissions.Any() ||
                    item.Permissions.Any(x => _authorizationService.TryCheckAccess(x, CurrentUser, null))) {
                    yield return new MenuItem {
                        Items = Reduce(item.Items),
                        Permissions = item.Permissions,
                        Position = item.Position,
                        RouteValues = item.RouteValues,
                        Text = item.Text,
                        Url = item.Url,
                        Href = item.Href
                    };
                }
            }
        }

        private IEnumerable<IEnumerable<MenuItem>> AllSources(string menuName) {
            foreach (var provider in _providers) {
                if (provider.MenuName == menuName) {
                    var builder = new NavigationBuilder();
                    provider.GetNavigation(builder);
                    yield return builder.Build();
                }
            }
        }

        private static IEnumerable<MenuItem> Merge(IEnumerable<IEnumerable<MenuItem>> sources) {
            var comparer = new MenuItemComparer();
            var orderer = new PositionComparer();

            return sources.SelectMany(x => x).ToArray()
                .GroupBy(key => key, (key, items) => Join(items), comparer)
                .OrderBy(item => item.Position, orderer);
        }

        static MenuItem Join(IEnumerable<MenuItem> items) {
            if (items.Count() < 2)
                return items.Single();

            var joined = new MenuItem {
                Text = items.First().Text,
                Url = items.First().Url,
                Href = items.First().Href,
                RouteValues = items.First().RouteValues,
                Items = Merge(items.Select(x => x.Items)).ToArray(),
                Position = SelectBestPositionValue(items.Select(x => x.Position)),
                Permissions = items.SelectMany(x => x.Permissions)
            };
            return joined;
        }

        private static string SelectBestPositionValue(IEnumerable<string> positions) {
            var comparer = new PositionComparer();
            return positions.Aggregate(string.Empty,
                                       (agg, pos) =>
                                       string.IsNullOrEmpty(agg)
                                           ? pos
                                           : string.IsNullOrEmpty(pos)
                                                 ? agg
                                                 : comparer.Compare(agg, pos) < 0 ? agg : pos);
        }
    }
}