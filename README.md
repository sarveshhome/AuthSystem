# Auth System

A complete authentication and authorization system built with ASP.NET 9 and React, featuring token-based authentication, role-based authorization, and refresh token support.

## Project Structure

```
AuthSystem/
├── src/
│   ├── AuthSystem.Web/                 # React Vite project (client)
│   ├── AuthSystem.Api/                 # ASP.NET Core Web API
│   ├── AuthSystem.Core/                 # Domain layer
│   ├── AuthSystem.Infrastructure/       # Infrastructure layer
│   ├── AuthSystem.Application/          # Application layer
│   └── AuthSystem.sln                   # Solution file
├── tests/
│   ├── AuthSystem.Api.IntegrationTests/
│   └── AuthSystem.Application.UnitTests/
```



## Features

- 🔐 Token-based Authentication
- 🔑 Role-based Authorization (User, Admin)
- 🔄 Refresh Token Support
- ⚡ Clean Architecture
- 🧪 Integration & Unit Testing
- 🎯 React Frontend with Vite

## Technology Stack

### Backend
- ASP.NET 9
- Entity Framework Core
- JWT Authentication
- BCrypt for Password Hashing
- SQL Server
- Clean Architecture

### Frontend
- React 18
- Vite
- React Router
- Axios
- TailwindCSS

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js (Latest LTS)
- SQL Server
- Visual Studio Code or Visual Studio 2022

### Backend Setup

1. Clone the repository
```bash
git clone https://github.com/sarveshhome/AuthSystem.git
cd AuthSystem

2. Update the connection string in src/AuthSystem.Api/appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AuthSystemDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}

3. Apply database migrations

cd src/AuthSystem.Api
dotnet ef database update --project ../AuthSystem.Infrastructure --startup-project .

4. Run the API


dotnet run


```


Frontend Setup
Navigate to the web project

cd src/AuthSystem.Web


bash
Install dependencies

npm install


bash
Start the development server

npm run dev


bash
API Endpoints
Authentication
POST /api/auth/register - Register new user
POST /api/auth/login - Login user
POST /api/auth/refresh-token - Refresh access token


text
Protected Routes
GET /api/auth/authenticated-only - Requires authentication
GET /api/auth/admin-only - Requires admin role



text
Testing
Running Unit Tests
cd tests/AuthSystem.Application.UnitTests
dotnet test


Running Integration Tests
cd tests/AuthSystem.Api.IntegrationTests
dotnet test




# AuthSystem

A full-stack authentication system built with **ASP.NET Core** (Web API) and **React** (frontend) following Clean Architecture principles.

---

## 📁 Project Structure Details

### 🔹 AuthSystem.Core
- Contains domain entities
- Enums
- Domain interfaces

### 🔹 AuthSystem.Application
- DTOs
- Interfaces
- Services
- Business logic

### 🔹 AuthSystem.Infrastructure
- Database context
- Repositories
- External service implementations
- Migrations

### 🔹 AuthSystem.Api
- Controllers
- Middleware
- API configuration
- Dependency injection

### 🔹 AuthSystem.Web (React)
- React components
- Authentication state management
- API integration
- Routing

---

## 🔐 Security Features

- JWT token authentication
- Password hashing using **BCrypt**
- Refresh token rotation
- Role-based access control
- HTTPS enforcement
- Cross-Origin Resource Sharing (**CORS**) configuration

---

## 🧑‍💻 Development Guidelines

- Follow **Clean Architecture** principles
- Use **Dependency Injection**
- Write **unit tests** for business logic
- Write **integration tests** for APIs
- Follow **REST API** best practices
- Use **async/await** for database operations
- Implement **proper error handling**
- Use **DTOs** for data transfer
- Follow **secure coding practices**

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

---

## 📄 License

This project is licensed under the **MIT License** – see the [LICENSE](./LICENSE) file for details.

---

## 🙏 Acknowledgments

- [ASP.NET Core](https://github.com/dotnet/aspnetcore) team  
- [React](https://reactjs.org/) team  
- [Entity Framework Core](https://github.com/dotnet/efcore) team  
- Community contributors





This README.md provides:
- Clear project structure
- Setup instructions
- Available features
- API documentation
- Testing instructions
- Development guidelines
- Security features
- Contributing guidelines

You can customize it further based on your specific implementation and requirements. Let me know if you need any clarification or have specific sections you'd like to add or modify!



### React

```
yarn add axios react-router-dom @tanstack/react-query
```


Build and run with Docker Compose:

```
### Build and start containers
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```


Apply Content-Security-Policy, X-Content-Type-Options, X-Frame-Options ,Strict-Transport-Security in .net core

Referrer-Policy, Permissions-Policy in react application

```
Security Headers Added:
Content-Security-Policy (CSP): Prevents XSS attacks by controlling which resources can be loaded

default-src 'self': Only allow resources from same origin

script-src 'self': Only allow scripts from same origin

style-src 'self' 'unsafe-inline': Allow styles from same origin and inline styles

img-src 'self' data:: Allow images from same origin and data URLs

font-src 'self': Only allow fonts from same origin

connect-src 'self': Only allow connections to same origin

frame-ancestors 'none': Prevent embedding in frames

X-Content-Type-Options: nosniff prevents MIME type sniffing attacks

X-Frame-Options: DENY prevents clickjacking by blocking iframe embedding

Strict-Transport-Security (HSTS): Forces HTTPS for 1 year including subdomains
```