﻿namespace Orchard.Roles.Models {
    public class UserRolesRecord {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual RoleRecord Role { get; set; }
    }
}