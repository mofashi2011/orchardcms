﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<object>" %>
<h1><%=Html.TitleForPage(T("Change Password").ToString()) %></h1>
<p><%=_Encoded("Your password has been changed successfully.")%></p>