﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<ModulesIndexViewModel>" %>
<%@ Import Namespace="Orchard.Modules.Extensions" %>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%@ Import Namespace="Orchard.Modules.ViewModels"%>
<h1><%=Html.TitleForPage(T("Installed Modules").ToString()) %></h1>
<%--<div class="manage"><%=Html.ActionLink(T("Install a module").ToString(), "Features", new { }, new { @class = "button primaryAction" })%></div>--%>
<%--<h2><%=T("Installed Modules") %></h2>--%>
<% if (Model.Modules.Count() > 0) { %>
<fieldset class="">
<ul class="contentItems"><%
    foreach (var module in Model.Modules.OrderBy(m => m.DisplayName)) { %>
    <li>
        <div class="summary">
            <div class="properties">
                <h3><%=Html.Encode(module.DisplayName) %></h3>
                <ul class="pageStatus" style="color:#666; margin:.6em 0 0 0;">
                    <li><%=T("Version: {0}", !string.IsNullOrEmpty(module.Version) ? Html.Encode(module.Version) : T("1.0")) %></li>
                    <li>&nbsp;&#124;&nbsp;<%=T("Features: {0}", string.Join(", ", module.Features.Select(f => Html.Link(Html.Encode(f.Name), string.Format("{0}#{1}", Url.Action("features", new { area = "Orchard.Modules" }), f.Name.AsFeatureId(n => T(n))) )).OrderBy(s => s).ToArray())) %></li>
                    <li>&nbsp;&#124;&nbsp;<%=T("Author: {0}", !string.IsNullOrEmpty(module.Author) ? Html.Encode(module.Author) : (new []{"Bradley", "Bertrand", "Renaud", "Suha", "Sebastien", "Jon", "Nathan", "Erik"})[(module.DisplayName.Length + (new Random()).Next()) % 7]) %></li><%-- very efficient, I know --%>
                   <li>&nbsp;&#124;&nbsp;<%=T("Website: {0}", !string.IsNullOrEmpty(module.HomePage) ? Html.Encode(module.HomePage) : T("<a href=\"http://orchardproject.net\">http://orchardproject.net</a>"))%></li>
                </ul>
            </div>
        </div><%
        if (!string.IsNullOrEmpty(module.Description)) { %>
        <p><%=Html.Encode(module.Description) %></p><%
        } %>
    </li><%
    } %>
</ul>


</fieldset><%
 } %>