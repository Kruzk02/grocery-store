# Grocery Store API

A backend REST API built with ASP.NET Core for managing users, products, orders, inventory, and customers.
The application uses JWT-based authentication, generates PDF invoices, and is fully containerized using Docker.

This project is intended as a portfolio and learning project demonstrating clean API design and backend fundamentals.

## Features

- User registration and authentication using JWT
- CRUD operations for products, orders, order items, inventory, and customers
- PDF invoice generation using QuestPDF
- RESTful API design
- Docker Compose–based setup

## Tech Stack

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT authentication
- QuestPDF
- Docker & Docker Compose

## Installation

1. Clone the repository: `git clone https://github.com/Kruzk02/grocery-store.git `.
2. Navigate to the project directory: `cd grocery-store `.
3. Start the services using Docker Compose: `docker compose up -d`
4. Access the API at <http://localhost:8080>.
   
## Usage

Once the application is running, you can use tools like Postman or curl to interact with the API endpoints. Here's an example:
1. **Create a new user**:
  Send a POST request to `http://localhost:8080/user/register` with the following JSON payload:
   ```json
    {
        "Email": "email1@gmail.com",
        "Username": "username12",
        "password": "Password123!"
    }
   ```
2. **Authenticate the user**:
 Send a POST request to `http://localhost:8080/user/login` with the following JSON payload to receive a JWT token:
   ```json
    {
        "UserNameOrEmail": "admin@example.com",
        "password": "Admin@123"
    }
   ```
3. Use the generated JWT token to access protected endpoints.
