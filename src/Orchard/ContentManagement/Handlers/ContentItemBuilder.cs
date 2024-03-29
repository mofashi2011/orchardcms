﻿namespace Orchard.ContentManagement.Handlers {
    public class ContentItemBuilder {
        private readonly ContentItem _item;

        public ContentItemBuilder(string contentType) {
            _item = new ContentItem { ContentType = contentType };
        }

        public ContentItem Build() {
            return _item;
        }

        public ContentItemBuilder Weld<TPart>() where TPart : ContentPart, new() {
            var part = new TPart();
            _item.Weld(part);
            return this;
        }

        public ContentItemBuilder Weld(ContentPart contentPart) {
            _item.Weld(contentPart);
            return this;
        }

    }
}
