﻿using System.Collections.Generic;
using Orchard.Mvc.ViewModels;
using Orchard.Themes;

namespace Orchard.Core.Themes.ViewModels {
    public class ThemesIndexViewModel : BaseViewModel {
        public ITheme CurrentTheme { get; set; }
        public IEnumerable<ITheme> Themes { get; set; }
    }
}
