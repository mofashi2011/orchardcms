﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<Blog>" %>
<%@ Import Namespace="Orchard.Blogs.Models"%>
<div class="blogdescription">
    <p><%=Html.Encode(Model.Description) %></p>
</div>