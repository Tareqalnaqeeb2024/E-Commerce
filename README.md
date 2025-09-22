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
```
## 📡 API Documentation
🔐 Authentication & Account
| Method   | Endpoint                       | Description                                   |
| -------- | ------------------------------ | --------------------------------------------- |
| **POST** | `/api/Account/register`        | Register a new user.                          |
| **POST** | `/api/Account/login`           | Login and receive a JWT token.                |
| **POST** | `/api/Account/forgot-password` | Request a password reset (send OTP to email). |
| **POST** | `/api/Account/reset-password`  | Reset password using OTP.                     |
| **POST** | `/api/Account/verify-otp`      | Verify OTP code.                              |
| **GET**  | `/api/Account/google-login`    | Login using Google OAuth.                     |


🗂 Categories
| Method     | Endpoint               | Description            |
| ---------- | ---------------------- | ---------------------- |
| **GET**    | `/api/Categories`      | Get all categories.    |
| **GET**    | `/api/Categories/{id}` | Get category by ID.    |
| **POST**   | `/api/Categories`      | Create a new category. |
| **PUT**    | `/api/Categories/{id}` | Update a category.     |
| **DELETE** | `/api/Categories/{id}` | Delete a category.     |

🗂 Categories
| Method     | Endpoint                           | Description                 |
| ---------- | ---------------------------------- | --------------------------- |
| **GET**    | `/api/Products`                    | Get all products.           |
| **GET**    | `/api/Products/{id}`               | Get product by ID.          |
| **POST**   | `/api/Products`                    | Create a new product.       |
| **PUT**    | `/api/Products/{id}`               | Update a product.           |
| **DELETE** | `/api/Products/{id}`               | Delete a product.           |
| **GET**    | `/api/Products/category/{name}`    | Get products by category.   |
| **GET**    | `/api/Products/search?keyword=...` | Search products by keyword. |
| **GET**    | `/api/Products/paged`              | Get paginated products.     |

🛒 Cart
| Method     | Endpoint                          | Description               |
| ---------- | --------------------------------- | ------------------------- |
| **GET**    | `/api/Cart`                       | Get cart content.         |
| **POST**   | `/api/Cart`                       | Add product to cart.      |
| **DELETE** | `/api/Cart/{productId}`           | Remove product from cart. |
| **PUT**    | `/api/Cart/increment/{productId}` | Increase item quantity.   |
| **PUT**    | `/api/Cart/decrement/{productId}` | Decrease item quantity.   |

❤️ Favorites
| Method     | Endpoint                     | Description                    |
| ---------- | ---------------------------- | ------------------------------ |
| **POST**   | `/api/Favorites/{productId}` | Add product to favorites.      |
| **DELETE** | `/api/Favorites/{productId}` | Remove product from favorites. |
| **GET**    | `/api/Favorites`             | Get user's favorites.          |

📦 Orders
| Method     | Endpoint                  | Description                |
| ---------- | ------------------------- | -------------------------- |
| **GET**    | `/api/Orders`             | Get all orders (Admin).    |
| **GET**    | `/api/Orders/user`        | Get current user's orders. |
| **GET**    | `/api/Orders/{id}`        | Get order by ID.           |
| **POST**   | `/api/Orders`             | Create a new order.        |
| **PUT**    | `/api/Orders/{id}`        | Update an order.           |
| **DELETE** | `/api/Orders/{id}`        | Delete an order.           |
| **PUT**    | `/api/Orders/cancel/{id}` | Cancel an order.           |
| **GET**    | `/api/Orders/paged`       | Get paginated orders.      |

📡 SignalR Hubs
| Hub                 | Endpoint           | Description                                            |
| ------------------- | ------------------ | ------------------------------------------------------ |
| **NotificationHub** | `/notificationHub` | Send live notifications to users (e.g., order status). |
| **ProductHub**      | `/productHub`      | Real-time updates for product changes.                 |


# Clone the repository.
-git clone  https://github.com/Tareqalnaqeeb2024/E-Commerce

# Update appsettings.json with your local configurations (SQL Server, Redis, RabbitMQ, Google OAuth)

# Run database migrations
dotnet ef database update

# Start the project
dotnet run


