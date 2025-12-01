# API Key Configuration Guide

## ⚠️ IMPORTANT: Never commit API keys to GitHub!

This project uses `appsettings.Local.json` for storing sensitive configuration like API keys. This file is **git-ignored** and will not be committed to the repository.

## Setup Instructions

### 1. Create Local Configuration File

Create a file named `appsettings.Local.json` in the `Incident` folder with your API key:

```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-YOUR_ACTUAL_API_KEY_HERE"
  }
}
```

### 2. Get Your OpenRouter API Key

1. Visit https://openrouter.ai/keys
2. Sign up or log in
3. Create a new API key
4. Copy it to your `appsettings.Local.json`

### 3. Configuration Priority

The application loads configuration in this order:
1. `appsettings.json` (base configuration, committed to git)
2. `appsettings.Development.json` (development settings, committed to git)
3. `appsettings.Local.json` (secrets, **NOT** committed to git) ✅
4. Environment variables

Settings in later files override earlier ones.

## Alternative: Environment Variables

You can also set the API key via environment variable:

### Windows (PowerShell):
```powershell
$env:OpenRouter__ApiKey = "sk-or-v1-YOUR_API_KEY"
```

### Linux/Mac:
```bash
export OpenRouter__ApiKey="sk-or-v1-YOUR_API_KEY"
```

## Production Deployment

For production, use one of these methods:

1. **Azure App Service**: Set configuration in Application Settings
2. **Docker**: Pass as environment variable
3. **Kubernetes**: Use secrets
4. **AWS**: Use Parameter Store or Secrets Manager

## Files Checked into Git

✅ `appsettings.json` - Contains empty/placeholder values
✅ `appsettings.Development.json` - Contains empty/placeholder values
✅ `.gitignore` - Excludes `appsettings.Local.json`

❌ `appsettings.Local.json` - **NEVER** committed (contains actual secrets)

## Verify Your Setup

Check what's being committed:
```bash
git status
```

The `appsettings.Local.json` should **NOT** appear in the list.
