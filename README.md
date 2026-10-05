# Motor Insurance Application

A production-style motor insurance management system built with **ASP.NET Core 8, C#, Entity Framework Core, PostgreSQL, Redis, Docker, CQRS, React and TypeScript**.

The project is designed to demonstrate practical **enterprise backend development**, Clean Architecture, secure API development, relational database design, testing, containerization, and modern software-engineering practices.

> **Project Status:** 🚧 In Development

---

## Overview

The Motor Insurance Application provides a backend platform for managing motor insurance policies and their associated business entities.

The system is designed around the following core concepts:

* Policy Holders
* Insurance Policies
* Vehicles
* Drivers
* Claims
* Telematics

The backend is built using **Clean Architecture** with a strong separation between business logic, application use cases, infrastructure concerns and API delivery.

A React/TypeScript frontend will consume the ASP.NET Core Web API once the core backend has been substantially completed.

---

## Technology Stack

### Backend

| Technology                | Purpose                            |
| ------------------------- | ---------------------------------- |
| **C#**                    | Primary programming language       |
| **.NET 8**                | Application platform               |
| **ASP.NET Core Web API**  | REST API                           |
| **Entity Framework Core** | ORM and data access                |
| **PostgreSQL**            | Relational database                |
| **MediatR**               | CQRS and application messaging     |
| **FluentValidation**      | Request validation                 |
| **AutoWrapper**           | Consistent API responses           |
| **ASP.NET Core Identity** | User management and authentication |
| **JWT**                   | API authentication                 |
| **Redis**                 | Distributed caching                |
| **Serilog**               | Structured logging                 |
| **Swagger / OpenAPI**     | API documentation                  |

### Infrastructure

| Technology         | Purpose                            |
| ------------------ | ---------------------------------- |
| **Docker**         | Containerization                   |
| **Docker Compose** | Local infrastructure orchestration |
| **PostgreSQL**     | Persistent relational storage      |
| **Redis**          | Distributed cache                  |

### Testing

| Technology                              | Purpose           |
| --------------------------------------- | ----------------- |
| **xUnit**                               | Automated testing |
| **Moq / appropriate mocking framework** | Test isolation    |

### Frontend

| Technology              | Purpose                        |
| ----------------------- | ------------------------------ |
| **React**               | UI framework                   |
| **TypeScript**          | Type-safe frontend development |
| **Vite**                | Frontend tooling               |
| **React Router**        | Client-side routing            |
| **Axios / HTTP client** | API communication              |
| **React Hook Form**     | Form management                |

---

# Architecture

The backend follows **Clean Architecture**.

```text
                    ┌─────────────────────┐
                    │     React + TS      │
                    │      Frontend       │
                    └──────────┬──────────┘
                               │ HTTP
                               ▼
                    ┌─────────────────────┐
                    │         API         │
                    │   ASP.NET Core      │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │    Application      │
                    │ CQRS / MediatR /    │
                    │ Validation / DTOs   │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │       Domain        │
                    │ Business Entities   │
                    │ & Rules              │
                    └─────────────────────┘
                               ▲
                               │
                    ┌──────────┴──────────┐
                    │   Infrastructure    │
                    │                     │
                    │ EF Core             │
                    │ PostgreSQL          │
                    │ Redis               │
                    │ Identity            │
                    └─────────────────────┘
```

### Projects

```text
MotorInsurance
│
├── MotorInsurance.Domain
├── MotorInsurance.Application
├── MotorInsurance.Infrastructure
└── MotorInsurance.API
```

### Dependency Direction

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application / Domain
```

The **Domain** remains independent from infrastructure and framework-specific implementation details.

---

# Clean Architecture

The solution separates responsibilities into four primary layers.

## Domain

Contains the core business model.

Responsibilities include:

* Entities
* Domain concepts
* Business rules
* Enumerations
* Domain-level abstractions where appropriate

The Domain should not depend on:

* Entity Framework Core
* PostgreSQL
* Redis
* ASP.NET Core
* MediatR
* API-specific concerns

---

## Application

Contains application use cases and orchestration.

Responsibilities include:

* Commands
* Queries
* Handlers
* DTOs
* Validators
* Application interfaces
* Mapping
* Pipeline behaviors

Example:

```text
CreatePolicyCommand
        ↓
CreatePolicyCommandHandler
        ↓
Application abstractions
        ↓
Infrastructure implementation
```

---

## Infrastructure

Contains implementations of technical concerns.

Responsibilities include:

* Entity Framework Core
* PostgreSQL
* Database configurations
* Migrations
* ASP.NET Core Identity
* Redis
* External infrastructure
* Repository/data-access implementations where appropriate

The `ApplicationDbContext` lives in Infrastructure.

EF Core migrations are also maintained by Infrastructure.

A design-time factory is used so EF Core can create the `DbContext` without requiring the API application to start.

---

## API

Contains the HTTP-facing application.

Responsibilities include:

* Controllers
* Middleware
* Dependency injection configuration
* Authentication configuration
* Authorization configuration
* Swagger/OpenAPI
* HTTP pipeline configuration
* Application startup

The API should remain thin and delegate business operations to the Application layer.

---

# Domain Model

The initial business model contains:

```text
PolicyHolder
     │
     └───< Policy
              │
              ├───< Vehicle
              │        │
              │        └───< Telematics
              │
              ├───< Driver
              │
              └───< Claim
```

The relationships are deliberately evaluated from a business perspective rather than blindly creating database relationships.

Important considerations include:

* A policy holder may have multiple policies.
* A policy may cover multiple vehicles depending on the insurance model.
* Policies may have multiple drivers.
* Drivers and vehicles may require explicit association modelling when the business relationship becomes many-to-many.
* Claims belong to the appropriate insured policy/vehicle context.
* Telematics represents historical vehicle-related data rather than simply storing one mutable record.
* Policy status represents the lifecycle of an insurance policy.

The final relational model is designed around **referential integrity, historical data requirements and business rules**.

---

# CQRS

The application uses **Command Query Responsibility Segregation (CQRS)** through MediatR.

Commands modify state:

```text
CreatePolicyCommand
UpdatePolicyCommand
CancelPolicyCommand
CreateClaimCommand
```

Queries retrieve information:

```text
GetPolicyByIdQuery
GetPoliciesQuery
GetVehicleByIdQuery
GetClaimsQuery
```

Example structure:

```text
Application
│
├── Policies
│   ├── Commands
│   │   ├── CreatePolicy
│   │   │   ├── CreatePolicyCommand.cs
│   │   │   ├── CreatePolicyCommandHandler.cs
│   │   │   └── CreatePolicyCommandValidator.cs
│   │   │
│   │   └── UpdatePolicy
│   │
│   └── Queries
│       ├── GetPolicyById
│       │   ├── GetPolicyByIdQuery.cs
│       │   └── GetPolicyByIdQueryHandler.cs
│       │
│       └── GetPolicies
```

CQRS is used where it provides useful separation between application operations rather than introducing unnecessary complexity.

---

# Validation

The application uses **FluentValidation** for application-level request validation.

Example:

```text
CreatePolicyCommand
        ↓
CreatePolicyCommandValidator
        ↓
CreatePolicyCommandHandler
```

Validation responsibilities are separated across appropriate boundaries.

### Client-side validation

Improves user experience.

### API validation

Protects the API from invalid requests.

### Business validation

Ensures application/business rules are respected.

### Database constraints

Provide the final persistence-level integrity guarantees.

Frontend validation is therefore **never treated as the only line of defence**.

---

# Authentication & Authorization

Authentication is implemented using **ASP.NET Core Identity** and JWT-based authentication.

The security architecture distinguishes between:

### Authentication

> Who are you?

### Authorization

> Are you allowed to perform this operation?

The application uses concepts including:

* Users
* Password hashing
* Roles
* Claims
* Access tokens
* JWT authentication
* Authorization policies
* Protected endpoints

Where appropriate, refresh tokens can be used to support secure long-lived sessions without unnecessarily extending access-token lifetimes.

---

# Database

The application uses:

**PostgreSQL**

running inside Docker.

Entity Framework Core is used with the **Code First** approach.

Database responsibilities include:

* Entity configuration
* Relationships
* Foreign keys
* Indexes
* Constraints
* Migrations
* PostgreSQL persistence

Example migration commands:

```bash
dotnet ef migrations add InitialCreate --project src/MotorInsurance.Infrastructure
```

Apply migrations:

```bash
dotnet ef database update --project src/MotorInsurance.Infrastructure
```

The Infrastructure project owns the EF Core `DbContext` and migrations.

A design-time `IDesignTimeDbContextFactory<ApplicationDbContext>` allows migrations to be created without booting the API.

---

# Redis

Redis is introduced only where caching provides a meaningful benefit.

Potential use cases include:

* Frequently accessed policy information
* Reference data
* Read-heavy endpoints
* Appropriate temporary application data

Caching decisions consider:

1. What data is expensive or unnecessary to retrieve repeatedly?
2. How frequently does the data change?
3. What is the acceptable staleness?
4. How will cache invalidation work?
5. What happens when Redis is unavailable?

Redis is therefore treated as an **optimization**, not the primary source of truth.

PostgreSQL remains the authoritative persistent datastore.

---

# API Response Handling

The API uses consistent response handling to make the API predictable for consumers.

Responses distinguish between:

```text
200 OK
201 Created
400 Bad Request
401 Unauthorized
403 Forbidden
404 Not Found
409 Conflict
500 Internal Server Error
```

Validation and error responses are standardized where appropriate using AutoWrapper and the application's exception-handling strategy.

This provides a more predictable contract for frontend applications and external API consumers.

---

# Docker

The development environment uses Docker Compose for infrastructure services.

Example:

```text
Docker Compose
│
├── PostgreSQL
│
└── Redis
```

The application can therefore run against consistent development infrastructure without requiring developers to manually install PostgreSQL or Redis on the host machine.

Typical commands:

```bash
docker compose up -d
```

View running containers:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs
```

Stop infrastructure:

```bash
docker compose down
```

---

# API Documentation

Swagger / OpenAPI is used to document and test the API during development.

When the API is running, Swagger UI is available at:

```text
https://localhost:<port>/swagger
```

Swagger provides visibility into:

* Available endpoints
* HTTP methods
* Request models
* Response models
* Authentication requirements
* API contracts

---

# Testing Strategy

The project uses automated testing to verify application behaviour.

Testing focuses on important business and application behaviour rather than simply maximizing code coverage.

Potential test layers include:

```text
Unit Tests
    ↓
Application Tests
    ↓
Integration Tests
    ↓
API Tests
```

### Unit Tests

Used for isolated application behaviour.

### Integration Tests

Used to verify interactions between components such as:

* EF Core
* PostgreSQL
* Application services
* Infrastructure

### API Tests

Used to verify HTTP behaviour and API contracts.

The goal is to develop confidence that changes do not silently break existing functionality.

---

# Git Workflow

Development follows an incremental, ticket-based workflow.

Example:

```text
Ticket 001 — Solution & Architecture Setup
Ticket 002 — Domain Entities
Ticket 003 — EF Core Configuration
Ticket 004 — PostgreSQL Integration
Ticket 005 — Policy Commands
Ticket 006 — Policy Queries
Ticket 007 — Authentication
...
```

Changes are committed in focused increments.

Example:

```bash
git add .
git commit -m "feat: add policy domain entity"
```

Commit messages aim to communicate the intent of the change clearly.

---

# Development Workflow

The project is developed incrementally rather than attempting to build the entire system at once.

The general workflow is:

```text
Business Requirement
        ↓
Ticket
        ↓
Design
        ↓
Implementation
        ↓
Validation
        ↓
Testing
        ↓
Database / Infrastructure
        ↓
API
        ↓
Git Commit
```

Each feature is evaluated from both a technical and business perspective.

---

# Project Development Phases

## Phase 1 — Architecture

* Solution structure
* Clean Architecture
* Project dependencies
* Dependency Injection
* Configuration

## Phase 2 — Domain

* Policy Holder
* Policy
* Vehicle
* Driver
* Claim
* Telematics
* Relationships
* Business rules

## Phase 3 — Persistence

* EF Core
* PostgreSQL
* Entity configurations
* Relationships
* Indexes
* Constraints
* Migrations

## Phase 4 — Application

* CQRS
* MediatR
* Commands
* Queries
* Handlers
* DTOs
* FluentValidation

## Phase 5 — API

* Controllers
* Swagger
* API responses
* Exception handling
* Logging

## Phase 6 — Security

* ASP.NET Core Identity
* Registration
* Login
* JWT
* Roles
* Claims
* Authorization

## Phase 7 — Infrastructure

* Docker
* PostgreSQL
* Redis
* Caching
* Logging

## Phase 8 — Testing

* Unit tests
* Integration tests
* API tests

## Phase 9 — Frontend

* React
* TypeScript
* Vite
* React Router
* API integration
* Forms
* Authentication
* State management

---

# Running the Project

## Prerequisites

Install:

* [.NET 8 SDK](https://dotnet.microsoft.com/)
* [Docker](https://www.docker.com/)
* Git
* Node.js
* npm

Verify .NET:

```bash
dotnet --version
```

Verify Docker:

```bash
docker --version
```

Verify Node:

```bash
node --version
```

---

## Clone the Repository

```bash
git clone https://github.com/PLThabangR/MotorInsurance.git
```

Navigate into the project:

```bash
cd MotorInsurance
```

---

## Start Infrastructure

```bash
docker compose up -d
```

Verify the containers:

```bash
docker compose ps
```

---

## Configure the API

Connection strings and environment-specific configuration should be supplied through configuration rather than hard-coded into source code.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=motorinsurance;Username=postgres;Password=your-password"
  }
}
```

> **Never commit production credentials, passwords, JWT signing keys or other secrets to Git.**

Use environment variables or an appropriate secret-management solution for sensitive configuration.

---

## Apply Database Migrations

From the solution directory:

```bash
dotnet ef database update --project src/MotorInsurance.Infrastructure
```

Create a new migration:

```bash
dotnet ef migrations add MigrationName --project src/MotorInsurance.Infrastructure
```

---

## Run the API

```bash
dotnet run --project src/MotorInsurance.API
```

Then open Swagger:

```text
https://localhost:<port>/swagger
```

---

# Project Structure

```text
MotorInsurance/
│
├── src/
│   │
│   ├── MotorInsurance.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── ...
│   │
│   ├── MotorInsurance.Application/
│   │   ├── Common/
│   │   ├── Policies/
│   │   ├── Vehicles/
│   │   ├── Drivers/
│   │   ├── Claims/
│   │   └── ...
│   │
│   ├── MotorInsurance.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Migrations/
│   │   ├── Identity/
│   │   ├── Caching/
│   │   └── ...
│   │
│   └── MotorInsurance.API/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Extensions/
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   ├── MotorInsurance.UnitTests/
│   ├── MotorInsurance.IntegrationTests/
│   └── ...
│
├── frontend/
│   └── ...
│
├── docker-compose.yml
├── .gitignore
├── README.md
└── MotorInsurance.sln
```

---

# Engineering Principles

This project intentionally focuses on practical engineering principles.

### Separation of Concerns

Each layer has a clearly defined responsibility.

### Dependency Inversion

High-level application logic should not depend directly on infrastructure implementations.

### Explicit Dependencies

Dependencies are provided through dependency injection rather than hidden global state.

### Secure by Default

Authentication, authorization, validation and secret management are treated as first-class concerns.

### Maintainability

Code is structured to make future changes easier rather than optimizing solely for initial development speed.

### Pragmatic Architecture

Patterns are introduced because they solve a problem, not simply because they are popular.

The project deliberately avoids unnecessary architectural complexity and does **not** attempt to implement DDD tactical patterns simply for the sake of demonstrating them.

---

# What This Project Demonstrates

This project is intended to demonstrate practical experience with:

* C# and modern .NET
* ASP.NET Core Web API
* Clean Architecture
* REST API design
* CQRS
* MediatR
* Entity Framework Core
* PostgreSQL
* Database migrations
* Relational database modelling
* Redis caching
* Docker
* Docker Compose
* Authentication and authorization
* ASP.NET Core Identity
* JWT
* FluentValidation
* API error handling
* Swagger/OpenAPI
* Structured logging
* Unit testing
* Integration testing
* React
* TypeScript
* Git
* Production-oriented development practices

---

# Future Improvements

Potential future improvements include:

* Refresh token rotation
* More comprehensive integration testing
* API rate limiting
* Health checks
* Observability and metrics
* CI/CD pipeline
* Cloud deployment
* Infrastructure as Code
* Azure hosting
* Automated database migration strategy
* More advanced caching strategies
* Background processing for appropriate workloads

These features will only be introduced where they provide a meaningful engineering or business benefit.

---

# Learning Objectives

This project is being developed as both a portfolio application and a practical software-engineering exercise.

The primary objectives are to develop deeper understanding of:

* How Clean Architecture works in a real application
* How ASP.NET Core applications are structured
* How CQRS works in practice
* How EF Core interacts with relational databases
* How authentication and authorization work
* How Redis fits into a distributed application
* How Docker is used during development
* How to design maintainable APIs
* How to test application behaviour
* How frontend applications consume backend APIs
* How professional Git workflows support software development
* How to reason about architectural trade-offs

---

# Author

**Thabang Rakgoropo**

Software Developer | C# / .NET | ASP.NET Core | React | TypeScript

GitHub: [PLThabangR](https://github.com/PLThabangR)

---

## License

This project is intended primarily as a portfolio and educational software-engineering project.
