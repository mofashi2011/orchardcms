using System.ComponentModel.DataAnnotations;
using Orchard.Mvc.ViewModels;
using Orchard.Security;
using Orchard.Users.Models;

namespace Orchard.Users.ViewModels {
    public class UserCreateViewModel : BaseViewModel {
        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public ContentItemViewModel<IUser> User { get; set; }
    }
}