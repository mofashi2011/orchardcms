﻿using Orchard.Core.Feeds.Models;

namespace Orchard.Core.Feeds {
    public interface IFeedQueryProvider : IEvents {
        FeedQueryMatch Match(FeedContext context);
    }

    public class FeedQueryMatch {
        public int Priority { get; set; }
        public IFeedQuery FeedQuery { get; set; }
    }
}
