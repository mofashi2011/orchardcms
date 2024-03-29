﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<BodyEditorViewModel>" %>
<%@ Import Namespace="Orchard.Core.Common.ViewModels"%>
<% Html.RegisterScript("tiny_mce.js"); %>
<%=Html.TextArea("Text", Model.Text, 25, 80, new { @class = "html" }) %><%
using (this.Capture("end-of-page-scripts")) {%>
<script type="text/javascript">
    tinyMCE.init({
        theme: "advanced",
        mode: "specific_textareas",
        editor_selector: "html",
        plugins: "fullscreen,autoresize,searchreplace,addmedia",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_buttons1: "search,replace,|,cut,copy,paste,|,undo,redo,|,image,addmedia,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",
        convert_urls: false,
        addmedia_action: "<%=Url.Action("AddFromClient", "Admin", new {area = "Orchard.Media"}) %>",
        addmedia_path: "<%= Model.AddMediaPath %>",
        request_verification_token: "<%=Html.AntiForgeryTokenValueOrchard() %>"
    });
</script><%
}%>