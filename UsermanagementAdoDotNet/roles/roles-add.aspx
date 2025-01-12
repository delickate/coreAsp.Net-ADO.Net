<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roles-add.aspx.cs" Inherits="UsermanagementAdoDotNet.roles.roles_add" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Role</title>
</head>
<body>

    <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->
            </ul>
        </nav>


    <form id="form1" runat="server">
        <asp:TextBox ID="txtRoleName" runat="server" Placeholder="Role Name"></asp:TextBox>
        <asp:Repeater ID="rptModules" runat="server">
            <HeaderTemplate>
                <table>
                    <tr>
                        <th>Module</th>
                        <th>View</th>
                        <th>Add</th>
                        <th>Edit</th>
                        <th>Delete</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                <%# Eval("name") %>
                <asp:HiddenField ID="hfModuleId" runat="server" Value='<%# Eval("id") %>' />
            </td>
                    <td><asp:CheckBox ID="chkView" runat="server" /></td>
                    <td><asp:CheckBox ID="chkAdd" runat="server" /></td>
                    <td><asp:CheckBox ID="chkEdit" runat="server" /></td>
                    <td><asp:CheckBox ID="chkDelete" runat="server" /></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
    </form>
</body>
</html>

