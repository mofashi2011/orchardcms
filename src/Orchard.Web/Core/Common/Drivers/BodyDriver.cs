﻿using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.ViewModels;

namespace Orchard.Core.Common.Drivers {
    [UsedImplicitly]
    public class BodyDriver : ContentPartDriver<BodyAspect> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Parts/Common.Body";
        private const string DefaultTextEditorTemplate = "TinyMceTextEditor";

        public BodyDriver(IOrchardServices services) {
            Services = services;
        }

        protected override string Prefix {
            get { return "Body"; }
        }

        // \/\/ Haackalicious on many accounts - don't copy what has been done here for the wrapper \/\/
        protected override DriverResult Display(BodyAspect part, string displayType) {
            var model = new BodyDisplayViewModel { BodyAspect = part, Text = BbcodeReplace(part.Text) };

            return Combined(
                Services.Authorizer.Authorize(Permissions.ChangeOwner) ? ContentPartTemplate(model, "Parts/Common.Body.ManageWrapperPre").Location("primary", "5") : null,
                Services.Authorizer.Authorize(Permissions.ChangeOwner) ? ContentPartTemplate(model, "Parts/Common.Body.Manage").Location("primary", "5") : null,
                ContentPartTemplate(model, TemplateName, Prefix).LongestMatch(displayType, "Summary", "SummaryAdmin").Location("primary", "5"),
                Services.Authorizer.Authorize(Permissions.ChangeOwner) ? ContentPartTemplate(model, "Parts/Common.Body.ManageWrapperPost").Location("primary", "5") : null);
        }
        
        protected override DriverResult Editor(BodyAspect part) {
            var model = BuildEditorViewModel(part);
            return ContentPartTemplate(model, TemplateName, Prefix).Location("primary", "5");
        }

        protected override DriverResult Editor(BodyAspect part, IUpdateModel updater) {
            var model = BuildEditorViewModel(part);
            updater.TryUpdateModel(model, Prefix, null, null);
            return ContentPartTemplate(model, TemplateName, Prefix).Location("primary", "5");
        }

        private static BodyEditorViewModel BuildEditorViewModel(BodyAspect part) {
            return new BodyEditorViewModel {
                BodyAspect = part,
                TextEditorTemplate = DefaultTextEditorTemplate,
                AddMediaPath= new PathBuilder(part).AddContentType().AddContainerSlug().AddSlug().ToString()
            };
        }
        class PathBuilder {
            private readonly IContent _content;
            private string _path;

            public PathBuilder(IContent content) {
                _content = content;
                _path = "";
            }

            public override string ToString() {
                return _path;
            }

            public PathBuilder AddContentType() {
                Add(_content.ContentItem.ContentType);
                return this;
            }

            public PathBuilder AddContainerSlug() {
                var common = _content.As<ICommonAspect>();
                if (common == null)
                    return this;

                var routable = common.Container.As<RoutableAspect>();
                if (routable == null)
                    return this;

                Add(routable.Slug);
                return this;
            }

            public PathBuilder AddSlug() {
                var routable = _content.As<RoutableAspect>();
                if (routable == null)
                    return this;

                Add(routable.Slug);
                return this;
            }

            private void Add(string segment) {
                if (string.IsNullOrEmpty(segment))
                    return;
                if (string.IsNullOrEmpty(_path))
                    _path = segment;
                else
                    _path = _path + "/" + segment;
            }

        }


        // Can be moved somewhere else once we have IoC enabled body text filters.
        private static string BbcodeReplace(string bodyText) {

            if ( string.IsNullOrEmpty(bodyText) )
                return string.Empty;

            Regex urlRegex = new Regex(@"\[url\]([^\]]+)\[\/url\]");
            Regex urlRegexWithLink = new Regex(@"\[url=([^\]]+)\]([^\]]+)\[\/url\]");
            Regex imgRegex = new Regex(@"\[img\]([^\]]+)\[\/img\]");

            bodyText = urlRegex.Replace(bodyText, "<a href=\"$1\">$1</a>");
            bodyText = urlRegexWithLink.Replace(bodyText, "<a href=\"$1\">$2</a>");
            bodyText = imgRegex.Replace(bodyText, "<img src=\"$1\" />");

            return bodyText;
        }
    }
}