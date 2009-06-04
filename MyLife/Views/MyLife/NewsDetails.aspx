<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" Inherits="System.Web.Mvc.ViewPage<MyLife.Web.News.News>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="news">
        <fieldset>
            <legend><%= Model.CreatedDate.ToString("d/M") %>: <%= Html.Encode(Model.Title) %></legend>
            <p>
                <%= Model.Content %>
            </p>
        </fieldset>
    </div>
</asp:Content>