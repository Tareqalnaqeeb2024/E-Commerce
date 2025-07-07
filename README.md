📌 Project Overview
This is a modern e-commerce API built with ASP.NET Core, featuring robust architecture with layered services, real-time capabilities, and scalable infrastructure. The solution includes user management, product catalog, order processing, cart functionality, and real-time notifications.

 ## Features
 - RESTful API with JWT Authentication
 - Real-time notifications via SignalR (NotificationHub, ProductHub)
 - Modular Architecture (Repositories, Services, DTOs, AutoMapper)
 - Order Processing with RabbitMQ (pub/sub)
 - Caching using Redis
 - Email Integration (SMTP) for user account workflows
   
# # Technologies
- Backend: ASP.NET Core 6+
- Database: SQL Server (Entity Framework Core)
- Auth: JWT + Identity
- Messaging: RabbitMQ
- Real-time: SignalR
- Caching: Redis
- DTOs: data transfer between layers.
- Validation: FluentValidation

## Architecture
The system follows Clean Architecture, separating concerns into distinct layers:
- **API Layer**: Handles API requests
- **Data Business Layer**: Business logic and Services
- **Data Access Layer**: Data persistence,Core entities
   ## Services
  Service	Description
- ProductService	Manage products & Categories
- OrderService	Handle orders 
-  CartService	User shopping cart operations
- UserService	User account management
- EmailService	Send transactional emails

