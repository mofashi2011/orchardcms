﻿using System.Collections.Generic;
using Orchard.Security.Permissions;

namespace Orchard.MultiTenancy {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageTenants = new Permission { Description = "Modifying Tenants of a Site", Name = "ManageTenants" };

        public string ModuleName {
            get {
                return "MultiTenancy";
            }
        }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageTenants
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageTenants}
                },
            };
        }

    }
}