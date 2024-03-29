﻿using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace Orchard.Core.Common.Models {
    public class RoutableRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string Title { get; set; }

        public virtual string Slug { get; set; }
    }
}