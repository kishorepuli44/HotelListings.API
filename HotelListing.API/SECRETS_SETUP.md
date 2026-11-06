# User Secrets Setup for JWT Authentication

## Overview
This document explains how to securely configure JWT authentication for local development using ASP.NET Core User Secrets Manager. **NEVER commit real secrets to the repository.**

## Prerequisites
- .NET 10 SDK installed
- PowerShell or bash terminal
- Project in: `HotelListing.API/`

## Step 1: Initialize User Secrets
Run this command from the `HotelListing.API/` project directory to set up the user secrets ID:

```powershell
dotnet user-secrets init
```

This creates a `UserSecretsId` in the `.csproj` file and stores secrets in a local, machine-specific location (not the repository).

## Step 2: Generate a Secure JWT Secret Key
Generate a strong random key (minimum 32 characters for HS256):

```powershell
# Option 1: Generate using PowerShell
[Convert]::ToBase64String([System.Random]::new().GetBytes(64))

# Option 2: Use an online tool (keep it local!)
# Generate at least 32 random characters, NO spaces
```

## Step 3: Configure JWT Settings as User Secrets
Store each JWT setting in local secrets:

```powershell
# From HotelListing.API/ directory:
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_GENERATED_SECRET_KEY_HERE"
dotnet user-secrets set "JwtSettings:Issuer" "https://localhost:7177"
dotnet user-secrets set "JwtSettings:Audience" "HotelListingApi"
dotnet user-secrets set "JwtSettings:ExpiresInMinutes" "60"
```

Replace `YOUR_GENERATED_SECRET_KEY_HERE` with your strong random key from Step 2.

## Step 4: List Configured Secrets
Verify your secrets are stored correctly:

```powershell
dotnet user-secrets list
```

Expected output:
```
JwtSettings:SecretKey = [REDACTED]
JwtSettings:Issuer = https://localhost:7177
JwtSettings:Audience = HotelListingApi
JwtSettings:ExpiresInMinutes = 60
```

## Step 5: Test the Configuration
Start the API and verify it loads without throwing "JWT SecretKey is not configured" exception:

```powershell
dotnet run
```

If the app starts without errors, your JWT configuration is ready!

## Important Notes

### 🔒 Security Checklist
- ✅ The placeholder `YOUR_SECRET_KEY_MIN_32_CHARS_PLACEHOLDER_FOR_DEVELOPMENT_ONLY` in `appsettings.json` is **intentionally weak**.
- ✅ Real secrets are stored in user-secrets (outside the repo).
- ✅ `.gitignore` is configured to exclude `secrets.json` and environment files.
- ✅ Environment-specific `appsettings.Production.json` should NOT be committed.
- ✅ Each developer uses their own local secrets — never share keys.

### 🚀 Production Deployment
For production, use **Azure Key Vault** or your cloud provider's secrets manager:
- Inject secrets via environment variables
- Use managed identities for authentication
- Rotate keys regularly
- Audit access logs

### 📝 Local Secrets Locations
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<USER_SECRETS_ID>\secrets.json`
- **Linux/macOS**: `~/.microsoft/usersecrets/<USER_SECRETS_ID>/secrets.json`

### 🐛 Troubleshooting
If the JWT settings are not loading:
```powershell
# Clear and reinitialize
dotnet user-secrets clear
dotnet user-secrets init
dotnet user-secrets list
```

---

**✅ Phase 1 Complete**: JWT authentication is configured and ready to protect your API endpoints!
