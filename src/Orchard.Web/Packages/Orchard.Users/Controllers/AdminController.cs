using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Models;
using Orchard.Models.Driver;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Controllers {

    public class AdminController : Controller, IUpdateModel {
        private readonly IMembershipService _membershipService;
        private readonly IContentManager _contentManager;
        private readonly IRepository<UserRecord> _userRepository;
        private readonly INotifier _notifier;

        public AdminController(
            IMembershipService membershipService,
            IContentManager contentManager,
            IRepository<UserRecord> userRepository,
            INotifier notifier) {
            _membershipService = membershipService;
            _contentManager = contentManager;
            _userRepository = userRepository;
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        public IUser CurrentUser { get; set; }

        public Localizer T { get; set; }

        public ActionResult Index() {
            var model = new UsersIndexViewModel();

            var users = _contentManager.Query<User, UserRecord>("user")
                .Where(x => x.UserName != null)
                .List();

            model.Rows = users.Select(x => new UsersIndexViewModel.Row { User = x }).ToList();

            return View(model);
        }

        public ActionResult Create() {
            var user = _contentManager.New("user");
            var model = new UserCreateViewModel {
                ItemView = _contentManager.GetEditorViewModel(user, null)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserCreateViewModel model) {

            if (model.Password != model.ConfirmPassword) {
                ModelState.AddModelError("ConfirmPassword", T("Password confirmation must match").ToString());
            }
            if (ModelState.IsValid == false) {
                model.ItemView = _contentManager.UpdateEditorViewModel(_contentManager.New("user"), null, this);
                return View(model);
            }
            var user = _membershipService.CreateUser(new CreateUserParams(
                                              model.UserName,
                                              model.Password,
                                              model.Email,
                                              null, null, true));
            model.ItemView = _contentManager.UpdateEditorViewModel(user, null, this);
            if (ModelState.IsValid == false) {
                //TODO: rollback transaction
                return View(model);
            }

            return RedirectToAction("edit", new { user.Id });
        }

        public ActionResult Edit(int id) {
            var model = new UserEditViewModel { User = _contentManager.Get<User>(id) };
            model.ItemView = _contentManager.GetEditorViewModel(model.User.ContentItem, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection input) {
            var model = new UserEditViewModel { User = _contentManager.Get<User>(id) };
            model.ItemView = _contentManager.UpdateEditorViewModel(model.User.ContentItem, null, this);

            if (!TryUpdateModel(model, input.ToValueProvider())) {
                return View(model);
            }

            _notifier.Information(T("User information updated"));
            return RedirectToAction("Edit", new { id });
        }


        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }
        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }

}
