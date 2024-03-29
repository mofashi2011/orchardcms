﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Core.Common.Services;
using Orchard.Localization;
using Orchard.Core.Common.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Pages.Drivers;
using Orchard.Pages.Models;
using Orchard.Pages.Services;
using Orchard.UI.Notify;

namespace Orchard.Pages.Handlers {
    [UsedImplicitly]
    public class PageHandler : ContentHandler {
        private readonly IPageService _pageService;
        private readonly IOrchardServices _orchardServices;

        public PageHandler(IPageService pageService, IOrchardServices orchardServices) {
            _pageService = pageService;
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<Page>(PageDriver.ContentType.Name));
            Filters.Add(new ActivatingFilter<CommonAspect>(PageDriver.ContentType.Name));
            Filters.Add(new ActivatingFilter<ContentPart<CommonVersionRecord>>(PageDriver.ContentType.Name));
            Filters.Add(new ActivatingFilter<RoutableAspect>(PageDriver.ContentType.Name));
            Filters.Add(new ActivatingFilter<BodyAspect>(PageDriver.ContentType.Name));

            OnLoaded<Page>((context, page) => page._scheduledPublishUtc.Loader(value => _pageService.GetScheduledPublishUtc(page)));
        }

        Localizer T { get; set; }
    }
}