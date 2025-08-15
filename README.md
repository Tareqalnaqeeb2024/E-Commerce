# 🛍 E-Commerce API

**E-Commerce API** is a fully-featured e-commerce system built with **ASP.NET Core 8**, providing product, order, user, cart, and favorites management, real-time notifications via **SignalR**, complete authentication support (JWT & Google OAuth), and integration with external services like **RabbitMQ** and **Redis**.

---

## ✨ Key Features
- **User Management**:
  - Register and create a new account.
  - Login using password or Google OAuth.
  - Reset password via email and OTP verification.

- **Product & Category Management**:
  - Add, update, delete products and categories.
  - Search, browse, and paginate products.
  - View products by category or availability status.

- **Order, Cart & Favorites Management**:
  - Create and manage orders.
  - Add and remove products from the cart.
  - Add and remove products from favorites.
  - Adjust item quantities in the cart.

- **Real-Time Notifications**:
  - Receive live notifications for new orders or product updates via **SignalR**.

- **Background Services**:
  - Consume messages from RabbitMQ for order processing.
  - Retry logic can be implemented for fault tolerance.

- **Caching**:
  - Improve performance and reduce DB load with **Redis** caching.

---

## 🛠️ Technologies Used
- **Back-End**:
  - ASP.NET Core 8 Web API
  - Entity Framework Core (Code-First)
  - Identity (User & Role Management)
  - FluentValidation (Data Validation)
  - AutoMapper (Entity ↔ DTO Mapping)
  - SignalR (Real-Time Communication)
  - RabbitMQ (Message Queue for background processing)
  - Redis (Caching)

- **Database**:
  - SQL Server (LocalDB or remote)

- **Authentication**:
  - JWT Authentication
  - Google OAuth 2.0

- **Architecture**:
  - 3-Tier Architecture (Data Access, Business, API layers)
  - Repository Pattern + Unit of Work

---

## 📂 Project Structure
- **E_Commerce** → API Layer (Controllers, Middleware, Hubs, Startup)
- **E_CommerceDataAccess** → Data Access Layer (Repositories, DbContext, Models)
- **E_CommerceDataBusiness** → Business Layer (Services, Interfaces, Validators)
- **E_Commerce.Basic / MappingProfile / Extension** → Core configurations, AutoMapper profiles, extensions

---

## ⚡ RabbitMQ + Redis + SignalR Flow
1. When a new order is created, it’s published to **RabbitMQ**.
2. A background consumer processes the order and updates the database.
3. The updated data is cached in **Redis**.
4. A **SignalR** notification is sent to connected clients.

---

## 📌 Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "ConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ECommerce2;Integrated Security=True;",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "Issuer": "http://localhost:42713",
    "Audience": "http://localhost:55555",
    "Key": "YOUR_SECURE_KEY"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  },
  "MailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Email": "your_email@gmail.com",
    "Password": "your_app_password",
    "DisplayName": "E-Commerce App",
    "EnableSSL": true
  }
}
