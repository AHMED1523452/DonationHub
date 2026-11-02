# 💙 DonationHub – Donation & Campaign Management System

**DonationHub** is a backend application built with **ASP.NET Core Web API**, designed to manage fundraising campaigns and donations through a structured and maintainable architecture.

The project focuses on applying modern backend development practices, including **Clean Architecture, Domain-Driven Design (DDD), Entity Framework Core, ASP.NET Core Identity, and domain-driven business rules**.

Beyond implementing CRUD operations, DonationHub emphasizes data integrity, secure configuration, concurrency handling, separation of concerns, and maintainable application design.

---

## 🎯 Project Overview

DonationHub provides a backend foundation for managing fundraising campaigns and their associated donations.

The system is designed around two main domain concepts:

- **Campaign:** Represents a fundraising initiative with a financial goal, collected amount, and lifecycle.
- **Donation:** Represents a financial contribution associated with a specific campaign.

The application enforces business rules to ensure that operations follow the domain's requirements and that entities cannot enter invalid states.

---

## ✨ Key Features

Features are listed according to the project's implementation progress.

- [ ] Campaign management.
- [ ] Donation management.
- [ ] Campaign lifecycle and status management.
- [ ] Donation lifecycle and status management.
- [ ] Monetary operations using a Money Value Object.
- [ ] User registration and management using ASP.NET Core Identity.
- [ ] JWT-based authentication and authorization.
- [ ] Entity Framework Core database integration.
- [ ] Optimistic concurrency control using SQL Server RowVersion.
- [ ] Testable date and time handling using TimeProvider.
- [ ] Strongly typed configuration using the Options Pattern.
- [ ] Secure configuration management using User Secrets.

> Update the checkboxes as features become implemented and tested.

---

## 🏛️ Architecture & Project Structure

DonationHub follows **Clean Architecture**, separating business logic from application orchestration, infrastructure concerns, and HTTP presentation.

```text
DonationHub/
│
├── DonationHub/                  
│   ├── Controllers/
│   ├── Middlewares/
│   ├── Extensions/
│   ├── Program.cs
│   └── ...
│
├── DonationHub.Application/
│   ├── Features/
│   ├── Interfaces/
│   ├── DTOs/
│   └── ...
│
├── DonationHub.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   └── ...
│
├── DonationHub.Infrastructure/
│   ├── Persistence/
│   ├── Configurations/
│   ├── Identity/
│   └── ...
│
└── DonationHub.slnx
```

### Layer Responsibilities

**1. Domain Layer**

Contains the core business model and rules.

- Domain entities and value objects.
- Business invariants.
- Entity behavior and state transitions.
- Domain-specific abstractions.

The Domain layer does not depend on Infrastructure or API implementation details.

**2. Application Layer**

Coordinates application use cases.

- Application services and handlers.
- Request and response models.
- Application interfaces and abstractions.
- Business use-case orchestration.
- Coordination between domain objects and persistence abstractions.

**3. Infrastructure Layer**

Implements technical dependencies.

- Entity Framework Core.
- SQL Server integration.
- Database configurations.
- ASP.NET Core Identity persistence.
- Repository implementations where required.
- External infrastructure services.

**4. API Layer**

Provides the HTTP interface.

- REST API controllers.
- HTTP request and response handling.
- Middleware configuration.
- Dependency injection registration.
- Authentication and authorization configuration.
- Application startup and configuration.

### Dependency Direction

The architecture follows the Dependency Inversion Principle:

```text
        API
         |
         v
    Application
         |
         v
       Domain
         ^
         |
   Infrastructure
```

Infrastructure implements abstractions defined by the inner layers rather than making the business logic depend on infrastructure details.

---

## 🧩 Domain Model

### Campaign

A Campaign represents a fundraising initiative.

Its responsibilities include:

- Maintaining the fundraising goal.
- Tracking the amount raised.
- Managing its current status.
- Enforcing rules regarding donations.
- Controlling valid lifecycle transitions.

### Donation

A Donation represents a financial contribution to a Campaign.

Its responsibilities include:

- Maintaining the donation amount.
- Referencing its associated campaign.
- Managing its payment or donation status.
- Enforcing valid status transitions.

### Entity Relationship

```text
        Campaign
            |
            | 1
            |
            | *
         Donation
```

A campaign can have multiple donations, while each donation belongs to one campaign.

---

## 💰 Money Value Object

The project uses the Value Object concept to model monetary information.

Instead of representing money using a standalone decimal value, a Money Value Object encapsulates both the amount and its currency.

Conceptual example:

```csharp
public sealed record Money(
    decimal Amount,
    string Currency
);
```

This approach provides:

- Consistent monetary representation.
- Currency-aware comparisons and operations.
- Value-based equality.
- Centralized validation.
- Reduced risk of invalid monetary calculations.

Campaign goals, raised amounts, and donation amounts can share the same monetary abstraction.

---

## 🔄 Domain Business Rules & State Transitions

Business rules are encapsulated within the domain model to prevent invalid operations.

### Donation Lifecycle

```text
             Pending
             /     \
            v       v
       Succeeded   Failed
```

### Campaign Lifecycle

```text
              Active
              /   \
             v     v
        Completed  Expired
```

The domain model is designed to enforce valid transitions.

For example:

- A successful donation cannot become pending again.
- A failed donation cannot become successful through an ordinary status update.
- A completed campaign cannot become active again through normal domain operations.
- An expired campaign cannot receive new donations.

Terminal states are protected from invalid transitions.

---

## 🔐 Authentication & Identity Management

The project uses **ASP.NET Core Identity** for user management.

Identity provides built-in functionality for:

- User registration.
- Password hashing and validation.
- User management.
- Username and email normalization.
- Role management.
- Authentication-related workflows.

### Username & Email Normalization

ASP.NET Core Identity uses `ILookupNormalizer` to normalize usernames and email addresses.

Its default implementation is `UpperInvariantLookupNormalizer`.

For example:

| Column | Example |
|---|---|
| UserName | Ahmed |
| NormalizedUserName | AHMED |
| Email | Ahmed@gmail.com |
| NormalizedEmail | AHMED@GMAIL.COM |

Normalization is handled automatically by `UserManager` during its user creation and update workflows.

### JWT Authentication

JWT authentication provides a token-based mechanism for securing protected endpoints.

Authentication and authorization are configured through ASP.NET Core's authentication middleware and Identity-related services.

---

## 🗄️ Database & Persistence

**Database:** Microsoft SQL Server

**ORM:** Entity Framework Core

The project follows the Code First approach, allowing the database schema to evolve alongside the application model.

Database responsibilities include:

- Entity configurations.
- Relationship mapping.
- Primary and foreign key constraints.
- Database migrations.
- Data persistence.
- Concurrency management.

### Optimistic Concurrency – RowVersion

The project uses SQL Server's `RowVersion` mechanism to detect conflicting updates.

Conceptual entity property:

```csharp
public byte[] RowVersion { get; private set; } = null!;
```

EF Core configuration:

```csharp
builder.Property(x => x.RowVersion)
       .IsRowVersion();
```

SQL Server generates and updates the RowVersion value automatically.

When two operations attempt to update the same record, EF Core compares the original version with the current database version.

If the record has changed, EF Core throws:

```csharp
DbUpdateConcurrencyException
```

This helps prevent the **Lost Update Problem**, where one user's update could unintentionally overwrite another user's changes.

---

## ⏰ Time Management & Testability

The project uses .NET's built-in `TimeProvider` abstraction for time-dependent application and domain operations.

Instead of directly depending on system time:

```csharp
DateTime.UtcNow
```

The application can inject `TimeProvider`:

```csharp
builder.Services.AddSingleton<TimeProvider>(
    TimeProvider.System
);
```

Usage:

```csharp
var currentTime = timeProvider.GetUtcNow();
```

Benefits:

- Consistent UTC time handling.
- Easier testing of time-dependent business rules.
- Ability to simulate different dates and times.
- Reduced direct dependency on system time.

---

## ⚙️ Configuration Management

The project applies the **Options Pattern** to bind configuration sections to strongly typed C# classes.

This provides:

- Strongly typed configuration.
- Centralized settings.
- Configuration validation.
- Better separation of concerns.

Example:

```csharp
services.AddOptions<JwtOptions>()
    .Bind(configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### Secure Secret Management

Sensitive values should never be hardcoded or committed to source control.

The project uses ASP.NET Core User Secrets during local development for sensitive configuration such as:

- JWT signing keys.
- Database credentials.
- Administrative seed passwords.

For production environments, secrets should be supplied through environment variables or a dedicated secret-management service.

---

## 🧠 Engineering Principles & Concepts

DonationHub is designed to apply practical backend engineering concepts, including:

| Concept | Application |
|---|---|
| Clean Architecture | Separation of application responsibilities |
| Domain-Driven Design | Domain entities and business rules |
| Encapsulation | Protecting entity state |
| Value Objects | Representing monetary values |
| Dependency Inversion | Depending on abstractions |
| Dependency Injection | Managing service dependencies |
| Entity Framework Core | Object-relational mapping |
| Identity | User and role management |
| JWT | Token-based authentication |
| Optimistic Concurrency | Preventing conflicting updates |
| TimeProvider | Testable time-dependent logic |
| Options Pattern | Strongly typed configuration |
| User Secrets | Secure local development configuration |
| Domain State Transitions | Preventing invalid lifecycle changes |

---

## 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| ASP.NET Core Web API | API development |
| .NET | Application platform |
| SQL Server | Relational database |
| Entity Framework Core | ORM and database access |
| ASP.NET Core Identity | User management |
| JWT | Authentication |
| Clean Architecture | Application structure |
| Domain-Driven Design | Domain modeling |
| Options Pattern | Configuration management |
| User Secrets | Local secret management |

---

## 🚀 Getting Started

### Prerequisites

Make sure you have:

- .NET SDK compatible with the project.
- Microsoft SQL Server.
- Visual Studio.
- Entity Framework Core tools.

### 1. Clone the Repository

```bash
git clone <YOUR_REPOSITORY_URL>
cd DonationHub
```

### 2. Configure User Secrets

Initialize User Secrets for the API startup project:

```bash
dotnet user-secrets init --project ./DonationHub
```

Add your required configuration values:

```bash
dotnet user-secrets set "Jwt:SigningKey" "YOUR_SECRET_KEY" --project ./DonationHub

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project ./DonationHub
```

Make sure the configuration keys match the ones used by your application.

### 3. Apply Database Migrations

```bash
dotnet ef database update
```

If your DbContext and migrations are located in separate projects, specify the appropriate `--project` and `--startup-project` arguments.

### 4. Run the Application

```bash
dotnet run --project ./DonationHub
```

---

## 🗺️ Roadmap

Potential future improvements:

- [ ] Complete campaign management workflows.
- [ ] Complete donation management workflows.
- [ ] Implement and verify authentication and authorization workflows.
- [ ] Add comprehensive validation and error handling.
- [ ] Add unit and integration tests.
- [ ] Improve API documentation with Swagger/OpenAPI.
- [ ] Add logging and monitoring.
- [ ] Add payment gateway integration.
- [ ] Add donation and campaign reporting.

---

## 🔒 Security

Security is an important part of the application's design.

- Sensitive configuration is kept outside source control.
- Password management is delegated to ASP.NET Core Identity.
- Protected endpoints require appropriate authentication and authorization.
- Database operations use EF Core.
- Concurrency conflicts are handled explicitly where required.

---

## 👨‍💻 Author

**Ahmed Mohamed Shaaban**

Backend Developer | C# | ASP.NET Core Web API

