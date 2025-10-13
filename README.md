# StrivoLabsSubscriptionService

This project was developed as part of the Strivo Labs Backend Developer Task. It provides a subscription management service built with C# and .NET 9, implementing clean architecture principles and RESTful API design.

## Overview
A simple C# .NET API with MySQL for managing subscriptions.

## Features
- **Login** - Authenticate with service_id and password to get a token
- **Subscribe** - Subscribe a phone number to a service
- **Unsubscribe** - Remove an active subscription
- **Check Status** - Get the subscription status for a phone number

## Requirements
- .NET 9.0
- MySQL Server
- Docker (optional, only if using Docker Compose)

## Installation & Running

1. **Clone the repository**
   ```bash
   git clone https://github.com/KelvinEsiri/StrivoLabsSubscriptionService.git
   cd StrivoLabsSubscriptionService
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Set up MySQL database**
   - Install and run MySQL Server locally
   - Update the connection string in `appsettings.json` with your MySQL credentials

5. **Run the application** (choose one option below)

   **Option 1: Run locally with dotnet (Recommended for development)**

   - Ensure MySQL is running on your local machine
   - Run the application:
     ```bash
     dotnet run
     ```
   - The API will be available at `http://localhost:5033`

   **Option 2: Run API in Docker (MySQL on host machine)**

   *Prerequisites: Docker Desktop must be installed*

   - Ensure MySQL is running on your local machine
   - Build and run the API container:
     ```bash
     docker compose up -d
     ```
   - The API will be available at `http://localhost:5033`

   *Note: The Docker setup runs the API in a container but connects to MySQL on your host machine using `host.docker.internal`.*

6. **Access the application**
   - Use Swagger UI for testing: `http://localhost:5033/swagger`
   - The database will be automatically created and seeded on first run

## Configuration

### Database Connection
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=subscription_service;User=root;Password=your_password;"
  },
  "TokenValidityHours": 24
}
```

### Token Validity
Token expiration time can be configured in `appsettings.json` using the `TokenValidityHours` setting (default is 24 hours).

## API Endpoints

### 1. Login
**POST** `/api/auth/login`

**Request Body:**
```json
{
  "service_id": "test_service",
  "password": "test_password"
}
```

**Success Response (200 OK):**
```json
{
  "token": "guid-token-string"
}
```

**Error Responses:**
- `400 Bad Request` - Missing required fields
- `401 Unauthorized` - Invalid credentials

### 2. Subscribe
**POST** `/api/subscription/subscribe`

**Request Body:**
```json
{
  "service_id": "test_service",
  "token_id": "your-token-from-login",
  "phone_number": "+1234567890"
}
```

**Success Response (200 OK):**
```json
{
  "subscription_id": "guid-subscription-id"
}
```

**Error Responses:**
- `400 Bad Request` - Missing required fields or invalid service_id
- `401 Unauthorized` - Invalid or expired token
- `409 Conflict` - Already subscribed

### 3. Unsubscribe
**POST** `/api/subscription/unsubscribe`

**Request Body:**
```json
{
  "service_id": "test_service",
  "token_id": "your-token-from-login",
  "phone_number": "+1234567890"
}
```

**Success Response (200 OK):**
```json
{
  "message": "Unsubscribed successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Missing required fields or invalid service_id
- `401 Unauthorized` - Invalid or expired token
- `404 Not Found` - No active subscription found

### 4. Check Status
**POST** `/api/subscription/status`

**Request Body:**
```json
{
  "service_id": "test_service",
  "token_id": "your-token-from-login",
  "phone_number": "+1234567890"
}
```

**Success Response (200 OK):**
```json
{
  "status": "subscribed",
  "subscription_date": "2025-10-09T23:23:54.579Z"
}
```
or
```json
{
  "status": "not_subscribed",
  "subscription_date": null
}
```

**Error Responses:**
- `400 Bad Request` - Missing required fields or invalid service_id
- `401 Unauthorized` - Invalid or expired token

## Default Test Service
The application seeds a test service on first run:
- **service_id:** `test_service`
- **password:** `test_password`

## Testing the API

You can use the provided `SubscriptionService.http` file with REST Client extensions in VS Code, or use curl:

### Example Flow

1. Login to get a token:
```bash
curl -X POST http://localhost:5033/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"service_id":"test_service","password":"test_password"}'
```

2. Subscribe with the token:
```bash
curl -X POST http://localhost:5033/api/subscription/subscribe \
  -H "Content-Type: application/json" \
  -d '{"service_id":"test_service","token_id":"your-token-here","phone_number":"+1234567890"}'
```

3. Check status:
```bash
curl -X POST http://localhost:5033/api/subscription/status \
  -H "Content-Type: application/json" \
  -d '{"service_id":"test_service","token_id":"your-token-here","phone_number":"+1234567890"}'
```

4. Unsubscribe:
```bash
curl -X POST http://localhost:5033/api/subscription/unsubscribe \
  -H "Content-Type: application/json" \
  -d '{"service_id":"test_service","token_id":"your-token-here","phone_number":"+1234567890"}'
```

## Database Schema

### Services Table
- Id (Primary Key)
- ServiceId (Unique, String)
- Password (String)

### Tokens Table
- Id (Primary Key)
- TokenId (Unique, String)
- ServiceId (Foreign Key)
- CreatedAt (DateTime)
- ExpiresAt (DateTime)

### Subscriptions Table
- Id (Primary Key)
- SubscriptionId (Unique, String)
- ServiceId (Foreign Key)
- PhoneNumber (String)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- IsActive (Boolean)

## Additional Implementation Notes

### Showcase Methods
The `SubscriptionManagementService` includes several additional methods that are **not exposed through API endpoints** but are implemented to demonstrate different coding patterns and architectural approaches:

- **`DeleteSubscriptionAsync`** - Permanently removes a subscription from the database (vs. soft delete in `UnsubscribeAsync`)
- **`GetAllSubscriptionsAsync`** - Retrieves all subscriptions from the database
- **`GetAllSubscriptionsResponseAsync`** - Gets all subscriptions with a standardized service result wrapper
- **`GetFilteredSubscriptionAsync`** - Filters subscriptions by date range, showcasing centralized logic reuse

These methods demonstrate:
- **Code reusability** - `GetAllSubscriptionsAsync` is reused by `GetFilteredSubscriptionAsync`
- **Consistent error handling** - Using the `ServiceResult<T>` pattern
- **Different data access patterns** - Both simple and complex queries

While not currently used in the API, these methods can be easily integrated if needed by adding corresponding controller endpoints.

## Contact

**Kelvin Esiri**

- 📧 Email: [kelvinesiri@gmail.com](mailto:kelvinesiri@gmail.com)
- 📱 Phone: 08161643301
- 💼 LinkedIn: [linkedin.com/in/kelvinesiri](https://www.linkedin.com/in/kelvinesiri/)
