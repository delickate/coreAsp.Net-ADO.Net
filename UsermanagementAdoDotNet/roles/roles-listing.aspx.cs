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
    public partial class roles_listing : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRoles();

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

        private void BindRoles()
        {
            string query = "SELECT id, name FROM roles";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvRoles.DataSource = dt;
                    gvRoles.DataKeyNames = new string[] { "id" }; // Ensure DataKeyNames is set
            
                    gvRoles.DataBind();
                }
            }
        }

        protected void gvRoles_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int roleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Edit")
            {
                Response.Redirect("roles-edit.aspx?roleId=" + roleId);
            }
            else if (e.CommandName == "Delete")
            {
                DeleteRole(roleId);
            }
        }

        protected void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < gvRoles.DataKeys.Count) // Validate RowIndex
            {
                int roleId = Convert.ToInt32(gvRoles.DataKeys[e.RowIndex].Value);

                // Call the method to delete the role
                DeleteRole(roleId);

                // Rebind the GridView after deletion
                BindRoles();
            }
            else
            {
                // Handle invalid RowIndex scenario
                Response.Write("<script>alert('Invalid row selected for deletion.');</script>");
            }
        }



        private void DeleteRole(int roleId)
        {
            string deleteUserRolesQuery = @"
        DELETE FROM users_roles 
        WHERE role_id = @RoleId";

            string deleteRightsQuery = @"
        DELETE FROM roles_modules_permissions_rights 
        WHERE roles_modules_permissions_id IN (
            SELECT id FROM roles_modules_permissions WHERE role_id = @RoleId
        )";

            string deletePermissionsQuery = @"
        DELETE FROM roles_modules_permissions 
        WHERE role_id = @RoleId";

            string deleteRoleQuery = @"
        DELETE FROM roles 
        WHERE id = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Delete associations in users_roles
                using (SqlCommand cmd = new SqlCommand(deleteUserRolesQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.ExecuteNonQuery();
                }

                // Delete rights associated with the role
                using (SqlCommand cmd = new SqlCommand(deleteRightsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.ExecuteNonQuery();
                }

                // Delete permissions associated with the role
                using (SqlCommand cmd = new SqlCommand(deletePermissionsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.ExecuteNonQuery();
                }

                // Delete the role
                using (SqlCommand cmd = new SqlCommand(deleteRoleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.ExecuteNonQuery();
                }
            }

            // Refresh the GridView after deletion
            BindRoles();
        }



        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("roles-add.aspx");
        }
    }
}