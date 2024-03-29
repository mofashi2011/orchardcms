﻿using System.Collections.Generic;
using System.Linq;
using Orchard.Security.Permissions;

namespace Orchard.Pages {
    public class Permissions : IPermissionProvider {
        public static readonly Permission PublishOthersPages = new Permission { Description = "Publish or unpublish page for others", Name = "PublishOthersPages" };
        public static readonly Permission PublishPages = new Permission { Description = "Publish or unpublish page", Name = "PublishPages", ImpliedBy = new[] { PublishOthersPages } };
        public static readonly Permission EditOthersPages = new Permission { Description = "Edit page for others", Name = "EditOthersPages", ImpliedBy = new[] { PublishOthersPages } };
        public static readonly Permission EditPages = new Permission { Description = "Edit page", Name = "EditPages", ImpliedBy = new[] { EditOthersPages, PublishPages } };
        public static readonly Permission DeleteOthersPages = new Permission { Description = "Delete page for others", Name = "DeleteOthersPages" };
        public static readonly Permission DeletePages = new Permission { Description = "Delete page", Name = "DeletePages", ImpliedBy = new[] { DeleteOthersPages } };

        public static readonly Permission MetaListPages = new Permission { ImpliedBy = new[] { EditPages, PublishPages, DeletePages } };

        public string ModuleName {
            get {
                return "Pages";
            }
        }

        public IEnumerable<Permission> GetPermissions() {
            return new Permission[] {
                EditPages,
                EditOthersPages,
                PublishPages,
                PublishOthersPages,
                DeletePages,
                DeleteOthersPages,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {PublishOthersPages,EditOthersPages,DeleteOthersPages}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {PublishOthersPages,EditOthersPages,DeleteOthersPages}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    //Permissions = new[] {}
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {PublishPages,EditPages,DeletePages}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {EditPages}
                },
            };
        }

    }
}