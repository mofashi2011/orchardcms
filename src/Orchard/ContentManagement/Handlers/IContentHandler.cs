﻿using System.Collections.Generic;
using Orchard.Events;

namespace Orchard.ContentManagement.Handlers {
    public interface IContentHandler : IEvents {
        IEnumerable<ContentType> GetContentTypes();

        void Activating(ActivatingContentContext context);
        void Activated(ActivatedContentContext context);
        void Creating(CreateContentContext context);
        void Created(CreateContentContext context);
        void Loading(LoadContentContext context);
        void Loaded(LoadContentContext context);
        void Versioning(VersionContentContext context);
        void Versioned(VersionContentContext context);
        void Publishing(PublishContentContext context);
        void Published(PublishContentContext context);
        void Removing(RemoveContentContext context);
        void Removed(RemoveContentContext context);

        void GetContentItemMetadata(GetContentItemMetadataContext context);
        void BuildDisplayModel(BuildDisplayModelContext context);
        void BuildEditorModel(BuildEditorModelContext context);
        void UpdateEditorModel(UpdateEditorModelContext context);
    }
}
