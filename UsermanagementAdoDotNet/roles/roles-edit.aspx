<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roles-edit.aspx.cs" Inherits="UsermanagementAdoDotNet.roles.roles_edit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Role</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Edit Role</h2>

            <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->
            </ul>
        </nav>


            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            <table>
                <tr>
                    <td>Role Name:</td>
                    <td>
                        <asp:TextBox ID="txtRoleName" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <h3>Module Permissions</h3>
            <asp:Repeater ID="rptModules" runat="server">
                <HeaderTemplate>
                    <table border="1">
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
                            <%# Eval("module_name") %>
                            <asp:HiddenField ID="hfModuleId" runat="server" Value='<%# Eval("module_id") %>' />
                            <asp:HiddenField ID="hfPermissionId" runat="server" Value='<%# Eval("permission_id") %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkView" runat="server" Checked='<%# Convert.ToBoolean(Eval("can_view")) %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAdd" runat="server" Checked='<%# Convert.ToBoolean(Eval("can_add")) %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkEdit" runat="server" Checked='<%# Convert.ToBoolean(Eval("can_edit")) %>' />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkDelete" runat="server" Checked='<%# Convert.ToBoolean(Eval("can_delete")) %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" PostBackUrl="roles-listing.aspx" />
        </div>
    </form>
</body>
</html>

