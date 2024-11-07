# Segal AI Demo

## Local Settings Setup

Each project (API and CLI) needs its own local settings file for secrets.

1. For the API:

   ```bash
   cd SegalAI.API
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

2. For the CLI:
   ```bash
   cd SegalAI.CLI
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

The `appsettings.local.json` files are gitignored and won't be committed.
