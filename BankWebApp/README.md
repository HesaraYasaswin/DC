# Bank Data Web Application (ASP.NET Core MVC)

This project is a web application built using **ASP.NET Core MVC** that serves as the presentation layer for the Bank Data Web Service developed in Tutorial 7 (Part A). The application consumes the RESTful web services exposed by the backend API and provides users with an interactive, user-friendly interface for managing banking operations.

The architecture follows a two-tier design:
- **Business Layer**: RESTful API endpoints accessed via `/api/`
- **Presentation Layer**: Web pages served at `/` that consume the API asynchronously

## Project Overview

The web application features two main dashboards:

### User Dashboard
- Displays user profile information including name, contact info, and profile picture
- Shows account summaries with balances and recent transactions
- Allows money transfers between accounts and to other users, with input validation
- Provides secure login and logout functionality to protect user data

### Admin Dashboard
- Displays admin profile information
- Enables user management functions such as creating, editing, deactivating user accounts, and password resets
- Provides access to view, filter, and sort transaction records for all users
- Implements security features including audit trails and activity logs

## Technologies Used

| Component           | Technology                          |
|---------------------|------------------------------------|
| Web Framework       | ASP.NET Core MVC                   |
| API Consumption     | RESTSharp / HttpClient             |
| Frontend            | HTML, CSS, JavaScript (AJAX)      |
| Language            | C#                                |
| Architecture        | Two-tier (Presentation + Business) |

## Key Features

- **Separation of Concerns**: Frontend UI communicates with backend services via REST APIs
- **Asynchronous Data Fetching**: Uses AJAX to fetch and update data without full page reloads
- **Secure User Authentication**: Login and logout mechanisms prevent unauthorized access
- **Dynamic Content Updates**: Transaction histories, account balances, and user details update dynamically
- **Input Validation**: Client and server side validation ensure accuracy in money transfers and profile updates
- **Role-Based Dashboards**: Separate views and permissions for regular users and administrators

## How It Works

1. The **Home Controller** serves the initial HTML page at `/`.
2. JavaScript loaded on the page asynchronously calls the backend API endpoints (e.g., `/api/accounts`, `/api/users`) to retrieve and manipulate data.
3. User interactions such as viewing transactions, transferring money, or managing profiles trigger API requests.
4. Responses are received in JSON format and the UI is updated dynamically.
5. Admin users have additional controls for managing user accounts and monitoring transactions.

## Getting Started

1. Ensure the Bank Data Web Service (Tutorial 7 API) is running and accessible.
2. Open the web application project in Visual Studio.
3. Configure the API base URL in the application settings (to point to the running API).
4. Run the web application.
5. Access the application in a browser at `http://localhost:{port}/`.
6. Use the user dashboard or admin dashboard depending on your login credentials.

## Testing

- Test user login and profile management.
- Verify account summaries and transaction histories load correctly.
- Perform money transfers with valid and invalid inputs to test validation.
- Test admin features such as user management and transaction monitoring.
- Validate that data updates dynamically without full page reloads.

