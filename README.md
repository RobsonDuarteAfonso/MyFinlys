# 💰 MyFinlys

**MyFinlys** is a personal and family financial management application built with a Domain-Driven Design (DDD) architecture, using ASP.NET Core, Entity Framework Core and Angular.

## 📚 Overview

The application allows the management of:

- Bank accounts and their balances
- Users associated with accounts
- Financial events (monthly, weekly, biweekly)
- Financial records (`Register`)
- Installment calculation (`Installment`) and time-based tracking (month, week)

## 🧱 Architecture

The project follows a layered architecture:

```
MyFinlys/
├── MyFinlys.Api # API exposure layer (ASP.NET Core)
├── MyFinlys.Application # Application layer (DTOs, services)
├── MyFinlys.Domain # Domain layer (entities, VOs, repositories, business rules)
├── MyFinlys.Infrastructure # Persistence and database configuration layer (EF Core)
```

## 🛠️ Technologies Used

- [.NET 9.0](https://dotnet.microsoft.com) (⚠️ Preview version — consider using .NET 8 LTS in production)
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (or another relational database — configurable)
- C#

## 👨‍💻 Author

Developed by Robson Duarte Afonso

## 📄 License

This project is open for learning and technical development purposes. Licensing may be defined in the future.
