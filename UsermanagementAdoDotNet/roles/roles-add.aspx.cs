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
    public partial class roles_add : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindModules();

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

        private void BindModules()
        {
            string query = "SELECT id, name FROM modules";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
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

            string checkRoleQuery = "SELECT COUNT(1) FROM roles WHERE name = @Name";
            string insertRoleQuery = "INSERT INTO roles (name) VALUES (@Name); SELECT SCOPE_IDENTITY();";

            int roleId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if the role name already exists
                using (SqlCommand checkCmd = new SqlCommand(checkRoleQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Name", roleName);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        // Show an error message and exit
                        Response.Write("<script>alert('Role name already exists. Please use a different name.');</script>");
                        return;
                    }
                }

                // Insert the role if it doesn't already exist
                using (SqlCommand cmd = new SqlCommand(insertRoleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", roleName);
                    roleId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Insert module permissions
                foreach (RepeaterItem item in rptModules.Items)
                {
                    HiddenField hfModuleId = (HiddenField)item.FindControl("hfModuleId");
                    CheckBox chkView = (CheckBox)item.FindControl("chkView");
                    CheckBox chkAdd = (CheckBox)item.FindControl("chkAdd");
                    CheckBox chkEdit = (CheckBox)item.FindControl("chkEdit");
                    CheckBox chkDelete = (CheckBox)item.FindControl("chkDelete");

                    if (hfModuleId != null)
                    {
                        int moduleId = Convert.ToInt32(hfModuleId.Value);

                        // Insert a record into roles_modules_permissions
                        string insertPermissionQuery = @"
                    INSERT INTO roles_modules_permissions (module_id, role_id, is_default) 
                    VALUES (@ModuleId, @RoleId, 0); 
                    SELECT SCOPE_IDENTITY();";

                        int permissionId;
                        using (SqlCommand cmd = new SqlCommand(insertPermissionQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@ModuleId", moduleId);
                            cmd.Parameters.AddWithValue("@RoleId", roleId);
                            permissionId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insert rights into roles_modules_permissions_rights
                        if (chkView.Checked) InsertRight(conn, permissionId, 1); // Assuming '1' is the ID for the 'view' right
                        if (chkAdd.Checked) InsertRight(conn, permissionId, 2);  // Assuming '2' is the ID for the 'add' right
                        if (chkEdit.Checked) InsertRight(conn, permissionId, 3); // Assuming '3' is the ID for the 'edit' right
                        if (chkDelete.Checked) InsertRight(conn, permissionId, 4); // Assuming '4' is the ID for the 'delete' right
                    }
                }
            }

            Response.Redirect("roles-listing.aspx");
        }

        // Helper method to insert rights
        private void InsertRight(SqlConnection conn, int permissionId, int rightId)
        {
            string query = @"
        INSERT INTO roles_modules_permissions_rights (roles_modules_permissions_id, rights_id) 
        VALUES (@PermissionId, @RightId);";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                cmd.Parameters.AddWithValue("@RightId", rightId);
                cmd.ExecuteNonQuery();
            }
        }



    }
}