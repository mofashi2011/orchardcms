﻿using System.Collections.Generic;
using System.Web;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Environment.Extensions {
    public interface IExtensionManager {
        IEnumerable<ExtensionDescriptor> AvailableExtensions();
        IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors);
        void InstallExtension(string extensionType, HttpPostedFileBase extensionBundle);
        void UninstallExtension(string extensionType, string extensionName);

        // Used by the HackInstallationGenerator and will be removed along with it.
        IEnumerable<ExtensionEntry> ActiveExtensions_Obsolete();
    }
}