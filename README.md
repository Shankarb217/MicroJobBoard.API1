# Micro Job Board - Backend API

ASP.NET Core 8 Web API for the Micro Job Board platform.

## Tech Stack

- **ASP.NET Core 8** - Web API Framework
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **JWT Authentication** - Security
- **AutoMapper** - Object Mapping
- **FluentValidation** - Validation
- **BCrypt.Net** - Password Hashing
- **Swagger/OpenAPI** - API Documentation

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Restore packages:**
```bash
dotnet restore
```

2. **Update database connection string** in `appsettings.json` if needed

3. **Create database and run migrations:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Run the application:**
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` or `https://localhost:5001`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Jobs
- `GET /api/jobs` - Get all approved jobs (with filters)
- `GET /api/jobs/{id}` - Get job by ID
- `POST /api/jobs` - Create job (Employer/Admin)
- `PUT /api/jobs/{id}` - Update job (Employer/Admin)
- `DELETE /api/jobs/{id}` - Delete job (Employer/Admin)
- `GET /api/my-jobs` - Get employer's jobs (Employer/Admin)

### Applications
- `POST /api/jobs/{jobId}/apply` - Apply to job (Seeker)
- `GET /api/my-applications` - Get seeker's applications (Seeker)
- `GET /api/jobs/{jobId}/applications` - Get job applications (Employer/Admin)
- `PUT /api/applications/{id}/status` - Update application status (Employer/Admin)

### Admin
- `GET /api/admin/users` - Get all users
- `PUT /api/admin/users/{id}/role` - Update user role
- `GET /api/admin/pending-jobs` - Get pending jobs
- `PUT /api/admin/jobs/{id}/approve` - Approve job
- `GET /api/admin/dashboard/stats` - Get dashboard statistics
- `GET /api/admin/reports` - Get reports

## Database Schema

### Users
- Id, FullName, Email, PasswordHash, Role, Phone, Location, Bio, CreatedAt, UpdatedAt

### Jobs
- Id, Title, Company, Location, JobType, Category, Salary, Description, Status, PostedDate, EmployerId

### Applications
- Id, CoverLetter, Status, AppliedDate, JobId, ApplicantId

### Reports
- Id, ReportType, Data, GeneratedDate

## Authentication

The API uses JWT Bearer tokens for authentication.

### Test Credentials

**Admin:**
- Email: admin@example.com
- Password: admin123

**Employer:**
- Email: employer@example.com
- Password: employer123

**Job Seeker:**
- Email: seeker@example.com
- Password: seeker123

### Using Authentication

1. Login via `/api/auth/login` to get a JWT token
2. Include the token in subsequent requests:
   ```
   Authorization: Bearer <your-token>
   ```

## Project Structure

```
MicroJobBoard.API/
├── Controllers/          # API Controllers
├── Models/              # Domain Models
├── DTOs/                # Data Transfer Objects
├── Services/            # Business Logic
│   ├── Interfaces/
│   └── Implementations/
├── Data/                # Database Context & Seed Data
├── Mappings/            # AutoMapper Profiles
├── Properties/          # Launch Settings
├── Program.cs           # Application Entry Point
└── appsettings.json     # Configuration
```

## Development

### Adding Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Seeding Data

Seed data is automatically applied on first run. See `Data/SeedData.cs`.

### Running Tests

```bash
dotnet test
```

## CORS Configuration

The API is configured to accept requests from:
- `http://localhost:3000` (React dev server - Vite)
- `http://localhost:5173` (React dev server - alternative)

Update `appsettings.json` to add more origins if needed.

## Security

- Passwords are hashed using BCrypt
- JWT tokens expire after 60 minutes (configurable)
- Role-based authorization is enforced
- SQL injection prevention via EF Core parameterized queries
- HTTP deployment (HTTPS optional for production)

## Deployment

### AWS EC2 Deployment

This project includes automated CI/CD deployment to AWS EC2 with RDS MSSQL Server.

**📚 Deployment Guides:**
- **[DEPLOYMENT.md](./DEPLOYMENT.md)** - Complete EC2 setup and deployment guide
- **[GITHUB_SECRETS_SETUP.md](./GITHUB_SECRETS_SETUP.md)** - GitHub secrets configuration
- **[HTTP_DEPLOYMENT_NOTE.md](./HTTP_DEPLOYMENT_NOTE.md)** - HTTP-only deployment details
- **[microjobboard-api.service](./microjobboard-api.service)** - Systemd service file

**Quick Start:**
1. Set up EC2 instance with .NET 8 runtime
2. Configure GitHub secrets (see GITHUB_SECRETS_SETUP.md)
3. Copy systemd service file to EC2
4. Push to `main` branch to trigger deployment

**GitHub Actions Workflow:**
- Automatically builds, tests, and deploys on push to `main`
- Uses systemd for service management
- Dynamically generates production config from secrets

### Manual Publish

```bash
dotnet publish -c Release -o ./publish
```

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Check connection string in `appsettings.json`
- Verify database exists

### Migration Issues
```bash
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### CORS Issues
- Check allowed origins in `appsettings.json`
- Ensure frontend URL matches exactly

## License

MIT
