# Bank Data Web Service (ASP.NET Core + SQLite)

This project implements a RESTful web service for managing banking data using **ASP.NET Core MVC** and **SQLite**. Developed for Tutorial 7 (Part A of Assignment 2) of the Distributed Computing unit, the service provides functionality for managing bank accounts, user profiles, and transactions, along with support for data seeding and error handling.

The project demonstrates practical knowledge of web service design, relational database integration, API development, and server-side validation.

## Overview

The system includes:
- Account management features
- Transaction tracking and balance updates
- User profile creation and editing
- Random data seeding for test/demo purposes
- SQLite-based persistent storage
- RESTful API with proper HTTP method design
- Exception handling with meaningful error responses

## Technologies Used

| Feature               | Technology                      |
|-----------------------|----------------------------------|
| Web Framework         | ASP.NET Core MVC                 |
| Database              | SQLite                           |
| Data Access           | Entity Framework Core / ADO.NET  |
| API Testing           | Postman                          |
| Language              | C#                               |
| Architecture          | RESTful Web Service              |

## Features

### 1. Account Management
- Create new bank accounts
- Retrieve account details using account number
- Update existing account data (e.g., balance, holder name)
- Delete accounts from the system

### 2. Transaction Management
- Perform deposit and withdrawal operations
- Automatically update account balances
- Store full transaction history for each account (audit log)

### 3. User Profile Management
- Create user profiles with name, email, phone, address, photo, and password
- Retrieve profiles using username or email
- Update user profile details
- Delete user profiles from the system

### 4. Database Integration
- Data is stored in a SQLite database file
- Database schema includes relational tables for Accounts, Transactions, and UserProfiles
- Data integrity and consistency maintained through foreign key constraints

### 5. Data Seeding
- Script automatically populates the database with:
  - Random bank accounts
  - Sample transaction history
  - Sample user profiles
- Useful for demonstration and testing scenarios

### 6. API Design
- RESTful endpoints designed with standard HTTP verbs:
  - `GET`, `POST`, `PUT`, `DELETE`
- Routes organized for clarity and logical grouping
- Full test coverage using Postman or equivalent API client

### 7. Exception Handling
- Input validation on all endpoints
- Catch and handle runtime errors gracefully
- Return meaningful status codes and error messages in JSON format

## How to Use

1. Clone the repository and open in Visual Studio
2. Ensure SQLite is installed or bundled with the project
3. Build and run the project to start the web service
4. Use Postman (or similar) to test API endpoints:
   - Account creation: `POST /api/accounts`
   - Deposit: `POST /api/transactions/deposit`
   - Profile creation: `POST /api/users`
   - etc.
5. Check the seeded data on startup (if enabled)
6. Review error messages and logging in case of failure

## API Endpoints Overview (Examples)

| Endpoint                         | Method | Description                        |
|----------------------------------|--------|------------------------------------|
| `/api/accounts`                 | POST   | Create a new bank account          |
| `/api/accounts/{id}`            | GET    | Get account details                |
| `/api/accounts/{id}`            | PUT    | Update account details             |
| `/api/accounts/{id}`            | DELETE | Delete an account                  |
| `/api/transactions/deposit`     | POST   | Deposit funds                      |
| `/api/transactions/withdraw`    | POST   | Withdraw funds                     |
| `/api/users`                    | POST   | Create a new user profile          |
| `/api/users/{email}`            | GET    | Get user profile by email          |
| `/api/users/{id}`               | PUT    | Update user profile                |
| `/api/users/{id}`               | DELETE | Delete user profile                |

## Getting Started

1. Open the project in Visual Studio
2. Configure the SQLite connection string in `appsettings.json`
3. Run migrations (if using Entity Framework)
4. Start the web service
5. Use any API client (e.g., Postman) to interact with the backend
6. Use seeded data or create your own entries

## Testing

- Test all endpoints using Postman or Swagger UI (if integrated)
- Validate error handling by sending invalid or incomplete requests
- Check seed data is correctly created during startup
- Manually test database content using SQLite browser or CLI
