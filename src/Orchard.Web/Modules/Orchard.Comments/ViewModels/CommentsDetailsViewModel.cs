﻿using System.Collections.Generic;
using Orchard.Mvc.ViewModels;

namespace Orchard.Comments.ViewModels {
    public class CommentsDetailsViewModel : BaseViewModel {
        public IList<CommentEntry> Comments { get; set; }
        public CommentDetailsOptions Options { get; set; }
        public string DisplayNameForCommentedItem { get; set; }
        public int CommentedItemId { get; set; }
        public bool CommentsClosedOnItem { get; set; }
    }

    public class CommentDetailsOptions {
        public CommentDetailsFilter Filter { get; set; }
        public CommentDetailsBulkAction BulkAction { get; set; }
    }

    public enum CommentDetailsBulkAction {
        None,
        Pend,
        Approve,
        MarkAsSpam,
        Delete,
    }

    public enum CommentDetailsFilter {
        All,
        Pending,
        Approved,
        Spam,
    }
}
