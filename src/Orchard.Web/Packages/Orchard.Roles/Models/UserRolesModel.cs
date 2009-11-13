using System.Collections.Generic;
using Orchard.Models;

namespace Orchard.Roles.Models.NoRecord {
    public interface IUserRoles {
        IList<string> Roles { get; }
    }

    public class UserRolesModel : ModelPart, IUserRoles {
        public UserRolesModel() {
            Roles = new List<string>();
        }

        public IList<string> Roles { get; set; }
    }
}