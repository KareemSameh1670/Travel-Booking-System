Travel Booking API 🛫🏨
A complete travel booking system built with ASP.NET Core 9.0, featuring flight and hotel bookings, user authentication with Identity, payment processing, and admin management.

🚀 Features
Built-in Authentication - ASP.NET Core Identity with cookie-based authentication

Flight Management - Search, book, and manage flights

Hotel Management - Search, book, and manage hotels

Booking System - Complete booking workflow with status management

Payment Processing - Simulated payment gateway integration

Admin Dashboard - Comprehensive admin interface for management

RESTful API - Clean, well-structured REST endpoints

Swagger Documentation - Interactive API documentation

🏗️ Architecture
This project follows N-Tier Architecture with clear separation of concerns:

text
TravelBookingSolution/
├── TravelBooking.API/           # Presentation Layer (Controllers)
├── TravelBooking.Application/   # Business Logic Layer (Services, DTOs)
├── TravelBooking.Domain/        # Domain Layer (Entities, Interfaces)
└── TravelBooking.Infrastructure/# Data Access Layer (Repositories, DbContext)
🛠️ Technology Stack
Framework: ASP.NET Core 9.0

Authentication: ASP.NET Core Identity with cookie authentication

Database: SQL Server with Entity Framework Core

Validation: Data Annotations

Mapping: AutoMapper for object-object mapping

Documentation: Swagger/OpenAPI

📋 Prerequisites
.NET 9.0 SDK

SQL Server or SQL Server LocalDB

Visual Studio 2022 or VS Code

⚡ Quick Start
1. Clone the Repository
bash
git clone https://github.com/yourusername/travel-booking-api.git
cd travel-booking-api
2. Configure Database
Update the connection string in TravelBooking.API/appsettings.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TravelBookingDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
3. Run Database Migrations
bash
dotnet ef database update --project TravelBooking.Infrastructure --startup-project TravelBooking.API
4. Run the Application
bash
dotnet run --project TravelBooking.API
The API will be available at:

Swagger UI: https://localhost:7064

Health Check: https://localhost:7064/health

🔐 Authentication
Built-in Identity Features
Cookie-based authentication

Password hashing and validation

User role management

Account lockout protection

Ready for email confirmation and 2FA

Default Accounts (Auto-created)
Admin Account:

Email: admin@travelbooking.com

Password: Admin123!

Role: Admin

Test Customer Account:

Register any email through the API

Default role: Customer

User Roles
Admin: Full system access (manage flights, hotels, users, bookings)

Customer: Book flights/hotels, view personal bookings and payments

📚 API Endpoints
Account Management
Method	Endpoint	Description	Access
POST	/api/Account/register	Register new user	Public
POST	/api/Account/login	User login (sets auth cookie)	Public
POST	/api/Account/logout	User logout	Authenticated
GET	/api/Account/userinfo	Get current user info	Authenticated
Flights
Method	Endpoint	Description	Access
GET	/api/Flights	Get all flights	Public
GET	/api/Flights/{id}	Get flight by ID	Public
GET	/api/Flights/search	Search flights	Public
POST	/api/Flights	Create new flight	Admin
PUT	/api/Flights/{id}	Update flight	Admin
DELETE	/api/Flights/{id}	Delete flight	Admin
Hotels
Method	Endpoint	Description	Access
GET	/api/Hotels	Get all hotels	Public
GET	/api/Hotels/{id}	Get hotel by ID	Public
GET	/api/Hotels/search	Search hotels	Public
GET	/api/Hotels/rating/{minRating}	Hotels by rating	Public
POST	/api/Hotels	Create new hotel	Admin
PUT	/api/Hotels/{id}	Update hotel	Admin
DELETE	/api/Hotels/{id}	Delete hotel	Admin
Bookings
Method	Endpoint	Description	Access
GET	/api/Bookings/{id}	Get booking by ID	Owner/Admin
GET	/api/Bookings/my-bookings	Get user's bookings	Owner
GET	/api/Bookings/status/{status}	Get bookings by status	Admin
POST	/api/Bookings	Create new booking	Customer
POST	/api/Bookings/{id}/confirm	Confirm booking	Admin
POST	/api/Bookings/{id}/cancel	Cancel booking	Owner/Admin
Payments
Method	Endpoint	Description	Access
GET	/api/Payments/{id}	Get payment by ID	Owner/Admin
GET	/api/Payments/my-payments	Get user's payments	Owner
GET	/api/Payments/status/{status}	Get payments by status	Admin
POST	/api/Payments	Process payment	Customer
GET	/api/Payments/revenue	Get total revenue	Admin
User Management (Admin Only)
Method	Endpoint	Description
GET	/api/Users	Get all users
GET	/api/Users/{id}	Get user by ID
DELETE	/api/Users/{id}	Delete user
🧪 Testing the API
Using Swagger UI (Recommended)
Navigate to https://localhost:7064

Register a new user via /api/Account/register

Login via /api/Account/login (cookie will be set automatically)

Test protected endpoints - authentication is handled automatically

Sample Workflow
1. Register a Customer:

json
POST /api/Account/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "Password123!",
  "role": "Customer"
}
2. Login:

json
POST /api/Account/login
{
  "email": "john.doe@example.com",
  "password": "Password123!",
  "rememberMe": false
}
3. Book a Flight:

json
POST /api/Bookings
{
  "type": "Flight",
  "referenceId": 1,
  "numberOfGuests": 1
}
4. Process Payment:

json
POST /api/Payments
{
  "bookingId": 1,
  "paymentMethod": "Credit Card",
  "amount": 499.99
}
🔧 Development Guide
Project Structure
text
TravelBooking.API/
├── Controllers/          # API endpoints (Account, Flights, Hotels, Bookings, Payments)
├── Data/                # Seed data configuration
└── Program.cs           # Startup and configuration

TravelBooking.Application/
├── DTOs/               # Data Transfer Objects
├── Interfaces/         # Service contracts
├── Services/          # Business logic implementation
└── Mappings/          # AutoMapper profiles

TravelBooking.Domain/
├── Entities/          # Domain models (User, Flight, Hotel, Booking, Payment)
├── Enums/            # Enum definitions (BookingType, BookingStatus, etc.)
└── Interfaces/       # Repository contracts

TravelBooking.Infrastructure/
├── Data/             # ApplicationDbContext and configurations
└── Repositories/     # Entity Framework repository implementations
Adding New Features
Domain Layer (TravelBooking.Domain)

Add entities in Entities/

Add enums in Enums/

Define interfaces in Interfaces/

Infrastructure Layer (TravelBooking.Infrastructure)

Implement repositories in Repositories/

Update DbContext in Data/ApplicationDbContext.cs

Application Layer (TravelBooking.Application)

Create DTOs in DTOs/

Define service interfaces in Interfaces/

Implement services in Services/

Add mapping profiles in Mappings/

API Layer (TravelBooking.API)

Create controllers in Controllers/

Register services in Program.cs

Database Operations
bash
# Add new migration
dotnet ef migrations add AddNewFeature --project TravelBooking.Infrastructure --startup-project TravelBooking.API

# Update database
dotnet ef database update --project TravelBooking.Infrastructure --startup-project TravelBooking.API

# Remove last migration
dotnet ef migrations remove --project TravelBooking.Infrastructure --startup-project TravelBooking.API
🚀 Deployment
Production Configuration
Update appsettings.Production.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "YourProductionConnectionString"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
Environment Setup:

bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="YourConnectionString"
Publish:

bash
dotnet publish -c Release -o ./publish
🤝 Contributing
We welcome contributions! Please see our Contributing Guide for details.

Fork the repository

Create a feature branch (git checkout -b feature/amazing-feature)

Commit your changes (git commit -m 'Add amazing feature')

Push to the branch (git push origin feature/amazing-feature)

Open a Pull Request

📝 License
This project is licensed under the MIT License - see the LICENSE file for details.

🆘 Support
Documentation: Check the Wiki for detailed guides

Issues: Report bugs or request features via GitHub Issues

Questions: Use GitHub Discussions for help

🏆 Acknowledgments
ASP.NET Core team for the excellent framework and Identity system

Entity Framework Core for robust data access

Swagger for interactive API documentation

The .NET community for excellent libraries and support

Built with ❤️ using ASP.NET Core Identity
