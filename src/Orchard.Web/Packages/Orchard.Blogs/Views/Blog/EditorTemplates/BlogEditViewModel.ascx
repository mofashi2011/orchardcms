﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BlogEditViewModel>" %>
<%@ Import Namespace="Orchard.Blogs.ViewModels"%>
<fieldset>
    <label for="">Blog Name</label>
    <%=Html.EditorFor(m => m.Name) %>
<fieldset>
<%=Html.EditorFor(m => m.Slug, "BlogPermalink") %>
</fieldset>
    <label for="">Description</label>
    <%=Html.TextAreaFor(m => m.Description, 5, 60, null)%>
<fieldset>
</fieldset>
    <input class="button" type="submit" value="Save" />
</fieldset>