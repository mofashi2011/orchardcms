using System.Collections.Generic;
using Orchard.Blogs.Models;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class BlogsViewModel : BaseViewModel {
        public IEnumerable<ContentItemViewModel<Blog>> Blogs { get; set; }
    }
}