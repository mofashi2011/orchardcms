﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<ContentItemViewModel<Blog>>" %>
<%@ Import Namespace="Orchard.Mvc.ViewModels"%>
<%@ Import Namespace="Orchard.Blogs.Extensions"%>
<%@ Import Namespace="Orchard.Blogs.Models"%>
<h1 class="withActions">
    <a href="<%=Url.BlogForAdmin(Model.Item.Slug) %>"><%=Html.TitleForPage(Model.Item.Name) %></a>
</h1>

<%--<form>
<fieldset class="actions bulk">
    <label for="filterResults"><%=_Encoded("Filter:")%></label>
        <select id="filterResults" name="">
            <option value="">All Posts</option>
            <option value="">Published Posts</option>
        </select>
    <input class="button" type="submit" name="submit.Filter" value="<%=_Encoded("Apply") %>"/>
</fieldset>
</form>--%>


<div class="actions"><a href="<%=Url.BlogPostCreate(Model.Item.Slug) %>" class="add button"><%=_Encoded("New Post")%></a></div>
<% Html.Zone("primary");
   Html.ZonesAny(); %>