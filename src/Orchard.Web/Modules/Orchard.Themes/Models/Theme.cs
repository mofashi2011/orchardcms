﻿using Orchard.ContentManagement;

namespace Orchard.Themes.Models {
    public class Theme : ContentPart<ThemeRecord>, ITheme {
        public static readonly ContentType ContentType = new ContentType { Name = "theme", DisplayName = "Themes" };

        public string ThemeName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string HomePage { get; set; }
        public string Tags { get; set; }
    }
}