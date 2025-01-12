<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="UsermanagementAdoDotNet.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <asp:Label ID="lblWelcome" runat="server" Text="Welcome!"></asp:Label>

    <nav>
            <ul id="navMenu" runat="server">
                <!-- Navigation items will be dynamically added here -->

                
            </ul>
        </nav>
    
    <form id="form1" runat="server">
    <div>
    <ul runat="server"><li>
        <asp:LinkButton ID="btnLogout" runat="server" OnClick="Logout_Click">Logout</asp:LinkButton>
    </li></ul>
    </div>
    </form>
</body>
</html>
