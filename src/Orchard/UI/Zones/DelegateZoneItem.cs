using System;
using System.Web.Mvc;

namespace Orchard.UI.Zones {
    public class DelegateZoneItem : ZoneItem {
        public Action<HtmlHelper> Action { get; set; }

        public override void Execute<TModel>(HtmlHelper<TModel> html) {
            Action(html);
        }
    }
}