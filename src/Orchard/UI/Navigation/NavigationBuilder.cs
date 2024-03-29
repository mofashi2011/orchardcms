using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.UI.Navigation {
    public class NavigationBuilder {
        IEnumerable<MenuItem> Contained { get; set; }

        public NavigationBuilder Add(string caption, string position, Action<NavigationItemBuilder> itemBuilder) {
            var childBuilder = new NavigationItemBuilder();

            if (!string.IsNullOrEmpty(caption))
                childBuilder.Caption(caption);

            if (!string.IsNullOrEmpty(position))
                childBuilder.Position(position);

            itemBuilder(childBuilder);
            Contained = (Contained ?? Enumerable.Empty<MenuItem>()).Concat(childBuilder.Build());
            return this;
        }

        public NavigationBuilder Add(string caption, Action<NavigationItemBuilder> itemBuilder) {
            return Add(caption, null, itemBuilder);
        }
        public NavigationBuilder Add(Action<NavigationItemBuilder> itemBuilder) {
            return Add(null, null, itemBuilder);
        }
        public NavigationBuilder Add(string caption, string position) {
            return Add(caption, position, x=> { });
        }
        public NavigationBuilder Add(string caption) {
            return Add(caption, null, x => { });
        }

        public IEnumerable<MenuItem> Build() {
            return (Contained ?? Enumerable.Empty<MenuItem>()).ToList();
        }
    }
}