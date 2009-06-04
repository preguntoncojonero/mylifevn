<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Canvass.master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="feature bg7">
		<h1 class="title">Chào mừng bạn đến với MyLife</h1>
		<div class="content">
			<p><img src="/Images/life-small.jpg" alt="" width="138" height="104" class="left" /> <strong>MyLife</strong> được tạo ra với mục đích tạo ra một thế giới kết nối. Tại nơi đây bạn có thể chia sẻ những cảm xúc của mình với mọi người xung quanh. Cùng với những tiện ích thú vị khác sẽ tạo cho bạn một không gian sống tràn đầy niềm vui. Với những tiếng cười sẽ giúp ích hơn cho cuộc sống của bạn. Hi vọng nơi đây sẽ là không gian sống song hành với cuộc sống của các bạn. Chúc các bạn vui vẻ!</p>
			<p>&nbsp;</p>
		</div>
	</div>
	<div class="box">
	    <% Html.RenderPartial("Personal", User.Identity.Name); %>
	</div>
	<div class="box" style="padding-left: 19px;">
		<% Html.RenderPartial("BlogTools", User.Identity.Name); %>
	</div>
</asp:Content>