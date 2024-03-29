﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<BodyDisplayViewModel>" %>
<%@ Import Namespace="Orchard.Core.Common.ViewModels"%>
<%--//doing excerpt generation on the way out for now so we don't stick ourselves with needing to regen excerpts for existing data
    //also, doing this here, inline, until we have a pluggable processing model (both in and out)
    //also, ...this is ugly--%>
<%=string.Format(
    "<p>{0} {1}</p>",
    Html.Excerpt(Model.Text, 200).ToString().Replace("\r\n", "</p>\r\n<p>"),
    Html.ItemDisplayLink(T("[more]").ToString(), Model.BodyAspect.ContentItem)) %>