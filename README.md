# TestTask API

## Setup

### Requirements

- [.NET 8.0 SDK or later version](https://dotnet.microsoft.com/download/dotnet)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Setup project

1. **Clone the repository:**

```bash
git clone https://github.com/naxiner/TestTask.git
cd TestTask
```

2. **Install dependencies:**

```bash
dotnet restore
```

3. **Set up a connection to the database.**

  Edit the `appsettings.json` file and configure the connection string to your database.

```json
"ConnectionStrings": {
	"DefaultConnection": "Server=your_server; Database=your_database; Trusted_Connection=True; TrustServerCertificate=True"
},
```

4. **Start the project:**

```bash
dotnet run
```

Your API will be available at the address `https://localhost:7250` or `http://localhost:5236`.

## API Documentation

### User endpoints
#### User registration

- **URL:** `/api/User/register`
- **Method:** `POST`
- **Request body:** 

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

- **Response:**

```json
{
  "message": "User registered successfully."
}
```

#### User login

- **URL:** `/api/User/login`
- **Method:** `POST`
- **Request body:** 

```json
{
  "usernameOrEmail": "string",
  "password": "string"
}
```

- **Response:**

```json
{
  "token": "user_jwt_token"
}
```

### Task endpoints

All operations with tasks are possible only for authorized users. The authorization token is automatically stored in a cookie
#### Create task

- **URL:** `/api/Task`
- Method:** `POST`
- **Request body:** 

```json
{
  "title": "string",
  "description": "string",
  "dueDate": "yyyy-mm-ddThh:mm:ss",
  "status": "0",
  "priority": "0"
}
```

- Response:**
  
```json
{
  "message": "Task added successfully."
}
```

### Retrieve a list of tasks

To retrieve a list of tasks for the authenticated user, with optional filters (e.g., status, due date, priority).

- **URL:** `/api/Task`
- **Method:** `GET`
- **Request parameters:** 

    - `status`: (0-2, optional)
    - `dueDate`: (yyyy-mm-ddThh:mm:ss, optional)
    - `priority`: (0-2, optional)

- **Response:**

```json
[
	{
		"id": "guid",
		"title": "string",
		"description": "string",
		"dueDate": "yyyy-mm-ddThh:mm:ss",
		"status": 0,
		"priority": 0,
		"createdAt": "yyyy-mm-ddThh:mm:ss",
		"updatedAt": "yyyy-mm-ddThh:mm:ss",
		"userId": "guid"
	}
]
```

### Retrieve task by id

- **URL:** `/api/Task/{id}`
- Method:** `GET`
- **Request parameters:** 

	- `id`: "guid"

- **Response:**
  
```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "dueDate": "yyyy-mm-ddThh:mm:ss",
  "status": 0,
  "priority": 0,
  "createdAt": "yyyy-mm-ddThh:mm:ss",
  "updatedAt": "yyyy-mm-ddThh:mm:ss",
  "userId": "guid"
}
```

### Update task by id

- **URL:** `/api/Task/{id}`
- Method:** `PUT`
- **Request parameters:** 

	- `id`: "guid"

- **Request body:** 

```json
{
  "title": "string",
  "description": "string",
  "dueDate": "yyyy-mm-ddThh:mm:ss",
  "status": 0,
  "priority": 0
}
```

- **Response:**
  
```json
{
  "message": "Task added successfully."
}
```

### Delete task by id

- **URL:** `/api/Task/{id}`
- Method:** `DELETE`
- **Request parameters:** 

	- `id`: "guid"

- **Response:**
  
```json
{
  "message": "Task deleted successfully."
}
```

## Explanation of Architecture and Design Solutions

### Architecture

The project is implemented using the ASP.NET Core Web-API, the main components include:

- **Controllers:** They are responsible for processing HTTP requests and returning responses.
- **Models:** Includes data stored in the database.
- **Repositories:** Provide access to the database and manipulate data.

### Design choices

- **JWT authentication:** It uses JSON Web Token (JWT) to provide authentication and authorization of users.
- **Entity Framework Core:** It is used to access the database and perform CRUD operations.
- **Logging:** Built-in logging is used to record important events and errors in the application.

### Configuration

The configuration is stored in `appsettings.json` or environment variables are used for security purposes.
