# E-Commerce_WebAPI
 -E-Commerce Restful API with .NET 8, utilizes AspNetCore.Identity for user management.
 -This API allow users to view products by categories and brand, manage carts, and complete purchases checkout.

 ## Features
 - User registeration and account management.
 - Product listing and categorization by Category or Brand.
 - Orders management.
 - Cart and Shopping functionality.
 -  Checkout .
   
# # Technologies
- ASP .NET core 8 WebApi.
- AspNetCore.Identity: for authentication and authorization.
- EF core as ORM.
- SQL Server as DBMS.
- JWT: as token-based authentication.
- Automapper:  For object-object mapping.
- DTOs: data transfer between layers.
- Clean architecture.

## Architecture
The system follows Clean Architecture, separating concerns into distinct layers:
- **API Layer**: Handles API requests
- **Data Business Layer**: Business logic and Services
- **Data Access Layer**: Data persistence,Core entities
   
