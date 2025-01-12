using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;


namespace UsermanagementAdoDotNet
{
    public partial class Login : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string csrfToken = Guid.NewGuid().ToString();
                Session["CSRFToken"] = csrfToken;
                ViewState["CSRFToken"] = csrfToken;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string csrfToken = Request.Form["csrfToken"]; // CSRF token validation
            string hashedPassword = SecurityHelper.HashPassword(password);

            if (ValidateCSRFToken(csrfToken))
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE email = @Email AND password = @Password AND status = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Store session data
                        Session["UserID"] = reader["id"];
                        Session["UserName"] = reader["name"];

                        int userId = 0;

                        // Check if Session["UserID"] is not null before casting
                        if (Session["UserID"] != null)
                        {
                            userId = Convert.ToInt32(Session["UserID"]);
                            // Now you can use userId safely
                        }
                        else
                        {
                            // Handle case where the session is not set
                            // For example, redirect to login page or show an error message
                        }


                        if (userId > 0)
                        {
                            LoadUserPermissions(userId);
                        }
                        Response.Redirect("Dashboard.aspx");
                    }
                    else
                    {
                        litMessage.Text = "Invalid email or password.";
                    }
                }
            }
            else
            {
                litMessage.Text = "Invalid CSRF token.";
            }
        }

        private bool ValidateCSRFToken(string token)
        {
            string sessionToken = Session["CSRFToken"].ToString();
            return sessionToken == token;
        }


        private void LoadUserPermissions(int userId)
        {
            string query = @"
        SELECT p.module_id, r.rights_id 
        FROM roles_modules_permissions p
        INNER JOIN roles_modules_permissions_rights r 
            ON p.id = r.roles_modules_permissions_id
        INNER JOIN users_roles ur 
            ON ur.role_id = p.role_id
        WHERE ur.user_id = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, List<int>> permissions = new Dictionary<int, List<int>>();
                    while (reader.Read())
                    {
                        int moduleId = reader.GetInt32(0);
                        int rightsId = reader.GetInt32(1);

                        if (!permissions.ContainsKey(moduleId))
                            permissions[moduleId] = new List<int>();

                        permissions[moduleId].Add(rightsId);
                    }

                    HttpContext.Current.Session["UserPermissions"] = permissions;
                }
            }
        }
    }
}