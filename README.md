# 🏦 Banking Management System

A secure and structured **Banking Management System** built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core**, and **SQL Server**. It features role-based user authentication via **ASP.NET Core Identity**, email confirmation, a dedicated service layer for business logic, and clean MVC architecture with ViewModels.

---

## 🚀 Tech Stack

| Layer            | Technology                                  |
|------------------|----------------------------------------------|
| Framework        | ASP.NET Core MVC (.NET 8)                    |
| Language         | C#                                           |
| ORM              | Entity Framework Core 8                      |
| Database         | SQL Server (SQLite also supported)           |
| Authentication   | ASP.NET Core Identity                        |
| Email Service    | Custom IEmailSender implementation           |
| UI               | Razor Views, HTML, CSS, Bootstrap            |
| Architecture     | MVC with Service Layer & ViewModels          |

---

## 📁 Project Structure
BankingManegmentSystem/
├── Areas/
│   └── Identity/
│       └── Pages/            # Identity Razor Pages (login, register, etc.)
├── Constants/                # Role names and app-wide constants
├── Controllers/              # MVC Controllers
├── Data/
│   └── AppDbContext.cs       # EF Core database context
├── Helper/                   # Utility/helper classes
├── Migrations/               # EF Core database migrations
├── Models/                   # Entity/domain models
├── Service/
│   └── EmailSender.cs        # Custom email sender service
├── ViewModels/               # View-specific models (clean data binding)
├── Views/                    # Razor Views
├── wwwroot/                  # Static files (CSS, JS, images)
├── Program.cs                # App entry point & service registration
├── appsettings.json          # Configuration & DB connection string
└── BankingManegmentSystem.csproj

---

## ✅ Features

### 🔐 Authentication & Security
- User registration with **email confirmation** required before login
- Role-based authorization (**Admin**, **User**, etc.)
- Custom login path: `/login`
- Custom access-denied path: `/access-denied`
- Secure cookie configuration for Identity

### 🏧 Banking Operations
- Account creation and management
- Deposit, withdrawal, and fund transfer
- Transaction history tracking
- Account balance overview

### 🛠️ Admin Panel
- Manage users and roles
- View all accounts and transactions
- System-wide oversight and control

### 🧱 Clean Architecture
- **Service Layer** — business logic separated from controllers
- **ViewModels** — clean, type-safe data binding for views
- **Constants** — centralized role and configuration values
- **Helpers** — reusable utility functions

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup Steps

```bash
# 1. Clone the repository
git clone https://github.com/rmezanur521-boop/BankingManegmentSystem.git
cd BankingManegmentSystem
```

**2. Update connection string** in `appsettings.json`:

```json
"ConnectionStrings": {
  "db-connection": "Server=YOUR_SERVER;Database=BankingDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

```bash
# 3. Apply database migrations
dotnet ef database update

# 4. Run the application
dotnet run
```

App will be available at: **https://localhost:7xxx** (shown in terminal)

---

## 📦 NuGet Packages

| Package | Version |
|---|---|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.23 |
| Microsoft.AspNetCore.Identity.UI | 8.0.23 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.23 |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.23 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.23 |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.19 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.23 |

---

## 🗺️ Key Routes

| Path | Description |
|---|---|
| `/` | Home / Dashboard |
| `/login` | Custom login page |
| `/access-denied` | Access denied page |
| `/Identity/Account/Register` | User registration |
| `/Identity/Account/ConfirmEmail` | Email confirmation |

---

## 👤 Author

**Mezanur Rahman**
GitHub: [@rmezanur521-boop](https://github.com/rmezanur521-boop)

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
