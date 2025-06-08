# 💇 HairSalon API

A RESTful API for managing a hair salon system including bookings, customers, employees, and services. Built with .NET and designed to be scalable, clean, and easy to integrate with frontend applications.

---

## ✨ Features

- 📅 Bookings: Create, update, and cancel appointments
- 👤 Customers: Register and manage customer profiles
- 💼 Employees: Manage salon staff and their availability
- 💇 Services: Define and organize available treatments

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API
- **Database:** SQL Server / PostgreSQL
- **ORM:** Entity Framework Core
- **Documentation:** Swagger / OpenAPI
- **Authentication (optional):** JWT / Identity

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- SQL Server / PostgreSQL
- (Optional) Postman for API testing

### Installation

```bash
git clone https://github.com/your-username/hairsalon-api.git
cd hairsalon-api
dotnet build
dotnet ef database update
dotnet run
