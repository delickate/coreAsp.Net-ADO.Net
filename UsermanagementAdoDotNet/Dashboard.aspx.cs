using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UsermanagementAdoDotNet
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                lblWelcome.Text = "Welcome, " + Session["UserName"].ToString();

                int userId;
                if (Session["UserID"] != null && int.TryParse(Session["UserID"].ToString(), out userId))
                {
                    GenerateNavigation(userId);
                }
                else
                {
                    // Redirect to login if the session is invalid
                    Response.Redirect("Login.aspx");
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

        bool HasAccess(string moduleName)
        {
            // Fetch role and module rights based on logged-in user
            return true; // Simplified for illustration
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Optionally, clear authentication cookies
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                HttpCookie authCookie = new HttpCookie(".ASPXAUTH");
                authCookie.Expires = DateTime.Now.AddDays(-1); // Expire the cookie
                Response.Cookies.Add(authCookie);
            }

            // Redirect to login page
            Response.Redirect("~/login.aspx");
        }


    }
}