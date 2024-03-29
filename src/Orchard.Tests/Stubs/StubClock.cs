﻿using System;
using Orchard.Services;

namespace Orchard.Tests.Stubs {
    public class StubClock : IClock {
        public StubClock() {
            UtcNow = new DateTime(2009, 10, 14, 12, 34, 56, DateTimeKind.Utc);
        }

        public DateTime UtcNow { get; private set; }

        public void Advance(TimeSpan span) {
            UtcNow = UtcNow.Add(span);
        }

        public DateTime FutureMoment(TimeSpan span) {
            return UtcNow.Add(span);
        }
    }
}
