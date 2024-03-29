using Orchard.Blogs.Models;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class CreateBlogViewModel : BaseViewModel {
        public ContentItemViewModel<Blog> Blog { get; set; }
        public bool PromoteToHomePage { get; set; }
    }
}