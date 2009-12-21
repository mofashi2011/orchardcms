using Orchard.Blogs.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class CreateBlogViewModel : AdminViewModel {
        public ItemEditorModel<Blog> Blog { get; set; }
    }
}