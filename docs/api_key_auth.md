# API Key Authentication

## Overview

The API is secured using API key authentication. All requests must include a valid API key in the `X-API-Key` header.

## Configuration

### Local Development

1. Copy `appsettings.local.Example.json` to `appsettings.local.json`
2. Add your API keys to the ApiSecurity section:

```json
{
  "ApiSecurity": {
    "ApiKeys": ["your-key-1", "your-key-2"]
  }
}
```

### Azure App Service

Configure the API keys using Application Settings:

- Key: `ApiSecurity__ApiKeys__0`
- Value: `your-first-key`
- Key: `ApiSecurity__ApiKeys__1`
- Value: `your-second-key`

## Usage

### HTTP Request

```http
GET /api/endpoint
X-API-Key: your-api-key
```

### Swagger UI Authentication

1. Navigate to the Swagger UI page (/swagger)
2. Click the "Authorize" button (lock icon) at the top right
3. In the "ApiKey" section, enter your API key
4. Click "Authorize"
5. Close the authentication dialog
6. All subsequent requests will include your API key

Note: The Authorize button will appear green when you've successfully entered an API key.

### Testing

Use the test keys from appsettings.local.json for development.
