﻿using System.Collections.Generic;
using Orchard.Mvc.ViewModels;
using Orchard.Roles.Records;

namespace Orchard.Roles.ViewModels {
    public class RolesIndexViewModel : AdminViewModel {
        public IList<RoleRecord> Rows { get; set; }
    }
}
