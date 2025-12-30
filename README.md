# Smart City Management System - Tourist Village (القرية السياحية)

A comprehensive Smart City Management System designed for modern tourist villages, built with .NET 8, Clean Architecture, and MongoDB.

## 🚀 Overview

This system provides a digital platform for managing city/village services, citizen requests, complaints, and IoT-based utility monitoring (Electricity & Water). It is structured using **Clean Architecture** principles to ensure scalability, maintainability, and testability.

## 🛠 Tech Stack

- **Backend**: .NET 8 Web API
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Database**: MongoDB
- **Authentication**: ASP.NET Core Identity with JWT
- **Real-time**: SignalR (for notifications and IoT alerts)
- **Logging**: Serilog

## 📂 Project Structure

- **SmartCity.Domain**: Core entities, value objects, and repository interfaces.
- **SmartCity.Application**: Business logic, CQRS (MediatR), DTOs, and mapping.
- **SmartCity.Infrastructure**: Persistence (MongoDB), Identity services, and external integrations.
- **SmartCity.API**: RESTful endpoints and middleware.

## ✨ Key Features

- **Dashboard**: Real-time statistics for managers and citizens.
- **Service Management**: Digital catalog of services (e.g., Beach Access, Villa Maintenance).
- **Service Requests**: Citizens can request services and track their status.
- **Complaint System**: Report issues with categories and status tracking.
- **IoT Monitoring**: Utility meter (Water/Electricity) monitoring with usage alerts.
- **Manager Portal**: Tools for managing requests and responding to complaints.

## 🚦 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (Local or Atlas)

### Configuration

Update the `appsettings.json` in `SmartCity.API` with your MongoDB connection string:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "SmartCityDb"
}
```

### Running the Project

1. Navigate to the project root.
2. Run the application:
   ```bash
   dotnet run --project src/SmartCity.API
   ```
3. Open `http://localhost:5000/swagger` to explore the API.

## 🧪 Seed Data

On initial run, the system automatically seeds:
- **Admin Account**: `admin@smartcity.com` / `Admin123!`
- **Citizen Account**: `ahmdasanhabwanar@gmail.com` / `Ahmed`
- **Default Services**: Beach Access, Villa Maintenance, Cleaning, etc.
- **Sample Data**: Initial complaints, service requests, and utility meter readings.

## 📝 License

This project is licensed under the MIT License.
