﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;

namespace Orchard.Sandbox.Models {
    [UsedImplicitly]
    public class SandboxContentHandler : ContentHandler {
        public override IEnumerable<ContentType> GetContentTypes() {
            return new[] { SandboxPage.ContentType };
        }

        public SandboxContentHandler(
            IRepository<SandboxPageRecord> pageRepository,
            IRepository<SandboxSettingsRecord> settingsRepository) {

            // define the "sandboxpage" content type
            Filters.Add(new ActivatingFilter<SandboxPage>(SandboxPage.ContentType.Name));
            Filters.Add(new ActivatingFilter<CommonAspect>(SandboxPage.ContentType.Name));
            Filters.Add(new ActivatingFilter<RoutableAspect>(SandboxPage.ContentType.Name));
            Filters.Add(new ActivatingFilter<BodyAspect>(SandboxPage.ContentType.Name));
            Filters.Add(new StorageFilter<SandboxPageRecord>(pageRepository) { AutomaticallyCreateMissingRecord = true });



            // add settings to site, and simple record-template gui
            Filters.Add(new ActivatingFilter<ContentPart<SandboxSettingsRecord>>("site"));
            Filters.Add(new StorageFilter<SandboxSettingsRecord>(settingsRepository) { AutomaticallyCreateMissingRecord = true });
            Filters.Add(new TemplateFilterForRecord<SandboxSettingsRecord>("SandboxSettings", "Parts/Sandbox.SiteSettings"));

        }
    }
}
