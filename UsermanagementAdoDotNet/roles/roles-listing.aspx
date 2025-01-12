<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roles-listing.aspx.cs" Inherits="UsermanagementAdoDotNet.roles.roles_listing" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Roles Listing</title>
</head>
<body>

    <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->
            </ul>
        </nav>
    <form id="form1" runat="server">
        <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="False" OnRowCommand="gvRoles_RowCommand" OnRowDeleting="gvRoles_RowDeleting" DataKeyNames="id">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Role Name" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("id") %>' Text="Edit" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("id") %>' Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this role?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnAddNew" runat="server" Text="Add New Role" OnClick="btnAddNew_Click" />
    </form>
</body>
</html>
