﻿using System.Collections.Generic;
using System.Linq;
using Orchard.Security.Permissions;

namespace Orchard.Media {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageMediaFiles = new Permission { Description = "Modifying Media Files", Name = "ManageMediaFiles" };
        public static readonly Permission UploadMediaFiles = new Permission { Description = "Uploading Media Files", Name = "UploadMediaFiles", ImpliedBy = new[] { ManageMediaFiles } };

        public string PackageName {
            get {
                return "Media";
            }
        }

        public IEnumerable<Permission> GetPermissions() {
            return new Permission[] {
                ManageMediaFiles,
                UploadMediaFiles,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrators",
                    Permissions = new[] {ManageMediaFiles}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {ManageMediaFiles}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    //Permissions = new[] {}
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManageMediaFiles}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {UploadMediaFiles}
                },
            };
        }

    }
}