﻿using System.Collections.Generic;
using Orchard.Security.Permissions;

namespace Orchard.Themes {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageThemes = new Permission { Description = "Manage Themes", Name = "ManageThemes" };
        public static readonly Permission ApplyTheme = new Permission { Description = "Apply a Theme", Name = "ApplyTheme" };

        public string ModuleName {
            get {
                return "Themes";
            }
        }

        public IEnumerable<Permission> GetPermissions() {
            return new Permission[] {
                                        ManageThemes,
                                        ApplyTheme,
                                    };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                             new PermissionStereotype {
                                                          Name = "Administrator",
                                                          Permissions = new[] {ManageThemes, ApplyTheme}
                                                      },
                         };
        }
    }
}