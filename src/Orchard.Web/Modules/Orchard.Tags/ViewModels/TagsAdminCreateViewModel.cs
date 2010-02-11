﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.Mvc.ViewModels;

namespace Orchard.Tags.ViewModels {
    public class TagsAdminCreateViewModel : AdminViewModel {
        [Required, DisplayName("Name:")]
        public string TagName { get; set; }
    }
}
