﻿using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.Core.Dashboard {
    public class AdminMenu : INavigationProvider {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add("Orchard", "0",
                        menu => menu.Add("Dashboard", "0", item => item.Action("Index", "Admin", new { area = "Dashboard" }).Permission(StandardPermissions.AccessAdminPanel)));
        }
    }
}