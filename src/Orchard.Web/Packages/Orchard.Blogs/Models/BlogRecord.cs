using Orchard.Data.Conventions;
using Orchard.ContentManagement.Records;

namespace Orchard.Blogs.Models {
    public class BlogRecord : ContentPartRecord {
        [CascadeAllDeleteOrphan]
        public virtual string Description { get; set; }
        //public virtual bool Enabled { get; set; }
        public virtual int PostCount { get; set; }
    }
}