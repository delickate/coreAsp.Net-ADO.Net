<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="users_edit.aspx.cs" Inherits="UsermanagementAdoDotNet.users.users_edit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit User</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Edit User</h2>

            <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->
            </ul>
        </nav>


            <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox><br />

            <asp:Label ID="lblPhone" runat="server" Text="Phone"></asp:Label>
            <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox><br />

            <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox><br />

            <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
            <asp:DropDownList ID="ddlStatus" runat="server">
                <asp:ListItem Value="1">Active</asp:ListItem>
                <asp:ListItem Value="0">Inactive</asp:ListItem>
            </asp:DropDownList><br />

            <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />

            <asp:Label ID="lblRole" runat="server" Text="Role"></asp:Label>
            <asp:DropDownList ID="ddlRole" runat="server"></asp:DropDownList><br />

            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </div>
    </form>
</body>
</html>
