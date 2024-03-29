﻿using System;
using System.Web;
using JetBrains.Annotations;
using Orchard.Comments.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Settings;
using Orchard.UI.Notify;
using Joel.Net;

namespace Orchard.Comments.Services {
    //This uses an akismet api implementation from http://akismetapi.codeplex.com/ 
    //Since the implementation is trivial, it may make sense to implement it to reduce dependencies.
    [UsedImplicitly]
    public class AkismetCommentValidator : ICommentValidator {
        private readonly INotifier _notifer;
        public AkismetCommentValidator(INotifier notifier) {
            _notifer = notifier;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        protected virtual ISite CurrentSite { get; [UsedImplicitly] private set; }

        public bool ValidateComment(Comment comment) {
            CommentSettingsRecord commentSettingsRecord = CurrentSite.As<CommentSettings>().Record;
            string akismetKey = commentSettingsRecord.AkismetKey;
            string akismetUrl = commentSettingsRecord.AkismetUrl;
            bool enableSpamProtection = commentSettingsRecord.EnableSpamProtection;
            if (enableSpamProtection == false) {
                return true;
            }
            if (String.IsNullOrEmpty(akismetKey)) {
                _notifer.Information(T("Please configure your Akismet key for spam protection"));
                return true;
            }
            if (String.IsNullOrEmpty(akismetUrl)) {
                akismetUrl = "http://www.orchardproject.net";
            }
            Akismet akismetApi = new Akismet(akismetKey, akismetUrl, null);
            AkismetComment akismetComment = new AkismetComment {
                CommentAuthor = comment.Record.Author,
                CommentAuthorEmail = comment.Record.Email,
                Blog = comment.Record.SiteName,
                CommentAuthorUrl = comment.Record.SiteName,
                CommentContent = comment.Record.CommentText,
                UserAgent = HttpContext.Current.Request.UserAgent,
            };

            if (akismetApi.VerifyKey()) {
                return akismetApi.CommentCheck(akismetComment);
            }

            return false;
        }
    }
}