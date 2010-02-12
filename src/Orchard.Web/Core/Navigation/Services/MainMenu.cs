﻿using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Records;
using Orchard.UI.Navigation;
using MenuItem=Orchard.Core.Navigation.Models.MenuItem;

namespace Orchard.Core.Navigation.Services {
    public class MainMenu : INavigationProvider {
        private readonly IContentManager _contentManager;

        public MainMenu(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public string MenuName { get { return "mainmenu"; } }

        public void GetNavigation(NavigationBuilder builder) {
            IEnumerable<MenuPart> menuParts = _contentManager.Query<MenuPart, MenuPartRecord>().Where(x => x.OnMainMenu).List();
            foreach (var menuPart in menuParts) {
                if (menuPart != null ) {
                    MenuPart part = menuPart;

                    if (part.Is<MenuItem>())
                        builder.Add(menu => menu.Add(part.MenuText, part.MenuPosition, part.As<MenuItem>().Url));
                    else
                        builder.Add(
                            menu =>
                            menu.Add(part.MenuText, part.MenuPosition,
                                     nib =>
                                     nib.Action(
                                         part.ContentItem.ContentManager.GetItemMetadata(part.ContentItem).
                                             DisplayRouteValues["action"] as string,
                                         part.ContentItem.ContentManager.GetItemMetadata(part.ContentItem).
                                             DisplayRouteValues["controller"] as string,
                                         part.ContentItem.ContentManager.GetItemMetadata(part.ContentItem).
                                             DisplayRouteValues)));
                }
            }
        }
    }
}
