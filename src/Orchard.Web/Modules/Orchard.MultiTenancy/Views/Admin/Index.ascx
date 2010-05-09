﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<TenantsIndexViewModel>" %>
<%@ Import Namespace="Orchard.Environment.Configuration" %>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%@ Import Namespace="Orchard.MultiTenancy.ViewModels"%>
<h1><%=Html.TitleForPage(T("List of Site's Tenants").ToString())%></h1>
<div class="manage"><%=Html.ActionLink(T("Add a Tenant").ToString(), "Add", new {area = "Orchard.MultiTenancy"}, new { @class = "button primaryAction" })%></div>
<ul class="contentItems"><%
    foreach (var tenant in Model.TenantSettings) { %>
    <li class="<%=tenant.State.CurrentState %>">
        <div class="summary">
            <div class="properties">
                <h3><span class="tenantState"><%
                        if (tenant.State.CurrentState == TenantState.State.Running) {
                            //todo: (heskew) need common shared resources (in the theme?) %>
                            <img class="icon" src="<%=ResolveUrl("~/Modules/Orchard.MultiTenancy/Content/Admin/images/enabled.gif") %>" alt="<%=_Encoded("Running") %>" title="<%=_Encoded("This tenant is currently running") %>" /><%
                        }
                        else { %>
                            <img class="icon" src="<%=ResolveUrl("~/Modules/Orchard.MultiTenancy/Content/Admin/images/disabled.gif") %>" alt="<%=_Encoded("Not Running") %>" title="<%=_Encoded("This tenant is not running for some reason") %>" /><%
                        } %></span>
                    <span class="tenantName"><%=Html.Encode(tenant.Name) %></span><%
                    if (!string.IsNullOrEmpty(tenant.RequestUrlHost)) {
                         %><span class="tenantHost"> - <%=Html.Link(tenant.RequestUrlHost, tenant.RequestUrlHost)%></span><%
                    } %>
                </h3>
            </div>
            <div class="related"><%
                if (!string.Equals(tenant.Name, "default", StringComparison.OrdinalIgnoreCase)) { //todo: (heskew) base this off the view model so logic on what can be removed and have its state changed stays in the controller
                var t = tenant; %>
                <%=Html.DisplayFor(m => t, string.Format("ActionsFor{0}", tenant.State.CurrentState)) %><%=_Encoded(" | ")%><%
                } %>
                <%=Html.ActionLink(T("Edit").ToString(), "edit", new {tenantName = tenant.Name, area = "Orchard.MultiTenancy"}) %><%
                if (!string.Equals(tenant.Name, "default", StringComparison.OrdinalIgnoreCase)) { //todo: (heskew) base this off the view model so logic on what can be removed and have its state changed stays in the controller
                %><%=_Encoded(" | ")%>
                <%=Html.ActionLink(T("Remove").ToString(), "delete", new {tenantName = tenant.Name, area = "Orchard.MultiTenancy"}) %><%
                } %>
            </div>
        </div>
    </li><%
    } %>
</ul>