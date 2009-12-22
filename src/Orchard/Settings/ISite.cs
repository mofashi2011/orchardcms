﻿using Orchard.ContentManagement;

namespace Orchard.Settings {
    /// <summary>
    /// Interface provided by the "settings" model. 
    /// </summary>
    public interface ISite : IContent {
        string PageTitleSeparator { get; }
        string SiteName { get; }
        string SuperUser { get; }
    }
}
