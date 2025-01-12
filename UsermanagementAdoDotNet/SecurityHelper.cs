using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

namespace UsermanagementAdoDotNet
{
    public class SecurityHelper
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
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

                    // Use HttpContext.Current.Session to store permissions
                    HttpContext.Current.Session["UserPermissions"] = permissions;
                }
            }
        }




    }
}