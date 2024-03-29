using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Orchard.Core.Common.Models {
    public class BodyRecord : ContentPartVersionRecord {
        [StringLengthMax]
        public virtual string Text { get; set; }

        public virtual string Format { get; set; }
    }
}