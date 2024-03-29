﻿
using Orchard.UI.Navigation;

namespace Orchard.MetaData {
    public class AdminMenu : INavigationProvider {

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add("Content Types", "5",
                        menu => menu
                                    .Add("Content Types", "1.0", item => item.Action("ContentTypeList", "MetaData", new { area = "Orchard.MetaData" }).Permission(Permissions.ManageMetaData))
                                    );
        }

        
    }
}