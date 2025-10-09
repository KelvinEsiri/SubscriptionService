# StrivoLabsSubscriptionService
API for subscription service

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

## Running the Application

1. Ensure MySQL is running
2. Update the connection string in `appsettings.json`
3. Run the application:
```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

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
- IsActive (Boolean)
