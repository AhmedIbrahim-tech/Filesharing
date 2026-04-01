![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

# ☁️ FileSharing.net - Premium Cloud Management

Welcome to **FileSharing.net**, a professional-grade, high-performance web application built with **ASP.NET Core 10**. This platform demonstrates modern architectural patterns, clean code principles, and a stunning "Glassmorphism" UI design for a seamless user experience.

---

## 🚀 Key Takeaways & Architecture

This project has been fully refactored to follow **Clean Architecture** and industry best practices. It's more than just a file sharing tool—it's a demonstration of professional software engineering.

### 🏛️ Modern Architecture
- **Service-Oriented Design**: Business logic is decoupled from controllers using dedicated services (`AccountService`, `UploadService`, `MailServices`).
- **Data Persistence**: Uses **Entity Framework Core** with specialized `IEntityTypeConfiguration` and Fluent API for clean database schema management.
- **Dependency Injection Infrastructure**: Organized service registrations using clean `AddInfrastructure` extension methods in `Program.cs`.
- **C# 12+ Features**: extensive use of **Primary Constructors**, file-scoped namespaces, and Tuples for efficient, modern code.

### 🛡️ Security & Validation
- **FluentValidation**: Complete migration from DataAnnotations to a robust, separate validation layer for all ViewModels.
- **Identity & Auth**: Integrated **ASP.NET Core Identity** with role-based access control and **Social Authentication** (Google, Facebook).
- **Security Defaults**: Configured safe defaults, including secure-only cookies, anti-XSS measures, and open-redirect protection.

### 🎨 Premium UI/UX Design
- **Dark Mode Aesthetic**: A custom, stunning dark theme featuring **Glassmorphism**, vibrant gradients, and responsive grids.
- **Modern Typography**: Integrated high-end fonts like **Outfit** and **Inter** for exceptional readability.
- **Interactive UI**: Micro-animations using `Animate.css`, smooth transitions, and a custom-designed **Favicon/Brand Identity**.

---

## 🛠️ Project Structure
Below is a glimpse of how the professional refactoring has improved the project hierarchy:

![project structure](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/project%20structure.PNG)

---

## 📦 Tech Stack

### Backend
- **Core**: .NET 10, ASP.NET Core MVC
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity (Identity, Google, Facebook)
- **Validation**: FluentValidation.AspNetCore
- **Mail**: Async SMTP Integration

### Frontend
- **Styling**: Vanilla CSS3 (Modern Polish), Bootstrap 5 (Responsive Layouts)
- **Icons**: Bootstrap Icons, Font-Awesome 4
- **Animations**: Animate.min.css

---

## 🚦 Getting Started

### 1. Configure the Database
Update your `appsettings.json` with your local SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=FileSharingDB;..."
}
```

### 2. Apply Migrations
The application is configured to auto-run migrations on startup, but you can also manually run:
```bash
dotnet ef database update
```

### 3. Default Super Admin
A default Super Admin account is seeded on first run:
- **Email**: `admin@filesharing.com`
- **Password**: `Admin@123`

---

## 🖼️ Peek from the Design

![Home Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Home.jpg)

![Browse Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Browse.jpg)

![About Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/About.png)

![Search Results](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Search-Results.png)

![My Uploads](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/MyUpload.png)

![Contact Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Contact.jpg)

![Login Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Login.jpg)

![Register Page](https://github.com/AhmedIbrahim-tech/Filesharing/blob/master/Files/screenshort/Register.jpg)

---
*Followed by heart from Ahmed Ibrahim*
