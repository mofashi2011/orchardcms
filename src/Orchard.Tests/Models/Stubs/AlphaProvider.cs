﻿using System.Collections.Generic;
using Orchard.Models;
using Orchard.Models.Driver;
using Orchard.UI.Models;

namespace Orchard.Tests.Models.Stubs {
    public class AlphaProvider : ContentProvider {
        public AlphaProvider() {
            OnGetDisplays<Alpha>((ctx, part) => ctx.Displays.Add(new ModelTemplate(part) { Position = "3" }));
        }
        public override IEnumerable<ContentType> GetContentTypes() {
            return new[] { new ContentType { Name = "alpha" } };
        }
        protected override void Activating(ActivatingContentContext context) {
            if (context.ContentType == "alpha") {
                context.Builder.Weld<Alpha>();
            }
        }
    }
}
