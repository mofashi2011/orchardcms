using Orchard.Blogs.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class BlogViewModel : BaseViewModel {
        public ItemDisplayModel<Blog> Blog { get; set; }
    }
}