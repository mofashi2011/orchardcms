using System.Collections.Generic;
using Orchard.Blogs.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class AdminBlogsViewModel : AdminViewModel {
        public IEnumerable<ItemDisplayModel<Blog>> Blogs { get; set; }
    }
}