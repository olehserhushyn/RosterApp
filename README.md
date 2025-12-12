# RosterApp

## RosterApp is a simple roster management application built with:

-   Backend: ASP.NET Core (.NET 10) Web API
-   Frontend: React + TypeScript + Vite
-   Database: SQLite (with automatic seeding on startup)
-   Containerization: Docker & Docker Compose

The project is structured as a clean architecture with separate layers
for API, Application, Domain, and Infrastructure.

Prerequisites

Local development: 
- .NET 10 SDK
- Node.js 18+ (or 20+ recommended)
- npm

Docker run: 
- Docker Desktop

## Running with Docker

From the repository root:

    docker compose up --build

This starts: - API on http://localhost:5000 - Client on
http://localhost:3000

Frontend → API communication in Docker uses:
VITE_API_BASE_URL=http://localhost:5000/api

## Running Locally

1.  Run the API:

    cd API dotnet run –project RosterApp.API/RosterApp.API.csproj

2.  Run the Client:

    cd Client npm install npm run dev

Environment Variables

Client/.env.development:

    VITE_API_BASE_URL=https://localhost:44309/api

Client/.env.production:

    VITE_API_BASE_URL=http://localhost:5000/api

## Database & Seeding

-   SQLite is used as the database provider.
-   Database file is created automatically.
-   Seeding runs automatically on application startup.

Useful Commands

Backend: dotnet restore dotnet build dotnet run

Frontend: npm install npm run dev npm run build npm run preview


# Preview

<img width="1920" height="883" alt="image" src="https://github.com/user-attachments/assets/29e46677-2bed-4be1-8d01-1b5cd603b5a5" />

<img width="1920" height="968" alt="image" src="https://github.com/user-attachments/assets/89261290-c71f-4406-9883-75609a0d83b7" />

<img width="1920" height="2970" alt="image" src="https://github.com/user-attachments/assets/8e8e072a-3ac1-48f5-94c1-bb482db216ae" />



