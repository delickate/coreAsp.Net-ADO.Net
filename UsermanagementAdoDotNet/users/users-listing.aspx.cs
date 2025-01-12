using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.IO;





namespace UsermanagementAdoDotNet.users
{
    public partial class users_listing : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUsers();
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

        private void BindUsers()
        {
            string query = @"
        SELECT u.id, u.name, u.phone, u.email, u.picture, u.is_default, u.status, r.name AS role 
        FROM users u
        JOIN users_roles ur ON u.id = ur.user_id
        JOIN roles r ON ur.role_id = r.id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
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
                    ddlRoles.DataSource = dt;
                    ddlRoles.DataTextField = "name";  // Name of the role to display
                    ddlRoles.DataValueField = "id";  // ID of the role
                    ddlRoles.DataBind();

                    // Optionally add a default "Select" item
                    //ddlRoles.Items.Insert(0, new ListItem("--Select Role--", "0"));
                }
            }
        }


        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string checkEmailQuery = "SELECT COUNT(1) FROM users WHERE email = @Email";
            string insertUserQuery = @"
        INSERT INTO users (name, phone, email, password, status, is_default) 
        VALUES (@Name, @Phone, @Email, @Password, @Status, 0);
        SELECT SCOPE_IDENTITY();";  // Get the inserted user ID

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if the email already exists
                using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    int emailExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (emailExists > 0)
                    {
                        // Show an error message if the email exists
                        Response.Write("<script>alert('The email already exists. Please use a different email.');</script>");
                        return;
                    }
                }

                // Insert the user and get the inserted user ID
                int userId = 0;
                using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, conn))
                {
                    string hashedPassword = SecurityHelper.HashPassword(txtPassword.Text);
                    insertCmd.Parameters.AddWithValue("@Name", txtName.Text);
                    insertCmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    insertCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    insertCmd.Parameters.AddWithValue("@Password", hashedPassword); // Encrypt if needed
                    insertCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);

                    userId = Convert.ToInt32(insertCmd.ExecuteScalar());
                }

                // Insert the role into the users_roles table
                string insertRoleQuery = "INSERT INTO users_roles (user_id, role_id) VALUES (@UserId, @RoleId)";
                using (SqlCommand roleCmd = new SqlCommand(insertRoleQuery, conn))
                {
                    roleCmd.Parameters.AddWithValue("@UserId", userId);
                    roleCmd.Parameters.AddWithValue("@RoleId", ddlRoles.SelectedValue); // Get the selected role
                    roleCmd.ExecuteNonQuery();
                }
            }

            // Refresh the user list after insertion
            BindUsers();
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the value of is_default for the current row
                int isDefault = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "is_default"));

                // Find the Edit and Delete buttons in the current row
                LinkButton btnEdit = (LinkButton)e.Row.FindControl("btnEdit");
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");

                // Show Edit and Delete buttons only if is_default is 0
                if (isDefault == 0)
                {
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                }
                else
                {
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }


        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Delete")
            {
                DeleteUser(userId);
            }
            else if (e.CommandName == "Edit")
            {
                // Redirect to an edit page
                //Response.Redirect("edit-user.aspx?id={userId}");

                //int userId = Convert.ToInt32(userId);
                Response.Redirect("users_edit.aspx?userId=" + userId);
            }
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //int moduleId = 1; // Module ID for "Users"
            //if (HasRight(moduleId, 3)) // Check 'Delete' right
            //{
                int userId = Convert.ToInt32(gvUsers.DataKeys[e.NewEditIndex].Value);

                // Redirect to the edit page with the userId in the query string
                Response.Redirect("users_edit.aspx?userId=" + userId);
            //}
        }



        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            int moduleId = 1; // Module ID for "Users"
            if (HasRight(moduleId, 4)) // Check 'Delete' right
            {
                int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

                // Call the method to delete the user
                DeleteUser(userId);

                // Rebind the GridView after deletion
                BindUsers();
            }

            
        }

        public bool HasRight(int moduleId, int rightId)
        {
            var permissions = Session["UserPermissions"] as Dictionary<int, List<int>>;
            if (permissions != null && permissions.ContainsKey(moduleId))
            {
                return permissions[moduleId].Contains(rightId);
            }
            return false;
        }



        private void DeleteUser(int userId)
        {
            string deleteUserRoleQuery = "DELETE FROM users_roles WHERE user_id = @UserId";
            string deleteUserQuery = "DELETE FROM users WHERE id = @UserId AND is_default = 0";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Delete related records from users_roles table
                using (SqlCommand deleteUserRoleCmd = new SqlCommand(deleteUserRoleQuery, conn))
                {
                    deleteUserRoleCmd.Parameters.AddWithValue("@UserId", userId);
                    deleteUserRoleCmd.ExecuteNonQuery();
                }

                // Delete the user from the users table
                using (SqlCommand deleteUserCmd = new SqlCommand(deleteUserQuery, conn))
                {
                    deleteUserCmd.Parameters.AddWithValue("@UserId", userId);
                    int rowsAffected = deleteUserCmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Response.Write("<script>alert('Cannot delete default users or user does not exist.');</script>");
                    }
                }
            }

            // Refresh the user list
            BindUsers();
        }

    }
}