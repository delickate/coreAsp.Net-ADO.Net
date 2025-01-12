<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="users_listing.aspx.cs" Inherits="UsermanagementAdoDotNet.users.users_listing" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Users Listing</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Users Management</h2>
        <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->
            </ul>
        </nav>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label><br />

        <asp:TextBox ID="txtName" runat="server" Placeholder="Name"></asp:TextBox><br />
        <asp:TextBox ID="txtPhone" runat="server" Placeholder="Phone"></asp:TextBox><br />
        <asp:TextBox ID="txtEmail" runat="server" Placeholder="Email"></asp:TextBox><br />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password"></asp:TextBox><br />
        <asp:DropDownList ID="ddlStatus" runat="server">
            <asp:ListItem Text="Active" Value="1"></asp:ListItem>
            <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
        </asp:DropDownList><br />
        <asp:DropDownList ID="ddlRoles" runat="server"></asp:DropDownList><br />
        <asp:FileUpload ID="fuPicture" runat="server" /><br />
        <asp:Button ID="btnAddUser" runat="server" Text="Add User" Visible='<%# HasRight(1, 2) %>' OnClick="btnAddUser_Click" /><br />
        

       <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvUsers_RowDeleting" OnRowEditing="gvUsers_RowEditing"  OnRowDataBound="gvUsers_RowDataBound" DataKeyNames="id">
    <Columns>
        <asp:BoundField DataField="name" HeaderText="Name" />
        <asp:BoundField DataField="phone" HeaderText="Phone" />
        <asp:BoundField DataField="email" HeaderText="Email" />
        <asp:ImageField DataImageUrlField="picture" HeaderText="Picture" />
        <asp:BoundField DataField="status" HeaderText="Status" />
        <asp:BoundField DataField="role" HeaderText="Roles" />
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id") %>' Text="Edit" HeaderText="Edit" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("id") %>' Text="Delete" HeaderText="Delete" OnClientClick="return confirm('Are you sure you want to delete this user?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>



    </form>
</body>
</html>
