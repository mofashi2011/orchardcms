﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<CommentCountViewModel>" %>
<%@ Import Namespace="Orchard.Comments.ViewModels"%>
<span class="commentcount"><%=_Encoded("{0} Comment{1}", Model.CommentCount, Model.CommentCount == 1 ? "" : "s")%></span>