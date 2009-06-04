<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<div class="personal">
    <h2>Thông tin cá nhân</h2>
    <p>Nơi đây lưu trữ những thông tin về bạn</p>
    <ul>
        <li><%= Html.Link("Hồ sơ cá nhân", string.Format("/{0}/profile", Model)) %></li>
        <li><%= Html.Link("Những người bạn", string.Format("/{0}/friends", Model)) %></li>
        <li><%= Html.Link("Tài chính cá nhân", string.Format("/{0}/moneybox", Model)) %></li>
    </ul>
</div>