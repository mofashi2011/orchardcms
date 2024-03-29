using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Orchard.Mvc.ViewModels;

namespace Orchard.Themes {
    public class ThemeFilter : FilterProvider, IActionFilter, IResultFilter {


        public void OnActionExecuting(ActionExecutingContext filterContext) {
            var attribute = GetThemedAttribute(filterContext.ActionDescriptor);
            if (attribute != null && attribute.Enabled) {
                Apply(filterContext.RequestContext);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;

            var model = viewResult.ViewData.Model as BaseViewModel;
            if (model == null)
                return;

            Apply(filterContext.RequestContext);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
            
        }

        public static void Apply(RequestContext context) {
            // the value isn't important
            context.HttpContext.Items[typeof(ThemeFilter)] = null;
        }

        public static bool IsApplied(RequestContext context) {
            return context.HttpContext.Items.Contains(typeof(ThemeFilter));
        }


        private static ThemedAttribute GetThemedAttribute(ActionDescriptor descriptor) {
            return descriptor.GetCustomAttributes(typeof(ThemedAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(ThemedAttribute), true))
                .OfType<ThemedAttribute>()
                .FirstOrDefault();
        }

    }
}