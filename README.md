# 💰 MyFinlys

**MyFinlys** é uma aplicação de controle financeiro pessoal e familiar, construída com uma arquitetura baseada em DDD (Domain-Driven Design), utilizando ASP.NET Core e Entity Framework Core.

## 📚 Visão Geral

A aplicação permite o gerenciamento de:

- Contas bancárias e seus saldos
- Usuários associados às contas
- Eventos financeiros (mensais, semanais, quinzenais)
- Lançamentos financeiros (`Register`)
- Cálculo de parcelas (`Installment`) e controle por período (mês, semana)

## 🧱 Arquitetura

O projeto segue uma arquitetura em camadas:

MyFinlys/
├── MyFinlys.Api # Camada de exposição da API (ASP.NET Core)
├── MyFinlys.Application # Camada de aplicação (DTOs, serviços)
├── MyFinlys.Domain # Camada de domínio (entidades, VOs, repositórios, regras de negócio)
├── MyFinlys.Infrastructure # Camada de persistência e configuração do banco (EF Core)

## 🛠️ Tecnologias Utilizadas

- [.NET 9.0](https://dotnet.microsoft.com) *(⚠️ em versão prévia — considere usar .NET 8 LTS em produção)*
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (ou outro banco relacional — configurável)
- C#

## 👨‍💻 Autor
Desenvolvido por Robson Duarte Afonso

## 📄 Licença
Este projeto é livre para fins de estudo e evolução técnica. Licenciamento poderá ser definido futuramente.


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

MyFinlys/
├── MyFinlys.Api # API exposure layer (ASP.NET Core)
├── MyFinlys.Application # Application layer (DTOs, services)
├── MyFinlys.Domain # Domain layer (entities, VOs, repositories, business rules)
├── MyFinlys.Infrastructure # Persistence and database configuration layer (EF Core)

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