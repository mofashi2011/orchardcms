﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Pages.Drivers;
using Orchard.Pages.Models;
using Orchard.ContentManagement;
using Orchard.Pages.Routing;
using Orchard.Tasks.Scheduling;

namespace Orchard.Pages.Services {
    [UsedImplicitly]
    public class PageService : IPageService {
        private readonly IContentManager _contentManager;
        private readonly IPublishingTaskManager _publishingTaskManager;
        private readonly IPageSlugConstraint _pageSlugConstraint;

        public PageService(IContentManager contentManager, IPublishingTaskManager publishingTaskManager, IPageSlugConstraint pageSlugConstraint) {
            _contentManager = contentManager;
            _publishingTaskManager = publishingTaskManager;
            _pageSlugConstraint = pageSlugConstraint;
        }

        public int GetCount() {
            //TODO: (erikpo) Need to add a count method to IContentQuery so it doesn't need to pull out all pages to get a count
            return _contentManager.Query<Page>(VersionOptions.Latest).List().Count();
        }

        public IEnumerable<Page> Get() {
            return Get(PageStatus.All);
        }

        public IEnumerable<Page> Get(PageStatus status) {
            switch (status) {
                case PageStatus.All:
                    return _contentManager.Query(PageDriver.ContentType.Name).Join<RoutableRecord>().ForVersion(VersionOptions.Latest).List().AsPart<Page>();
                case PageStatus.Published:
                    return _contentManager.Query(PageDriver.ContentType.Name).Join<RoutableRecord>().ForVersion(VersionOptions.Published).List().AsPart<Page>();
                case PageStatus.Offline:
                    return _contentManager.Query(PageDriver.ContentType.Name).Join<RoutableRecord>().ForVersion(VersionOptions.Latest).Where<ContentPartVersionRecord>(ci => !ci.ContentItemVersionRecord.Published).List().AsPart<Page>();
                default:
                    return Enumerable.Empty<Page>();
            }
        }

        public Page Get(int id) {
            return _contentManager.Get<Page>(id);
        }

        public Page Get(string slug) {
            return
                _contentManager.Query(PageDriver.ContentType.Name).Join<RoutableRecord>().Where(rr => rr.Slug == slug).List().FirstOrDefault
                    ().As<Page>();
        }

        public Page GetLatest(int id) {
            return _contentManager.Get<Page>(id, VersionOptions.Latest);
        }

        public Page GetLatest(string slug) {
            return
                _contentManager.Query(VersionOptions.Latest, PageDriver.ContentType.Name).Join<RoutableRecord>().Where(rr => rr.Slug == slug)
                    .Slice(0, 1).FirstOrDefault().As<Page>();
        }

        public Page GetPageOrDraft(int id)  {
            return _contentManager.GetDraftRequired<Page>(id);
        }

        public Page GetPageOrDraft(string slug) {
            Page page = GetLatest(slug);
            return _contentManager.GetDraftRequired<Page>(page.Id);
        }

        public void Delete(Page page) {
            _publishingTaskManager.DeleteTasks(page.ContentItem);
            _contentManager.Remove(page.ContentItem);
        }

        public void Publish(Page page) {
            _publishingTaskManager.DeleteTasks(page.ContentItem);
            _contentManager.Publish(page.ContentItem);
            _pageSlugConstraint.AddSlug(page.Slug);
        }

        public void Publish(Page page, DateTime scheduledPublishUtc) {
            _publishingTaskManager.Publish(page.ContentItem, scheduledPublishUtc);
        }

        public void Unpublish(Page page) {
            _contentManager.Unpublish(page.ContentItem);
            _pageSlugConstraint.RemoveSlug(page.Slug);
        }

        public DateTime? GetScheduledPublishUtc(Page page) {
            var task = _publishingTaskManager.GetPublishTask(page.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }
    }
}