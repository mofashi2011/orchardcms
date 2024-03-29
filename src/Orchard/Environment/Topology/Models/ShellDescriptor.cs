﻿using System.Collections.Generic;
using System.Linq;

namespace Orchard.Environment.Topology.Models {

    /// <summary>
    /// Contains a snapshot of a tenant's enabled features.
    /// The information is drawn out of the shell via IShellDescriptorManager
    /// and cached by the host via IShellDescriptorCache. It is
    /// passed to the ICompositionStrategy to build the ShellTopology.
    /// </summary>
    public class ShellDescriptor {
        public ShellDescriptor() {
            EnabledFeatures = Enumerable.Empty<ShellFeature>();
            Parameters = Enumerable.Empty<ShellParameter>();
        }

        public int SerialNumber { get; set; }
        public IEnumerable<ShellFeature> EnabledFeatures { get; set; }
        public IEnumerable<ShellParameter> Parameters { get; set; }
    }

    public class ShellFeature {
        public string Name { get; set; }
    }

    public class ShellParameter {
        public string Component { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
