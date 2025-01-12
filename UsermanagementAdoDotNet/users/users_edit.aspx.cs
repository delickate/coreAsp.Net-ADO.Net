using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UsermanagementAdoDotNet.users
{
    public partial class users_edit : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int userId;
                if (int.TryParse(Request.QueryString["userId"], out userId))
                {
                    BindUserDetails(userId);
                    BindRoles();

                   
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
                else
                {
                    Response.Redirect("users-listing.aspx");
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

        private void BindUserDetails(int userId)
        {
            string query = "SELECT id, name, phone, email, status FROM users WHERE id = @UserId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtName.Text = reader["name"].ToString();
                        txtPhone.Text = reader["phone"].ToString();
                        txtEmail.Text = reader["email"].ToString();
                        ddlStatus.SelectedValue = reader["status"].ToString();
                    }
                    else
                    {
                        Response.Redirect("users_listing.aspx");
                    }
                }
            }
        }

        private void BindRoles()
        {
            string query = "SELECT id, name FROM roles";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlRole.DataSource = reader;
                    ddlRole.DataTextField = "name";
                    ddlRole.DataValueField = "id";
                    ddlRole.DataBind();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string updateUserQuery = @"
        UPDATE users 
        SET name = @Name, phone = @Phone, email = @Email, password = @Password, status = @Status, is_default = @IsDefault 
        WHERE id = @UserId";

            string updateRoleQuery = "UPDATE users_roles SET role_id = @RoleId WHERE user_id = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Update the user details
                using (SqlCommand userCmd = new SqlCommand(updateUserQuery, conn))
                {
                    string hashedPassword = SecurityHelper.HashPassword(txtPassword.Text);
                    userCmd.Parameters.AddWithValue("@Name", txtName.Text);
                    userCmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    userCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    userCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    userCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    userCmd.Parameters.AddWithValue("@IsDefault", 0); // Assuming you're not allowing default user to be updated
                    userCmd.Parameters.AddWithValue("@UserId", userId); // Set the correct user ID
                    userCmd.ExecuteNonQuery();
                }

                // Update the user's role
                using (SqlCommand roleCmd = new SqlCommand(updateRoleQuery, conn))
                {
                    roleCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue); // Get the selected role
                    roleCmd.Parameters.AddWithValue("@UserId", userId);
                    roleCmd.ExecuteNonQuery();
                }
            }

            // Optionally, update the user's role here
            UpdateUserRole(userId);

            // Redirect to the listing page after saving
            Response.Redirect("users-listing.aspx");
        }

        private void UpdateUserRole(int userId)
        {
            string updateRoleQuery = "UPDATE users_roles SET role_id = @RoleId WHERE user_id = @UserId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(updateRoleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}