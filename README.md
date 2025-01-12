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
3. [Database Schema](#database-schema)
4. [Usage](#usage)
5. [Directory Structure](#directory-structure)
6. [Helper Functions](#helper-functions)
7. [Contributing](#contributing)
8. [License](#license)

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

