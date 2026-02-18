SchoolSystem.Backend 🖥️⚙️

The backend service for the SchoolSystem platform. This repository handles API routing, request processing, authentication, and orchestration of business logic defined in the SchoolSystem.Domain
 layer.

Responsibilities 📌

API Layer — Exposes RESTful endpoints consumed by clients (web, mobile, or admin interfaces). Routes incoming requests and returns structured responses.

Application Logic — Coordinates use cases such as managing students, teachers, courses, enrollments, and grades by delegating to the domain layer.

Authentication & Authorization — Handles user identity, role-based access control, and session management to protect school data.

Data Persistence — Interfaces with the database to read and write school records, keeping infrastructure concerns separated from business rules.

Architecture 🏗️

This repo follows a clean, layered architecture and depends on SchoolSystem.Domain for all core business rules and entities. It does not contain domain logic itself — keeping concerns properly separated.
```
SchoolSystem.Backend
├── Controllers/     # API endpoints
├── Services/        # Application-level orchestration
├── Infrastructure/  # DB context, repositories, external services
└── Program.cs       # Entry point & DI configuration
```
Getting Started 🚀
git clone https://github.com/AbayleE/SchoolSystem.Backend.git
cd SchoolSystem.Backend

# Restore dependencies
```
dotnet restore
```

# Run the application
```
dotnet run
```

Configure your connection strings and environment settings in appsettings.json or via environment variables before running.

Related Repositories 🔗

SchoolSystem.Domain
 — Core business entities and domain logic
