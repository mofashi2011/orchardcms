﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<FeaturesViewModel>" %>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%@ Import Namespace="Orchard.Modules.ViewModels"%>
<h1><%=Html.TitleForPage(T("Manage Features").ToString()) %></h1>
<div class="manage"><%=Html.ActionLink(T("∞").ToString(), "Features", new { }, new { @class = "button primaryAction" })%></div>
<% if (Model.Features.Count() > 0) {
       
using (Html.BeginFormAntiForgeryPost()) { %>
    <%=Html.ValidationSummary()%>
    <fieldset class="actions bulk">
        <label for="publishActions"><%=_Encoded("Actions: ")%></label>
        <select id="publishActions" name="<%=Html.NameOf(m => m.Options.BulkAction) %>">
            <%=Html.SelectOption(Model.Options.BulkAction, FeaturesBulkAction.None, _Encoded("Choose action...").ToString())%>
            <%=Html.SelectOption(Model.Options.BulkAction, FeaturesBulkAction.Enable, _Encoded("Enable").ToString())%>
            <%=Html.SelectOption(Model.Options.BulkAction, FeaturesBulkAction.Disable, _Encoded("Disable").ToString())%>
        </select>
        <input class="button" type="submit" name="submit.BulkEdit" value="<%=_Encoded("Apply") %>" />
    </fieldset>
    <fieldset class="pageList">
        <ul class="contentItems"><%
            foreach (var featureGroup in Model.Features.OrderBy(f => f.Descriptor.Name).GroupBy(f => f.Descriptor.Category)) { %>
            <li>
                <h2><%=Html.Encode(featureGroup.First().Descriptor.Category ?? T("General")) %></h2>
                <ul><%
                    foreach (var feature in featureGroup) {%>
                    <li>
                        <div class="summary">
                            <div class="properties">
                                <input type="checkbox" name="selection" value="<%=Html.Encode(feature.Descriptor.Name) %>" />
                                <h3><%=Html.Encode(feature.Descriptor.Name) %></h3>
                                <ul class="pageStatus">
                                    <li><%
                                    //enabled or not
                                    if (feature.IsEnabled) { %>
                                        <img class="icon" src="<%=ResolveUrl("~/Modules/Orchard.Modules/Content/Admin/images/enabled.gif") %>" alt="<%=_Encoded("Enabled") %>" title="<%=_Encoded("This feature is currently enabled") %>" /><%=_Encoded("Enabled") %><%
                                    }
                                    else { %>
                                        <img class="icon" src="<%=ResolveUrl("~/Modules/Orchard.Modules/Content/Admin/images/disabled.gif") %>" alt="<%=_Encoded("Disabled") %>" title="<%=_Encoded("This feature is currently disabled") %>" /><%=_Encoded("Disabled")%><%
                                    } %>
                                    </li><%
                                    //dependencies
                                    if (feature.Descriptor.Dependencies.Count() > 0) { %>
                                    <li>&nbsp;&#124;&nbsp;
                                        <%=_Encoded("Depends on: ") %><%
                                        foreach (var dependency in feature.Descriptor.Dependencies) {
                                            %><% if (dependency != feature.Descriptor.Dependencies.First()) { %><%=_Encoded(", ") %><% }
                                            %><%=Html.Encode(dependency) %><%
                                        } %>
                                    </li><%
                                    } 
                                    //dependencies == nothing <- temporary just to get some stuff in the UI
                                    else { %>
                                    <li>&nbsp;&#124;&nbsp;
                                        <%=T("Depends on: <em>nothing</em>") %>
                                    </li><%
                                    } %>
                                </ul>
                            </div>
                            <div class="related"><%
                                if (feature.IsEnabled) { %>
                                <a href="<%=Html.AntiForgeryTokenGetUrl(Url.Action("Disable", new { featureName = feature.Descriptor.Name, area = "Orchard.Modules" })) %>"><%=_Encoded("Disable") %></a><%
                                } else { %>
                                <a href="<%=Html.AntiForgeryTokenGetUrl(Url.Action("Enable", new { featureName = feature.Descriptor.Name, area = "Orchard.Modules" })) %>"><%=_Encoded("Enable") %></a><%
                                } %>
                            </div>
                        </div>
                    </li><%
                    } %>
                </ul>
            </li><%
            } %>
        </ul><%
        } %>
    </fieldset><%
} %>   