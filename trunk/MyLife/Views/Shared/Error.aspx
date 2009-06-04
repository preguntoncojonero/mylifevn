<%@ Page Language="C#" MasterPageFile="~/Views/MyLife/Shared.master" AutoEventWireup="true"
    CodeBehind="Error.aspx.cs" Inherits="MyLife.Views.Shared.Error" %>
<%@ Import Namespace="MyLife"%>

<asp:Content ID="divHeadContent" ContentPlaceHolderID="divHead" runat="server">
    <title><%= ViewData[Constants.ViewData.Title] %></title>
</asp:Content>

<asp:Content ID="divDetailsContent" ContentPlaceHolderID="divDetails" runat="server">
    <br />
        <div class="mylife-form mylife-error">
            <form action="#">                
                <center><span class="status"><%= ViewData[Constants.ViewData.Message] %></span></center>
            </form>
        </div>
    <br />    
</asp:Content>
