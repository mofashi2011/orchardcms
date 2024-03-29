using System;
using Orchard.ContentManagement.Records;

namespace Orchard.Core.Common.Models {
    public class CommonVersionRecord : ContentPartVersionRecord {
        public virtual DateTime? CreatedUtc { get; set; }
        public virtual DateTime? PublishedUtc { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}