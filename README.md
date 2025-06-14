# Order Service API

Orders API built with .NET 8, Entity Framework Core, and SQLite following clean architecture principles.

## Features

- **POST /api/orders** - Create new orders with validation
- **GET /api/orders/{id}** - Retrieve order by ID
- **GET /api/orders** - Retrieve all orders
- Entity Framework Core with SQLite
- Comprehensive unit and integration tests
- Async/await programming model
- Structured logging with Serilog
- Swagger/OpenAPI documentation
- Dependency injection

## Project Structure

```
OrderSvc.WebAPI/
├── Controllers/         # API Controllers
├── Data/                # Entity Framework DbContext
├── Exceptions/          # Custom Exception Classes
├── Repositories/        # Data Access Layer
├── Services/            # Business Logic Layer
├── Utilities/           # Utilities and extensions
└── Program.cs           # Application Entry Point

OrderSvc.DataContract/
├── Model/              # Domain Models
├── DTOs/                # Data Transfer Objects

OrderService.Tests/
├── Services/            # Unit tests for services
├── Repositories/        # Unit tests for repositories
└── Integration/         # Integration tests for API endpoints
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Any IDE that supports .NET (Visual Studio, VS Code)

### Installation and Setup

1. **Clone or navigate to the project directory:**
   ```bash
   cd OrderWebAPI
   ```

2. **Install SQLite (if not already installed):**
   SQLite is included with Entity Framework Core SQLite package, no separate installation needed.

3. **Restore NuGet packages:**
   ```bash
   cd OrderWebAPI
   dotnet restore
   ```

4. **Build the project:**
   ```bash
   dotnet build
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

The API will be available at:
- **HTTP**: `http://localhost:5009`
- **HTTPS**: `https://localhost:7159`
- **Swagger UI**: `http://localhost:5009/index.html` (root path)

### Database Setup

The application uses Entity Framework Core with SQLite and Code First approach:

- **Development Database**: `orders-dev.db`
- **Production Database**: `orders.db`
- Database is automatically created on first run
- Includes seed data for demonstration

### API Usage Examples

plese refer to OrderWebAPI.http file to test all supported scenarios.

## Testing

### Unit Tests
Run unit tests (services and repositories):
```bash
cd OrderSvc.Tests
dotnet test --filter "FullyQualifiedName!~Integration"
```

### All Tests
Run all tests:
```bash
dotnet test
```
## Database Schema

### Orders Table
- `OrderId` (GUID, Primary Key)
- `CustomerName` (string, required, max 100 chars)
- `CreatedAt` (DateTime, required)

### OrderItems Table
- `Id` (int, Primary Key, Auto-increment)
- `OrderId` (GUID, Foreign Key)
- `ProductId` (string, required, max 50 chars)
- `Quantity` (int, required, min 1)

## Logging

Logs are written to:
- **Console**: Development information
- **File**: `logs/logYYYYMMDD.txt` (rolling daily)

Log levels:
- **Information**: Normal operation events
- **Warning**: Potential issues (e.g., order not found)
- **Error**: Exceptions and errors
