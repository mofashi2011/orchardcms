using System.Collections.Generic;
using System.Web.Routing;

namespace Orchard.UI.Navigation {
    public interface INavigationManager : IDependency {
        IEnumerable<MenuItem> BuildMenu(string menuName);
        string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary);
    }
}