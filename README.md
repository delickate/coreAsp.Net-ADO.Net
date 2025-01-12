# coreAsp.Net-ADO.Net

# User Management System

This project is a **User Management System** developed using **Core ASP.NET Web Forms (ADO.NET)**. It provides a robust system for managing users, roles, modules, and their respective permissions.

---

## Features

1. **User Management**
   - Add, edit, delete, and view user details.
   - Manage user roles and permissions.

2. **Role Management**
   - Define roles with specific module permissions.
   - Assign roles to users.

3. **Module Management**
   - Manage modules and their hierarchy (e.g., parent-child relationship).
   - Control module access through roles and permissions.

4. **Permission Control**
   - Define rights (e.g., view, add, edit, delete) for modules.
   - Assign rights to roles on a per-module basis.

5. **Authentication**
   - Secure login with role-based access control.
   - Maintain session-based user authentication.

---

## Database Schema

### Database Name
`sani_usermanagement`

### Tables and Fields

#### 1. Rights
Stores the list of all possible actions or operations.

| Field Name | Description         |
|------------|---------------------|
| `id`       | Unique identifier.  |
| `name`     | Name of the right (e.g., view, add). |

#### 2. Modules
Represents system modules and their details.

| Field Name    | Description                              |
|---------------|------------------------------------------|
| `id`          | Unique identifier.                      |
| `name`        | Name of the module.                     |
| `url`         | URL or route of the module.             |
| `parent_id`   | ID of the parent module (for hierarchy). |
| `is_default`  | Indicates if it's a default module.      |
| `status`      | Status of the module (e.g., active).     |
| `created_at`  | Timestamp of creation.                  |
| `slug`        | Unique slug for the module.             |
| `sortid`      | Sorting order for display.              |

#### 3. Roles
Stores information about system roles.

| Field Name   | Description                              |
|--------------|------------------------------------------|
| `id`         | Unique identifier.                      |
| `name`       | Name of the role (e.g., admin, viewer).  |
| `is_default` | Indicates if it's a default role.        |

#### 4. Roles Modules Permissions
Defines which roles have access to specific modules.

| Field Name   | Description                              |
|--------------|------------------------------------------|
| `id`         | Unique identifier.                      |
| `role_id`    | ID of the associated role.              |
| `module_id`  | ID of the associated module.            |
| `is_default` | Indicates if it's a default permission. |

#### 5. Roles Modules Permissions Rights
Maps rights (actions) to role-module permissions.

| Field Name                  | Description                              |
|-----------------------------|------------------------------------------|
| `id`                        | Unique identifier.                      |
| `roles_modules_permissions_id` | ID of the associated role-module permission. |
| `rights_id`                 | ID of the associated right.             |
| `is_default`                | Indicates if it's a default right.      |

#### 6. Users
Stores user information.

| Field Name      | Description                              |
|------------------|------------------------------------------|
| `id`            | Unique identifier.                      |
| `name`          | Name of the user.                       |
| `email`         | Email address of the user.              |
| `password`      | Encrypted password.                     |
| `phone`         | Phone number.                           |
| `picture`       | Profile picture URL.                    |
| `status`        | Status (e.g., active, inactive).        |
| `aboutme`       | User's bio.                             |
| `interests`     | User's interests.                       |
| `education_id`  | Education level.                        |
| `gender_id`     | Gender identifier.                      |
| `is_default`    | Indicates if it's a default user.       |

#### 7. Users Roles
Links users with roles.

| Field Name | Description                              |
|------------|------------------------------------------|
| `id`       | Unique identifier.                      |
| `user_id`  | ID of the associated user.              |
| `role_id`  | ID of the associated role.              |

---

## How Permissions Work

1. **Modules**
   - Represent system functionalities (e.g., users, dashboard).

2. **Rights**
   - Represent actions like `view`, `add`, `edit`, `delete`.

3. **Roles Modules Permissions**
   - Assign modules to roles.

4. **Roles Modules Permissions Rights**
   - Assign specific rights to roles for a module.

5. **Users Roles**
   - Assign roles to users.

---

## Usage

### 1. Adding Roles and Assigning Permissions
- Use `roles-add.aspx` to create roles.
- Assign module permissions (view, add, edit, delete) to roles.

### 2. Editing Roles and Permissions
- Use `roles-edit.aspx` to modify roles and their permissions.

### 3. Managing Users
- Assign roles to users via `users-add.aspx` or `users-edit.aspx`.

---

## Navigation

The system dynamically generates the navigation menu based on user permissions. The menu hides unauthorized modules and actions based on the user's role and rights.

---

## Features

- **User Management**: Add, edit, delete, and view user details.
- **Role Management**: Assign roles to users with module-based permissions.
- **Permission Control**: Define hierarchical permissions (view, add, edit, delete) for each module.
- **Authentication**: Secure login and session-based authentication.
- **Dynamic Rights Check**: Only users with appropriate rights can access specific actions.
- **File Uploads**: Supports profile picture uploads for users.
- **Pagination**: Manage listings with pagination for improved performance.
- **Validation**: Comprehensive validation for forms and input fields.
- **Responsive Design**: Optimized for desktop and mobile devices.

---

## Table of Contents

1. [Installation](#installation)
2. [Configuration](#configuration)
3. [License](#license)

---

## Installation

1. Import the `sani_usermanagement` database into your SQL Server.
2. Configure the connection string in `Web.config`:
   ```xml
   <connectionStrings>
       <add name="UserManagementConnection" connectionString="Data Source=YOUR_SERVER;Initial Catalog=sani_usermanagement;Integrated Security=True;" providerName="System.Data.SqlClient" />
   </connectionStrings>```
   
   
---

## Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new feature branch.
3. Submit a pull request.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Basic concepts ADO

### MSACESS

#### Connection
```
<%
set conn=Server.CreateObject("ADODB.Connection")
conn.Provider="Microsoft.Jet.OLEDB.4.0"
conn.Open "C:/Users/delickate/Documents/Visual Studio 2013/Projects/UsermanagementAdoDotNet/webdata/usermanagemenetdb.mdb"
%> 
```

#### Recordset
```
<%
set rs=Server.CreateObject("ADODB.recordset")
rs.Open "Customers", conn
%>
```

#### Show record
```
<html>
<body>

<%
set conn=Server.CreateObject("ADODB.Connection")
conn.Provider="Microsoft.Jet.OLEDB.4.0"
conn.Open "C:/Users/delickate/Documents/Visual Studio 2013/Projects/UsermanagementAdoDotNet/webdata/usermanagemenetdb.mdb"

set rs = Server.CreateObject("ADODB.recordset")
rs.Open "SELECT * FROM users", conn

do until rs.EOF
  for each x in rs.Fields
    Response.Write(x.name)
    Response.Write(" = ")
    Response.Write(x.value & "<br>")
  next
  Response.Write("<br>")
  rs.MoveNext
loop

rs.close
conn.close
%>

</body>
</html> 
```

#### Fetch record
```
<%
set conn=Server.CreateObject("ADODB.Connection")
conn.Provider="Microsoft.Jet.OLEDB.4.0"
conn.Open "C:/Users/delickate/Documents/Visual Studio 2013/Projects/UsermanagementAdoDotNet/webdata/usermanagemenetdb.mdb"

set rs=Server.CreateObject("ADODB.recordset")
sql="SELECT * FROM users WHERE name LIKE 'A%'"
rs.Open sql, conn
%>

<table border="1" width="100%">
  <tr>
  <%for each x in rs.Fields
    response.write("<th>" & x.name & "</th>")
  next%>
  </tr>
  <%do until rs.EOF%>
    <tr>
    <%for each x in rs.Fields%>
      <td><%Response.Write(x.value)%></td>
    <%next
    rs.MoveNext%>
    </tr>
  <%loop
  rs.close
  conn.close%>
</table>
```

#### insert
```
<%
set conn=Server.CreateObject("ADODB.Connection")
conn.Provider="Microsoft.Jet.OLEDB.4.0"
conn.Open "C:/Users/delickate/Documents/Visual Studio 2013/Projects/UsermanagementAdoDotNet/webdata/usermanagemenetdb.mdb"

sql="INSERT INTO customers (id,name,"
sql=sql & "phone,address,city,postalcode,country)"
sql=sql & " VALUES "
sql=sql & "('" & Request.Form("id") & "',"
sql=sql & "'" & Request.Form("person_name") & "',"
sql=sql & "'" & Request.Form("person_phone") & "',"
sql=sql & "'" & Request.Form("person_address") & "',"
sql=sql & "'" & Request.Form("person_city") & "',"
sql=sql & "'" & Request.Form("person_postcode") & "',"
sql=sql & "'" & Request.Form("person_country") & "')"

on error resume next
conn.Execute sql,recaffected
if err<>0 then
  Response.Write("No update permissions!")
else
  Response.Write("<h3>" & recaffected & " record added</h3>")
end if
conn.close
%>
```

#### update
```
<%
set conn=Server.CreateObject("ADODB.Connection")
conn.Provider="Microsoft.Jet.OLEDB.4.0"
conn.Open "C:/Users/delickate/Documents/Visual Studio 2013/Projects/UsermanagementAdoDotNet/webdata/usermanagemenetdb.mdb"

id=Request.Form("person_id")

if Request.form("companyname")="" then
  set rs=Server.CreateObject("ADODB.Recordset")
  rs.open "SELECT * FROM users WHERE id='" & id & "'",conn
  %>
  <form method="post" action="demo_update.asp">
  <table>
  <%for each x in rs.Fields%>
  <tr>
  <td><%=x.name%></td>
  <td><input name="<%=x.name%>" value="<%=x.value%>"></td>
  <%next%>
  </tr>
  </table>
  <br><br>
  <input type="submit" value="Update record">
  </form>
<%
else
  sql="UPDATE users SET "
  sql=sql & "'" & Request.Form("person_name") & "',"
  sql=sql & "'" & Request.Form("person_phone") & "',"
  sql=sql & "'" & Request.Form("person_address") & "',"
  sql=sql & "'" & Request.Form("person_city") & "',"
  sql=sql & "'" & Request.Form("person_postcode") & "',"
  sql=sql & "'" & Request.Form("person_country") & "')"
  sql=sql & " WHERE id='" & id & "'"
  on error resume next
  conn.Execute sql
  if err<>0 then
    response.write("No update permissions!")
  else
    response.write("Record " & id & " was updated!")
  end if
end if
conn.close
%>
```

#### Delete
```
sql="DELETE FROM users"
  sql=sql & " WHERE id='" & id & "'"
  on error resume next
  conn.Execute sql
  if err<>0 then
    response.write("No update permissions!")
  else
    response.write("Record " & id & " was deleted!")
  end if
```  

### MS SQL

#### connection
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                Console.WriteLine("Connection Established Successfully");
            }
```

#### command
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
        connection.Open();
        string sqlQuery = "SELECT * FROM users";
		SqlCommand command = new SqlCommand(sqlQuery, connection);

		SqlDataReader reader = command.ExecuteReader();
		while (reader.Read())
		{
		      Console.WriteLine(reader["name"] + " " + reader["phone"]);
		}
		reader.Close();
}

```

#### datareader
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
        // Creating the command object
        SqlCommand cmd = new SqlCommand("select * from users", connection);
        // Opening Connection  
        connection.Open();
        // Executing the SQL query  
        SqlDataReader sdr = cmd.ExecuteReader();
        //Looping through each record
        while (sdr.Read())
        {
           Console.WriteLine(sdr["name"] + ",  " + sdr["email"] + ",  " + sdr["phone"]);
        }

}

```

#### dataAdapter
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
                    SqlDataAdapter da = new SqlDataAdapter("select * from users", connection);
                    //Using Data Table
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    //The following things are done by the Fill method
                    //1. Open the connection
                    //2. Execute Command
                    //3. Retrieve the Result
                    //4. Fill/Store the Retrieve Result in the Data table
                    //5. Close the connection
                    Console.WriteLine("Using Data Table");
                    //Active and Open connection is not required
                    //dt.Rows: Gets the collection of rows that belong to this table
                    //DataRow: Represents a row of data in a DataTable.
                    foreach (DataRow row in dt.Rows)
                    {
                        //Accessing using string Key Name
                        Console.WriteLine(row["name"] + ",  " + row["email"] + ",  " + row["phone"]);
                        //Accessing using integer index position
                        //Console.WriteLine(row[0] + ",  " + row[1] + ",  " + row[2]);
                    }
                    Console.WriteLine("---------------");
                    //Using DataSet
                    DataSet ds = new DataSet();
                    da.Fill(ds, "student"); //Here, the datatable student will be stored in Index position 0
                    Console.WriteLine("Using Data Set");
                    //Tables: Gets the collection of tables contained in the System.Data.DataSet.
                    //Accessing the datatable from the dataset using the datatable name
                    foreach (DataRow row in ds.Tables["users"].Rows)
                    {
                        //Accessing the data using string Key Name
                        Console.WriteLine(row["name"] + ",  " + row["email"] + ",  " + row["phone"]);
                        //Accessing the data using integer index position
                        //Console.WriteLine(row[0] + ",  " + row[1] + ",  " + row[2]);
                    }
}

```

#### dataTable
```
using System;
using System.Data.SqlClient;


				//Creating data table instance
                DataTable dataTable = new DataTable("users");
                //Add the DataColumn using all properties
                DataColumn Id = new DataColumn("id");
                Id.DataType = typeof(int);
                Id.Unique = true;
                Id.AllowDBNull = false;
                Id.Caption = "ID";
                dataTable.Columns.Add(Id);
                
                //Add the DataColumn few properties
                DataColumn Name = new DataColumn("name");
                Name.MaxLength = 50;
                Name.AllowDBNull = false;
                dataTable.Columns.Add(Name);
                
                //Add the DataColumn using defaults
                DataColumn Email = new DataColumn("email");
                dataTable.Columns.Add(Email);
                
                //Setting the Primary Key
                dataTable.PrimaryKey = new DataColumn[] { Id };
                
                //Add New DataRow by creating the DataRow object
                DataRow row1 = dataTable.NewRow();
                row1["id"] = 1;
                row1["name"] = "Sani";
                row1["email"] = "delickate@hotmail.com";
                dataTable.Rows.Add(row1);
                //Adding new DataRow by simply adding the values
                dataTable.Rows.Add(1, "Hyne", "delickate@gmail.com");
                foreach (DataRow row in dataTable.Rows)
                {
                    Console.WriteLine(row["id"] + ",  " + row["name"] + ",  " + row["email"]);
                }


```

#### using stored procedure
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
        			SqlCommand cmd = new SqlCommand("spGetUsers", connection)
                    {
                        //Specify the command type as Stored Procedure
                        CommandType = CommandType.StoredProcedure
                    };
                    //Open the Connection
                    connection.Open();
                    //Execute the command i.e. Executing the Stored Procedure using ExecuteReader method
                    //SqlDataReader requires an active and open connection
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Read the data from the SqlDataReader 
                    //Read() method will returns true as long as data is there in the SqlDataReader
                    while (sdr.Read())
                    {
                        //Accessing the data using the string key as index
                        Console.WriteLine(sdr["id"] + ",  " + sdr["name"] + ",  " + sdr["email"] + ",  " + sdr["phone"]);
                        //Accessing the data using the integer index position as key
                        //Console.WriteLine(sdr[0] + ",  " + sdr[1] + ",  " + sdr[2] + ",  " + sdr[3]);
                    }
}

```

#### using transections
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
                connection.Open();
                // Create the transaction object by calling the BeginTransaction method on connection object
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    // Associate the first update command with the transaction
                    SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Balance = Balance - 500 WHERE AccountNumber = 'Account1'", connection, transaction);
                    //Execute the First Update Command
                    cmd.ExecuteNonQuery();
                    // Associate the second update command with the transaction
                    cmd = new SqlCommand("UPDATE Accounts SET Balance = Balance + 500 WHERE AccountNumber = 'Account2'", connection, transaction);
                    //Execute the Second Update Command
                    cmd.ExecuteNonQuery();
                    // If everythinhg goes well then commit the transaction
                    transaction.Commit();
                    Console.WriteLine("Transaction Committed");
                }
                catch(Exception EX)
                {
                    // If anything goes wrong, then Rollback the transaction
                    transaction.Rollback();
                    Console.WriteLine("Transaction Rollback");
                }
}

```

#### using commandbuilder
```
using System;
using System.Data.SqlClient;

string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
using (SqlConnection connection = new SqlConnection(connectionString))
{
                 SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Employee WHERE Gender='Male'", connection);
                //SqlDataAdapter dataAdapter = new SqlDataAdapter();
                //dataAdapter.SelectCommand = new SqlCommand("SELECT * FROM Employee WHERE Gender='Male'", connection);
                // Associate SqlDataAdapter object with SqlCommandBuilder.
                // At this point SqlCommandBuilder should generate T-SQL statements automatically
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                //SqlCommandBuilder commandBuilder = new SqlCommandBuilder();
                //commandBuilder.DataAdapter = dataAdapter;
                //Creating DataSet Object
                DataSet dataSet = new DataSet();
                //Filling the DataSet using the Fill Method of SqlDataAdapter object
                dataAdapter.Fill(dataSet);
                //Iterating through the DataSet 
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    //Accessing the Data using the string column name as key
                    Console.WriteLine($"Id: {row["Id"]}, Name: {row[1]}, Salary: {row[2]}, Gender: {row["Gender"]}, Department: {row["Department"]}");
                }
                //Now Update First Row i.e. Index Position 0
                DataRow dataRow = dataSet.Tables[0].Rows[0];
                dataRow["Name"] = "Name Updated";
                dataRow["Gender"] = "Female";
                dataRow["Salary"] = 50000;
                dataRow["Department"] = "Payroll";
                //Provide the DataSet and the DataTable name to the Update method
                //Here, SqlCommandBuilder will automatically generate the UPDATE SQL Statement 
                int rowsUpdated = dataAdapter.Update(dataSet, dataSet.Tables[0].TableName);
                //The GetUpdateCommand() method will return the auto generated UPDATE Command
                Console.WriteLine($"\nUPDATE Command: {commandBuilder.GetUpdateCommand().CommandText}");
                if (rowsUpdated == 0)
                {
                    Console.WriteLine("\nNo Rows Updated");
                }
                else
                {
                    Console.WriteLine($"\n{rowsUpdated} Row(s) Updated");
                }
                //First fetch the DataTable
                DataTable EmployeeTable = dataSet.Tables[0];
                //Create a new Row
                DataRow newRow = EmployeeTable.NewRow();
                newRow["Name"] = "Pranaya Rout";
                newRow["Gender"] = "Male";
                newRow["Salary"] = 450000;
                newRow["Department"] = "Payroll";
                EmployeeTable.Rows.Add(newRow);
                //Provide the DataSet and the DataTable name to the Update method
                //Here, SqlCommandBuilder will automatically generate the INSERT SQL Statement
                int rowsInserted = dataAdapter.Update(dataSet, dataSet.Tables[0].TableName);
                //The GetInsertCommand() method will return the auto generated INSERT Command
                Console.WriteLine($"\nINSERT Command: {commandBuilder.GetInsertCommand().CommandText}");
                if (rowsInserted == 0)
                {
                    Console.WriteLine("\nNo Rows Inserted");
                }
                else
                {
                    Console.WriteLine($"\n{rowsUpdated} Row(s) Inserted");
                }
                //Now Delete 3nd Row i.e. Index Position 2
                dataSet.Tables[0].Rows[2].Delete();
                //Provide the DataSet and the DataTable name to the Update method
                //Here, SqlCommandBuilder will automatically generate the DELETE SQL Statement
                int rowsDeleted = dataAdapter.Update(dataSet, dataSet.Tables[0].TableName);
                //The GetDeleteCommand() method will return the auto generated DELETE Command
                Console.WriteLine($"\nDELETE Command: {commandBuilder.GetDeleteCommand().CommandText}");
                if (rowsDeleted == 0)
                {
                    Console.WriteLine("\nNo Rows Deleted");
                }
                else
                {
                    Console.WriteLine($"\n{rowsUpdated} Row(s) Deleted");
                }
            }
            Console.ReadKey();

}

```



