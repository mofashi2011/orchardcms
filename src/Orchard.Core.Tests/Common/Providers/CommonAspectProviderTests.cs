﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData.Records;
using Orchard.Core.Common;
using Orchard.Core.Common.Handlers;
using Orchard.Core.Common.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Records;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Tests.Modules;
using Orchard.Mvc.ViewModels;
using Orchard.Core.Common.ViewModels;
using System.Web.Mvc;

namespace Orchard.Core.Tests.Common.Providers {
    [TestFixture]
    public class CommonAspectProviderTests : DatabaseEnabledTestsBase {
        private Mock<IAuthenticationService> _authn;
        private Mock<IAuthorizationService> _authz;
        private Mock<IMembershipService> _membership;

        public override void Register(ContainerBuilder builder) {
            builder.RegisterType<DefaultContentManager>().As<IContentManager>();
            builder.RegisterType<DefaultContentManagerSession>().As<IContentManagerSession>();
            builder.RegisterType<TestHandler>().As<IContentHandler>();
            builder.RegisterType<CommonAspectHandler>().As<IContentHandler>();

            _authn = new Mock<IAuthenticationService>();
            _authz = new Mock<IAuthorizationService>();
            _membership = new Mock<IMembershipService>();

            builder.RegisterInstance(_authn.Object);
            builder.RegisterInstance(_authz.Object);
            builder.RegisterInstance(_membership.Object);

        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                return new[] {
                                 typeof(ContentTypeRecord), 
                                 typeof(ContentTypePartRecord), 
                                 typeof(ContentTypePartNameRecord),
                                 typeof(ContentItemRecord), 
                                 typeof(ContentItemVersionRecord), 
                                 typeof(CommonRecord),
                                 typeof(CommonVersionRecord),
                             };
            }
        }

        [UsedImplicitly]
        class TestHandler : ContentHandler {
            public TestHandler() {
                Filters.Add(new ActivatingFilter<CommonAspect>("test-item"));
                Filters.Add(new ActivatingFilter<ContentPart<CommonVersionRecord>>("test-item"));
                Filters.Add(new ActivatingFilter<TestUser>("user"));
            }
        }

        class TestUser : ContentPart, IUser {
            public int Id { get { return 6655321; } }
            public string UserName {get { return "x"; }}
            public string Email { get { return "y"; } }
        }

        [Test]
        public void OwnerShouldBeNullAndZeroByDefault() {
            var contentManager = _container.Resolve<IContentManager>();
            var item = contentManager.Create<CommonAspect>("test-item", init => { });
            ClearSession();

            Assert.That(item.Owner, Is.Null);
            Assert.That(item.Record.OwnerId, Is.EqualTo(0));
        }

        [Test]
        public void PublishingShouldFailIfOwnerIsUnknown()
        {
            var contentManager = _container.Resolve<IContentManager>();
            var updateModel = new Mock<IUpdateModel>();

            var user = contentManager.New<IUser>("user");
            _authn.Setup(x => x.GetAuthenticatedUser()).Returns(user);

            var createUtc = _clock.UtcNow;
            var item = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });
            var viewModel = new OwnerEditorViewModel() { Owner = "user" };
            updateModel.Setup(x => x.TryUpdateModel(viewModel, "", null, null)).Returns(true);
            contentManager.UpdateEditorModel(item.ContentItem, updateModel.Object);
        }

        class UpdatModelStub : IUpdateModel {

            ModelStateDictionary _modelState = new ModelStateDictionary();

            public ModelStateDictionary ModelErrors
            {
                get { return _modelState; }
            }
           
            public string Owner { get; set; }
            
            public bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class {
                (model as OwnerEditorViewModel).Owner = Owner;
                return true;
            }

            public void AddModelError(string key, LocalizedString errorMessage) {
                _modelState.AddModelError(key, errorMessage.ToString());
            }
        }

        [Test]
        public void PublishingShouldNotThrowExceptionIfOwnerIsNull()
        {
            var contentManager = _container.Resolve<IContentManager>();

            var item = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });

            var user = contentManager.New<IUser>("user");
            _authn.Setup(x => x.GetAuthenticatedUser()).Returns(user);
            _authz.Setup(x => x.TryCheckAccess(Permissions.ChangeOwner, user, item)).Returns(true);

            item.Owner = user;

            var updater = new UpdatModelStub() { Owner = null };

            contentManager.UpdateEditorModel(item.ContentItem, updater);
        }

        [Test]
        public void PublishingShouldFailIfOwnerIsEmpty()
        {
            var contentManager = _container.Resolve<IContentManager>();

            var item = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });
            
            var user = contentManager.New<IUser>("user");
            _authn.Setup(x => x.GetAuthenticatedUser()).Returns(user);
            _authz.Setup(x => x.TryCheckAccess(Permissions.ChangeOwner, user, item)).Returns(true);

            item.Owner = user;

            var updater = new UpdatModelStub() {Owner = ""};

            contentManager.UpdateEditorModel(item.ContentItem, updater);

            Assert.That(updater.ModelErrors.ContainsKey("CommonAspect.Owner"), Is.True);
        }

        [Test]
        public void PublishingShouldSetPublishUtc()
        {
            var contentManager = _container.Resolve<IContentManager>();

            var createUtc = _clock.UtcNow;
            var item = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });

            Assert.That(item.CreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item.PublishedUtc, Is.Null);

            _clock.Advance(TimeSpan.FromMinutes(1));
            var publishUtc = _clock.UtcNow;

            contentManager.Publish(item.ContentItem);

            Assert.That(item.CreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item.PublishedUtc, Is.EqualTo(publishUtc));
        }


        [Test]
        public void VersioningItemShouldCreatedAndPublishedUtcValuesPerVersion() {
            var contentManager = _container.Resolve<IContentManager>();

            var createUtc = _clock.UtcNow;
            var item1 = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });

            Assert.That(item1.CreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item1.PublishedUtc, Is.Null);

            _clock.Advance(TimeSpan.FromMinutes(1));
            var publish1Utc = _clock.UtcNow;
            contentManager.Publish(item1.ContentItem);

            // db records need to be updated before demanding draft as item2 below
            _session.Flush();

            _clock.Advance(TimeSpan.FromMinutes(1));
            var draftUtc = _clock.UtcNow;
            var item2 = contentManager.GetDraftRequired<ICommonAspect>(item1.ContentItem.Id);

            _clock.Advance(TimeSpan.FromMinutes(1));
            var publish2Utc = _clock.UtcNow;
            contentManager.Publish(item2.ContentItem);

            // both instances non-versioned dates show it was created upfront
            Assert.That(item1.CreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item2.CreatedUtc, Is.EqualTo(createUtc));

            // both instances non-versioned dates show the most recent publish
            Assert.That(item1.PublishedUtc, Is.EqualTo(publish2Utc));
            Assert.That(item2.PublishedUtc, Is.EqualTo(publish2Utc));

            // version1 versioned dates show create was upfront and publish was oldest
            Assert.That(item1.VersionCreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item1.VersionPublishedUtc, Is.EqualTo(publish1Utc));

            // version2 versioned dates show create was midway and publish was most recent
            Assert.That(item2.VersionCreatedUtc, Is.EqualTo(draftUtc));
            Assert.That(item2.VersionPublishedUtc, Is.EqualTo(publish2Utc));
        }

        [Test]
        public void UnpublishShouldClearFlagButLeaveMostrecentPublishDatesIntact() {
            var contentManager = _container.Resolve<IContentManager>();

            var createUtc = _clock.UtcNow;
            var item = contentManager.Create<ICommonAspect>("test-item", VersionOptions.Draft, init => { });

            Assert.That(item.CreatedUtc, Is.EqualTo(createUtc));
            Assert.That(item.PublishedUtc, Is.Null);

            _clock.Advance(TimeSpan.FromMinutes(1));
            var publishUtc = _clock.UtcNow;
            contentManager.Publish(item.ContentItem);

            // db records need to be updated before seeking by published flags
            _session.Flush();

            _clock.Advance(TimeSpan.FromMinutes(1));
            var unpublishUtc = _clock.UtcNow;
            contentManager.Unpublish(item.ContentItem);

            // db records need to be updated before seeking by published flags
            _session.Flush();
            _session.Clear();

            var publishedItem = contentManager.Get<ICommonAspect>(item.ContentItem.Id, VersionOptions.Published);
            var latestItem = contentManager.Get<ICommonAspect>(item.ContentItem.Id, VersionOptions.Latest);
            var draftItem = contentManager.Get<ICommonAspect>(item.ContentItem.Id, VersionOptions.Draft);
            var allVersions = contentManager.GetAllVersions(item.ContentItem.Id);

            Assert.That(publishedItem, Is.Null);
            Assert.That(latestItem, Is.Not.Null);
            Assert.That(draftItem, Is.Not.Null);
            Assert.That(allVersions.Count(), Is.EqualTo(1));
            Assert.That(publishUtc, Is.Not.EqualTo(unpublishUtc));
            Assert.That(latestItem.PublishedUtc, Is.EqualTo(publishUtc));
            Assert.That(latestItem.VersionPublishedUtc, Is.EqualTo(publishUtc));
            Assert.That(latestItem.ContentItem.VersionRecord.Latest, Is.True);
            Assert.That(latestItem.ContentItem.VersionRecord.Published, Is.False);
        }
    }
}
