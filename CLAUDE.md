# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AICodingAgentv2 is a full-stack data breach management system with:
- **Backend**: .NET 9.0 ASP.NET Core Web API (`BreachApi/`)
- **Frontend**: Angular 20 application (`breach-viewer/`)

## Development Commands

### Backend (.NET API)
```bash
cd BreachApi
dotnet restore         # Restore dependencies
dotnet build          # Build project
dotnet run            # Run API (http://localhost:5012)
dotnet watch run      # Run with hot reload
```

### Frontend (Angular)
```bash
cd breach-viewer
npm install           # Install dependencies
npm start            # Start dev server (http://localhost:4200)
npm run build        # Production build
npm test             # Run unit tests
npm run watch        # Build with watch mode
```

## Architecture

### Backend Architecture
- **CQRS Pattern**: Uses MediatR for command/query separation
- **Feature Organization**: Code organized by domain features under `Features/`
- **Clean Architecture**: Controllers → MediatR → Query/Command Handlers
- **Global Exception Handling**: Centralized error handling middleware
- **PDF Generation**: Uses PuppeteerSharp with Razor views

Key Dependencies:
- MediatR (v12.2.0) - Mediator pattern
- Flurl.Http (v4.0.2) - HTTP client
- PuppeteerSharp (v13.0.2) - PDF generation
- Swashbuckle - Swagger/OpenAPI docs

### Frontend Architecture
- **Standalone Components**: Modern Angular approach without NgModules
- **Material Design**: Angular Material for UI components
- **Reactive Programming**: RxJS for data streams
- **Service Layer**: HTTP services for API communication

## Code Standards

### C# (.NET API)
- 4-space indentation
- PascalCase for public members, camelCase for private fields
- CQRS pattern with MediatR handlers
- XML documentation for controllers
- Global exception handling middleware

### TypeScript (Angular)
- 2-space indentation
- Standalone components with OnPush change detection
- Reactive forms and proper lifecycle management
- Material Design components
- JSDoc comments for services

## API Structure

### Endpoints
- `GET /api/breach` - Get breaches with optional date filtering
- `GET /api/breach/pdf` - Generate PDF reports
- Swagger UI available at `/swagger` in development

### CQRS Organization
- Queries in `Features/Breaches/Queries/`
- Each query has its own handler class
- MediatR handles request/response pipeline

## Key Files

### Backend Entry Points
- `Program.cs` - Application startup and DI configuration
- `Controllers/BreachController.cs` - API endpoints
- `Features/Breaches/Queries/` - Business logic handlers

### Frontend Entry Points
- `src/main.ts` - Application bootstrap
- `src/app/app.config.ts` - App configuration
- `src/app/services/breach.service.ts` - API communication
- `src/app/components/breach-list/` - Main UI component

## Development Notes

### API Configuration
- Runs on port 5012 in development
- CORS configured for Angular app (port 4200)
- Swagger enabled in development environment
- Uses appsettings.json for configuration

### Angular Configuration
- Configured for API base URL: `http://localhost:5012/api/breach`
- Material theme with custom styling
- Error handling with user-friendly messages
- Responsive design with data tables

### PDF Generation
- Uses headless browser (PuppeteerSharp)
- Razor views for PDF templates in `Views/`
- ViewModels in `ViewModels/` for data binding

## Testing

### Backend
- Use `dotnet test` (no test projects currently configured)

### Frontend
- Unit tests with Jasmine/Karma: `npm test`
- Angular testing utilities configured