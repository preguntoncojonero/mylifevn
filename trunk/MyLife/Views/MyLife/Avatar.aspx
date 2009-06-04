<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="silverlightControlHost" style="width: 500px; height: 350px;">
	    <object id="avatarControl" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		    <param name="source" value="/ClientBin/AvatarEditor.xap"/>
		    <param name="background" value="white" />
		    <param name="minRuntimeVersion" value="2.0.31005.0" />
		    <param name="autoUpgrade" value="true" />
		    <a href="http://go.microsoft.com/fwlink/?LinkID=124807" style="text-decoration: none; z-index: 1">
 			    <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style: none"/>
		    </a>
	    </object>
	    <iframe style='visibility:hidden;height:0;width:0;border:0px'></iframe>
    </div>
</asp:Content>