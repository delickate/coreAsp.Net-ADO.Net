using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace UsermanagementAdoDotNet.roles
{
    public partial class roles_edit : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
        int roleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get role ID from query string
            if (!int.TryParse(Request.QueryString["roleId"], out roleId))
            {
                Response.Redirect("roles-listing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRole();
                LoadModules();

                int userId;
                if (Session["UserID"] != null && int.TryParse(Session["UserID"].ToString(), out userId))
                {
                    GenerateNavigation(userId);
                }
                else
                {
                    // Redirect to login if the session is invalid
                    Response.Redirect("../Login.aspx");
                }
            }
        }

        private void GenerateNavigation(int userId)
        {
            // Database connection string
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;


            // Query to fetch modules and permissions for the user
            //            string query = @"
            //                SELECT m.name AS ModuleName, rmp.is_default AS IsDefault, r.name AS RightName
            //                FROM users_roles ur
            //                INNER JOIN roles_modules_permissions rmp ON ur.role_id = rmp.role_id
            //                INNER JOIN modules m ON rmp.module_id = m.id
            //                INNER JOIN roles_modules_permissions_rights rmpr ON rmp.id = rmpr.roles_modules_permissions_id
            //                INNER JOIN rights r ON rmpr.rights_id = r.id
            //                WHERE ur.user_id = @UserId
            //                ORDER BY m.sortid";

            string query = @"
                SELECT m.name AS ModuleName, m.url AS ModuleUrl
                FROM users_roles ur
                INNER JOIN roles_modules_permissions rmp ON ur.role_id = rmp.role_id
                INNER JOIN modules m ON rmp.module_id = m.id
                WHERE ur.user_id = @UserId
                ORDER BY m.sortid";

            // Connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        StringBuilder navBuilder = new StringBuilder();

                        while (reader.Read())
                        {
                            string moduleName = reader["ModuleName"].ToString();
                            string moduleUrl = reader["ModuleUrl"].ToString();

                            // Generate navigation link
                            navBuilder.AppendFormat(
                                "<li><a href='{0}'>{1}</a></li>",
                                ResolveUrl(moduleUrl), // Ensure proper URL formatting
                                moduleName
                            );
                        }

                        //string currentModule = null;

                        //while (reader.Read())
                        //{
                        //    string moduleName = reader["ModuleName"].ToString();
                        //    string rightName = reader["RightName"].ToString();

                        //    if (currentModule != moduleName)
                        //    {
                        //        // Close previous module's list (if any)
                        //        if (currentModule != null)
                        //            navBuilder.Append("</li>");

                        //        // Start a new module
                        //        navBuilder.AppendFormat("<li>{0}", moduleName);
                        //        currentModule = moduleName;
                        //    }

                        //    // Add rights under the module
                        //    //navBuilder.AppendFormat(" [{0}]", rightName);
                        //}

                        //// Close the last module's list
                        //if (currentModule != null)
                        //    navBuilder.Append("</li>");

                        // Add generated HTML to the navigation menu
                        navMenu.InnerHtml = navBuilder.ToString();
                    }
                }
            }
        }

        private void LoadRole()
        {
            string query = "SELECT name FROM roles WHERE id = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtRoleName.Text = reader["name"].ToString();
                        }
                        else
                        {
                            Response.Redirect("roles-listing.aspx");
                        }
                    }
                }
            }
        }

        private void LoadModules()
        {
            string query = @"
                SELECT 
                    m.id AS module_id,
                    m.name AS module_name,
                    rmp.id AS permission_id,
                    CASE WHEN EXISTS (SELECT 1 FROM roles_modules_permissions_rights r WHERE r.roles_modules_permissions_id = rmp.id AND r.rights_id = 1) THEN 1 ELSE 0 END AS can_view,
                    CASE WHEN EXISTS (SELECT 1 FROM roles_modules_permissions_rights r WHERE r.roles_modules_permissions_id = rmp.id AND r.rights_id = 2) THEN 1 ELSE 0 END AS can_add,
                    CASE WHEN EXISTS (SELECT 1 FROM roles_modules_permissions_rights r WHERE r.roles_modules_permissions_id = rmp.id AND r.rights_id = 3) THEN 1 ELSE 0 END AS can_edit,
                    CASE WHEN EXISTS (SELECT 1 FROM roles_modules_permissions_rights r WHERE r.roles_modules_permissions_id = rmp.id AND r.rights_id = 4) THEN 1 ELSE 0 END AS can_delete
                FROM modules m
                LEFT JOIN roles_modules_permissions rmp ON rmp.module_id = m.id AND rmp.role_id = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@RoleId", roleId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptModules.DataSource = dt;
                    rptModules.DataBind();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Update role name
                string updateRoleQuery = "UPDATE roles SET name = @Name WHERE id = @RoleId";
                using (SqlCommand cmd = new SqlCommand(updateRoleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", roleName);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.ExecuteNonQuery();
                }

                // Update module permissions
                foreach (RepeaterItem item in rptModules.Items)
                {
                    HiddenField hfModuleId = (HiddenField)item.FindControl("hfModuleId");
                    HiddenField hfPermissionId = (HiddenField)item.FindControl("hfPermissionId");
                    CheckBox chkView = (CheckBox)item.FindControl("chkView");
                    CheckBox chkAdd = (CheckBox)item.FindControl("chkAdd");
                    CheckBox chkEdit = (CheckBox)item.FindControl("chkEdit");
                    CheckBox chkDelete = (CheckBox)item.FindControl("chkDelete");

                    int moduleId = Convert.ToInt32(hfModuleId.Value);

                    string upsertPermissionQuery = @"
                        IF NOT EXISTS (SELECT 1 FROM roles_modules_permissions WHERE module_id = @ModuleId AND role_id = @RoleId)
                        BEGIN
                            INSERT INTO roles_modules_permissions (module_id, role_id, is_default)
                            VALUES (@ModuleId, @RoleId, 0);
                            SET @PermissionId = SCOPE_IDENTITY();
                        END
                        ELSE
                        BEGIN
                            SET @PermissionId = (SELECT id FROM roles_modules_permissions WHERE module_id = @ModuleId AND role_id = @RoleId);
                        END";

                    SqlCommand permissionCmd = new SqlCommand(upsertPermissionQuery, conn);
                    permissionCmd.Parameters.AddWithValue("@ModuleId", moduleId);
                    permissionCmd.Parameters.AddWithValue("@RoleId", roleId);
                    SqlParameter permissionIdParam = new SqlParameter("@PermissionId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    permissionCmd.Parameters.Add(permissionIdParam);
                    permissionCmd.ExecuteNonQuery();

                    int permissionId = Convert.ToInt32(permissionIdParam.Value);

                    UpdateRights(conn, permissionId, chkView.Checked, chkAdd.Checked, chkEdit.Checked, chkDelete.Checked);
                }
            }

            Response.Redirect("roles-listing.aspx");
        }

        private void UpdateRights(SqlConnection conn, int permissionId, bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            string deleteRightsQuery = "DELETE FROM roles_modules_permissions_rights WHERE roles_modules_permissions_id = @PermissionId";
            using (SqlCommand cmd = new SqlCommand(deleteRightsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                cmd.ExecuteNonQuery();
            }

            if (canView) InsertRight(conn, permissionId, 1); // 'View'
            if (canAdd) InsertRight(conn, permissionId, 2);  // 'Add'
            if (canEdit) InsertRight(conn, permissionId, 3); // 'Edit'
            if (canDelete) InsertRight(conn, permissionId, 4); // 'Delete'
        }

        private void InsertRight(SqlConnection conn, int permissionId, int rightId)
        {
            string query = @"
                INSERT INTO roles_modules_permissions_rights (roles_modules_permissions_id, rights_id) 
                VALUES (@PermissionId, @RightId)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                cmd.Parameters.AddWithValue("@RightId", rightId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
