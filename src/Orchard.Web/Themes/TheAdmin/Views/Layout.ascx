﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<BaseViewModel>" %>
<%@ Import Namespace="Orchard.Mvc.ViewModels"%>
<%@ Import Namespace="Orchard.Mvc.Html"%><%
Html.RegisterStyle("site.css");
Html.RegisterStyle("ie6.css").WithCondition("if lte IE 6").ForMedia("screen, projection");
Model.Zones.AddRenderPartial("header", "Header", Model);
Model.Zones.AddRenderPartial("header:after", "User", Model); // todo: (heskew) should be a user display or widget
Model.Zones.AddRenderPartial("menu", "Menu", Model);
%>
<div id="header" role="banner"><% Html.Zone("header"); %></div>
<div id="content">
    <div id="navshortcut"><a href="#menu"><%=_Encoded("Skip to navigation") %></a></div>
    <div id="main" role="main"><% Html.ZoneBody("content"); %></div>
    <div id="menu"><% Html.Zone("menu"); %></div>
</div>
<div id="footer" role="contentinfo"><% Html.Zone("footer"); %></div>