# Data Breach Viewer

A modern Angular application for viewing and analyzing data breach information from the Breach API.

## Features

- **Data Breach Display**: View comprehensive breach data in a modern, responsive table
- **Date Filtering**: Filter breaches by date range using intuitive date pickers
- **PDF Export**: Download breach reports as PDF files
- **Real-time Data**: Live data from the Breach API with proper error handling
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Material Design**: Modern UI using Angular Material components

## Prerequisites

- Node.js (v18 or higher)
- npm (v9 or higher)
- Angular CLI (v20 or higher)
- .NET 8.0 SDK (for the API)

## Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Install Angular Material** (if not already installed):
   ```bash
   ng add @angular/material
   ```

## Development

### Starting the Development Server

1. **Start the Angular development server**:
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:4200`

2. **Start the Breach API** (in a separate terminal):
   ```bash
   cd ../BreachApi
   dotnet run
   ```
   The API will be available at `http://localhost:5000`

### Building for Production

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

## Usage

### Viewing Breaches

1. Open the application in your browser
2. The breach list will load automatically
3. Use the date filters to narrow down results:
   - **From Date**: Set the start date for filtering
   - **To Date**: Set the end date for filtering
4. Click "Apply Filters" to update the results
5. Click "Clear" to reset all filters

### Downloading PDF Reports

1. Set your desired date filters (optional)
2. Click the "Download PDF" button
3. The PDF will be automatically downloaded to your default download folder

### Understanding the Data

The breach table displays the following information:

- **Title**: The name of the breach
- **Domain**: The affected website/service
- **Breach Date**: When the breach occurred
- **Accounts Affected**: Number of compromised accounts
- **Data Types**: Types of data that were compromised
- **Verified**: Whether the breach has been verified
- **Flags**: Special indicators (Sensitive, Retired, Spam List)

## API Integration

The application communicates with the Breach API using the following endpoints:

- `GET /api/breach` - Retrieve breaches with optional date filtering
- `GET /api/breach/pdf` - Download PDF report with optional date filtering

### API Configuration

The API URL is configured in `src/app/services/breach.service.ts`:
```typescript
private readonly apiUrl = 'http://localhost:5000/api/breach';
```

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   └── breach-list/
│   │       ├── breach-list.component.ts
│   │       ├── breach-list.component.html
│   │       └── breach-list.component.css
│   ├── models/
│   │   └── breach.model.ts
│   ├── services/
│   │   └── breach.service.ts
│   ├── app.config.ts
│   ├── app.routes.ts
│   ├── app.ts
│   └── app.html
├── styles.css
└── main.ts
```

## Technologies Used

- **Angular 20**: Modern web framework
- **Angular Material**: UI component library
- **RxJS**: Reactive programming library
- **TypeScript**: Type-safe JavaScript
- **Angular Forms**: Form handling and validation

## Development Guidelines

### Code Style

- Follow Angular style guide
- Use TypeScript strict mode
- Implement proper error handling
- Use reactive forms for complex forms
- Follow component architecture best practices

### Component Architecture

- Use standalone components
- Implement OnPush change detection where possible
- Use proper lifecycle hooks
- Implement proper cleanup in ngOnDestroy

### Error Handling

- Use proper HTTP error handling
- Display user-friendly error messages
- Log errors for debugging
- Implement retry mechanisms where appropriate

## Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure the API is running and CORS is properly configured
2. **API Connection**: Verify the API URL in the service configuration
3. **Build Errors**: Check Node.js and npm versions
4. **Material Design Issues**: Ensure Angular Material is properly installed

### Debug Mode

Enable debug logging by opening browser developer tools and checking the console for detailed error messages.

## Contributing

1. Follow the established code style
2. Add proper error handling
3. Include unit tests for new features
4. Update documentation as needed

## License

This project is part of the AICodingAgentv2 repository.
