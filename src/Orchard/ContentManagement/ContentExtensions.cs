﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.Records;
using Orchard.Mvc.ViewModels;

namespace Orchard.ContentManagement {
    public static class ContentCreateExtensions {

        /* Item creation extension methods */

        public static T New<T>(this IContentManager manager, string contentType) where T : class, IContent {
            var contentItem = manager.New(contentType);
            if (contentItem == null)
                return null;

            var part = contentItem.Get<T>();
            if (part == null)
                throw new InvalidCastException();

            return part;
        }

        public static void Create(this IContentManager manager, IContent content) {
            manager.Create(content.ContentItem, VersionOptions.Draft);
            manager.Publish(content.ContentItem);
        }

        public static void Create(this IContentManager manager, IContent content, VersionOptions options) {
            manager.Create(content.ContentItem, options);
        }

        public static ContentItem Create(this IContentManager manager, string contentType) {
            return manager.Create<ContentItem>(contentType, init => { });
        }

        public static T Create<T>(this IContentManager manager, string contentType) where T : class, IContent {
            return manager.Create<T>(contentType, init => { });
        }

        public static T Create<T>(this IContentManager manager, string contentType, Action<T> initialize) where T : class, IContent {
            var content = manager.New<T>(contentType);
            if (content == null)
                return null;

            initialize(content);
            manager.Create(content.ContentItem);
            return content;
        }

        public static ContentItem Create(this IContentManager manager, string contentType, VersionOptions options) {
            return manager.Create<ContentItem>(contentType, options, init => { });
        }

        public static T Create<T>(this IContentManager manager, string contentType, VersionOptions options) where T : class, IContent {
            return manager.Create<T>(contentType, options, init => { });
        }

        public static T Create<T>(this IContentManager manager, string contentType, VersionOptions options, Action<T> initialize) where T : class, IContent {
            var content = manager.New<T>(contentType);
            if (content == null)
                return null;

            initialize(content);
            manager.Create(content.ContentItem, options);
            return content;
        }
    }

    public static class ContentQueryExtensions {

        /* Query related extension methods */

        public static IContentQuery<TPart> Query<TPart>(this IContentManager manager)
            where TPart : ContentPart {
            return manager.Query().ForPart<TPart>();
        }
        public static IContentQuery<TPart, TRecord> Query<TPart, TRecord>(this IContentManager manager)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord {
            return manager.Query().ForPart<TPart>().Join<TRecord>();
        }

        /* Query(VersionOptions options) */

        public static IContentQuery<ContentItem> Query(this IContentManager manager, VersionOptions options) {
            return manager.Query().ForVersion(options);
        }

        public static IContentQuery<TPart> Query<TPart>(this IContentManager manager, VersionOptions options) where TPart : ContentPart {
            return manager.Query().ForPart<TPart>().ForVersion(options);
        }

        public static IContentQuery<TPart, TRecord> Query<TPart, TRecord>(this IContentManager manager, VersionOptions options)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord {
            return manager.Query().ForPart<TPart>().ForVersion(options).Join<TRecord>();
        }

        public static IContentQuery<ContentItem> Query(this IContentManager manager, VersionOptions options, params string[] contentTypeNames) {
            return manager.Query().ForVersion(options).ForType(contentTypeNames);
        }
        public static IContentQuery<TPart> Query<TPart>(this IContentManager manager, VersionOptions options, params string[] contentTypeNames) where TPart : ContentPart {
            return manager.Query().ForPart<TPart>().ForVersion(options).ForType(contentTypeNames);
        }

        /* Query(params string[] contentTypeNames) */

        public static IContentQuery<ContentItem> Query(this IContentManager manager, params string[] contentTypeNames) {
            return manager.Query().ForType(contentTypeNames);
        }
        public static IContentQuery<TPart> Query<TPart>(this IContentManager manager, params string[] contentTypeNames) where TPart : ContentPart {
            return manager.Query().ForPart<TPart>().ForType(contentTypeNames);
        }
        public static IContentQuery<TPart, TRecord> Query<TPart, TRecord>(this IContentManager manager, params string[] contentTypeNames)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord {
            return manager.Query().ForPart<TPart>().ForType(contentTypeNames).Join<TRecord>();
        }



        public static IEnumerable<T> List<T>(this IContentManager manager, params string[] contentTypeNames) where T : ContentPart {
            return manager.Query<T>(contentTypeNames).List();
        }

        public static IEnumerable<T> List<T>(this IContentQuery query) where T : IContent {
            return query.ForPart<T>().List();
        }

        public static IEnumerable<T> Slice<T>(this IContentQuery<T> query, int count) where T : IContent {
            return query.Slice(0, count);
        }

    }

    public static class ContentGetExtensions {

        public static ContentItem GetLatest(this IContentManager manager, int id) {
            return manager.Get(id, VersionOptions.Latest);
        }
        public static ContentItem GetDraftRequired(this IContentManager manager, int id) {
            return manager.Get(id, VersionOptions.DraftRequired);
        }

        public static T Get<T>(this IContentManager manager, int id) where T : class, IContent {
            var contentItem = manager.Get(id);
            return contentItem == null ? null : contentItem.Get<T>();
        }
        public static T Get<T>(this IContentManager manager, int id, VersionOptions options) where T : class, IContent {
            var contentItem = manager.Get(id, options);
            return contentItem == null ? null : contentItem.Get<T>();
        }
        public static T GetLatest<T>(this IContentManager manager, int id) where T : class, IContent {
            return Get<T>(manager, id, VersionOptions.Latest);
        }
        public static T GetDraftRequired<T>(this IContentManager manager, int id) where T : class, IContent {
            return Get<T>(manager, id, VersionOptions.DraftRequired);
        }
        
    }

    public static class ContentExtensions {


        /* Display and editor convenience extension methods */

        public static ContentItemViewModel<T> BuildDisplayModel<T>(this IContentManager manager, int id, string displayType) where T : class, IContent {
            var content = manager.Get<T>(id);
            if (content == null)
                return null;
            return manager.BuildDisplayModel(content, displayType);
        }

        public static ContentItemViewModel<T> BuildEditorModel<T>(this IContentManager manager, int id) where T : class, IContent {
            var content = manager.Get<T>(id);
            if (content == null)
                return null;
            return manager.BuildEditorModel(content);

        }

        public static ContentItemViewModel<T> UpdateEditorModel<T>(this IContentManager manager, int id, IUpdateModel updater) where T : class, IContent {
            var content = manager.Get<T>(id);
            if (content == null)
                return null;
            return manager.UpdateEditorModel(content, updater);
        }




        /* Aggregate item/part type casting extension methods */

        public static bool Is<T>(this IContent content) {
            return content == null ? false : content.ContentItem.Has(typeof(T));
        }
        public static T As<T>(this IContent content) where T : IContent {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }

        public static bool Has<T>(this IContent content) {
            return content == null ? false : content.ContentItem.Has(typeof(T));
        }
        public static T Get<T>(this IContent content) where T : IContent {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }


        public static IEnumerable<T> AsPart<T>(this IEnumerable<ContentItem> items) where T : IContent {
            return items == null ? null : items.Where(item => item.Is<T>()).Select(item => item.As<T>());
        }

    }
}
