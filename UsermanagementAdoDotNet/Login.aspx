<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UsermanagementAdoDotNet.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="hidden" name="csrfToken" value="<%= ViewState["CSRFToken"] %>" />

<asp:TextBox ID="txtEmail" runat="server" Placeholder="Email"></asp:TextBox>
<asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password"></asp:TextBox>
<asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
<asp:Literal ID="litMessage" runat="server" />

    </div>
    </form>
</body>
</html>
