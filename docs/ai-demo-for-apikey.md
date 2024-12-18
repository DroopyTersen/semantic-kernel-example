Generate a user story:

I want to be able to prevent exposing the API to the internet. I want to require that there is a custom header attached to all requests what the caller includes an api key. the valid values for the api key should be a secret setup in app.settings.local and eventually an evironment variable. it should work fine whether running local or running in azure app service. All requests to my api should require this. it should still be testable in swagger, so setup whatever is needed so we can use swagger.

## User Story

**Title**: Implement API Key Authentication for API Requests

**User Story**: As a developer, I want to secure the API by requiring a custom header with an API key for all requests so that unauthorized access is prevented.

**Description**:

- The API should require a custom header containing an API key for all incoming requests.
- The valid API key should be stored securely in `appsettings.local.json` and/or as an environment variable.
- The solution should work seamlessly both locally and when deployed to Azure App Service.
- Swagger should be configured to allow testing with the custom header.

**Acceptance Criteria**:

- **User Interface Details**:

  - No direct UI changes are required as this is a backend feature.

- **User Actions and Interactions**:

  - All API requests must include a custom header named `X-API-KEY`.
  - Requests without this header or with an invalid API key should receive a `401 Unauthorized` response.

- **Data Handling**:

  - The API key should be configurable via `appsettings.local.json` and environment variables.
  - Ensure that the API key is not exposed in logs or error messages.

- **Business Logic**:

  - Middleware should be implemented to check for the presence and validity of the `X-API-KEY` header in all requests.
  - The middleware should be added to the request pipeline in `Program.cs`.

- **Performance Requirements**:

  - The addition of the middleware should not significantly impact the API's response time.

- **Error Handling and Notifications**:

  - Return a `401 Unauthorized` status code for requests missing the custom header or with an invalid API key.
  - Provide a clear error message indicating the reason for the unauthorized status.

- **Security Considerations**:

  - The API key should be stored securely and not hard-coded in the application.
  - Ensure that the API key is only accessible to authorized personnel.

- **Edge Cases and Exceptions**:

  - Handle cases where the API key is missing, malformed, or incorrect.
  - Ensure that the feature works both in local development and in Azure App Service environments.

- **Swagger Configuration**:
  - Configure Swagger to include the `X-API-KEY` header in requests for testing purposes.
  - Ensure that developers can easily test the API with the required header using Swagger.
