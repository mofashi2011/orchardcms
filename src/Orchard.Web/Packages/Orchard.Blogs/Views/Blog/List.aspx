<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlogsViewModel>" %>
<%@ Import Namespace="Orchard.Blogs.Extensions"%>
<%@ Import Namespace="Orchard.Blogs.Models"%>
<%@ Import Namespace="Orchard.Blogs.ViewModels"%>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%-- todo: (heskew) make master-less when we get into theming --%>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Blogs</h2>
    <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>
    <%=Html.DisplayForModel() %>
</asp:Content>