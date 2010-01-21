﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<ContentItemViewModel<Blog>>" %>
<%@ Import Namespace="Orchard.Mvc.ViewModels"%>
<%@ Import Namespace="Orchard.Blogs.Extensions"%>
<%@ Import Namespace="Orchard.Blogs.Models"%>
<h2><%=Html.Link(Html.Encode(Model.Item.Name), Url.BlogForAdmin(Model.Item.Slug)) %></h2>
<div class="meta">
    <%Html.Zone("meta"); %>
    | <%=Html.Link(_Encoded("?? comments").ToString(), "") %>
</div>
<%--<p>[list of authors] [modify blog access]</p>--%>
<p><%=Html.Encode(Model.Item.Description) %></p>
<ul class="actions">
    <li class="construct">
        <a href="<%=Url.BlogForAdmin(Model.Item.Slug) %>" class="ibutton blog" title="<%=_Encoded("Manage Blog") %>"><%=_Encoded("Manage Blog") %></a>
        <a href="<%=Url.BlogEdit(Model.Item.Slug) %>" class="ibutton edit" title="<%=_Encoded("Edit Blog") %>"><%=_Encoded("Edit Blog") %></a>
        <a href="<%=Url.Blog(Model.Item.Slug) %>" class="ibutton view" title="<%=_Encoded("View Blog") %>"><%=_Encoded("View Blog") %></a>
        <a href="<%=Url.BlogPostCreate(Model.Item.Slug) %>" class="ibutton add page" title="<%=_Encoded("New Post") %>"><%=_Encoded("New Post") %></a>
    </li>
    <li class="destruct">
        <%-- todo: (heskew) this is waaaaa too verbose. need template helpers for all ibuttons --%>
        <% using (Html.BeginFormAntiForgeryPost(Url.BlogDelete(Model.Item.Slug), FormMethod.Post, new { @class = "inline" })) { %>
            <fieldset>
                <button type="submit" class="ibutton remove" title="<%=_Encoded("Remove Blog") %>"><%=_Encoded("Remove Blog") %></button>
            </fieldset><%
        } %>
    </li>
</ul>