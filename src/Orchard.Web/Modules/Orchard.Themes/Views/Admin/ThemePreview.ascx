﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<PreviewViewModel>" %>
<%@ Import Namespace="Orchard.Mvc.Html"%>
<%@ Import Namespace="Orchard.Themes.ViewModels"%>
<style type="text/css">
body {
    margin-top:40px;
}
#themepreview {
    background:#2D2F25 url('<%=ResolveUrl("../../Styles/Images/toolBarBackground.gif") %>') repeat-x left top;
    border-bottom:1px solid #494d4d;
    font-size:15px; 
    left:0;
    height:30px;
    margin:0;
    position:absolute;
    overflow:hidden;
    padding:5px 0;
    top:0;
    width:100%;
}   
#themepreview fieldset,
#themepreview span, #themepreview input,
#themepreview select, #themepreview button  {
    border:none;
    color:#000;
    font:1em/1em Frutiger,"Frutiger Linotype",Univers,Calibri,"Gill Sans","Gill Sans MT","Myriad Pro",Myriad,"DejaVu Sans Condensed","Liberation Sans","Nimbus Sans L",Tahoma,Geneva,"Helvetica Neue",Helvetica,Arial,sans-serif;
    margin:0;
    padding:0;
    width:auto;
}
#themepreview span { color: #ccc; padding-right:5px; }
#themepreview fieldset { padding:3px 8px; }
html.dyn #themepreview button.preview { display:none; }
#themepreview fieldset * { float:left; }
#themepreview fieldset span { line-height:1.6em; }
#themepreview button.cancel { float:right; }
/* Button styles */
#themepreview button {
    background:#2a2626 url('<%=ResolveUrl("../../Styles/Images/toolBarActiveButtonBackground.gif") %>') repeat-x left center;
    border:1px solid;
    border-top-color:#191d1d;
    border-right-color:#494d4d;
    border-bottom-color:#494d4d;
    border-left-color:#202626;
    color:#f1f1f1;
    line-height:1.22em;
    margin: 0 0 0 10px;
    padding:0 4px 1px;
    text-align:center;
}
#themepreview button:hover {
    background:#2a2626 url('<%=ResolveUrl("../../Styles/Images/toolBarHoverButtonBackground.gif") %>') repeat-x left center;
    border-color:#545959;
    color:#fdcc64;
    cursor:pointer;
}
</style>
<div id="themepreview">
<% using(Html.BeginFormAntiForgeryPost(Url.Action("Preview", new{Controller="Admin", Area="Orchard.Themes"}), FormMethod.Post, new { @class = "inline" })) { %>
    <fieldset>    
        <span><%=T("You are previewing: ")%></span>
        <%=Html.Hidden("ReturnUrl", Context.Request.Url)%>
        <%=Html.DropDownList("ThemeName", Model.Themes, new {onChange = "this.form.submit();"})%>
        <button type="submit" class="preview" title="<%=_Encoded("Preview")%>" name="submit.Preview" value="<%=_Encoded("Preview")%>"><%=_Encoded("Preview")%></button>
        <button type="submit" title="<%=_Encoded("Apply")%>" name="submit.Apply" value="<%=_Encoded("Apply")%>"><%=_Encoded("Apply this theme") %></button>
        <button type="submit" class="cancel" title="<%=_Encoded("Cancel")%>" name="submit.Cancel" value="<%=_Encoded("Cancel")%>"><%=_Encoded("Cancel")%></button>
    </fieldset>
<% } %>
</div>

