# HotelListing.API - Authentication Quick Reference

## 🔐 JWT Authentication Status: ✅ ACTIVE

This API requires JWT Bearer tokens for write operations (POST/PUT/DELETE).

---

## For Developers: Local Setup (First Time Only)

### 1. Initialize User Secrets
```powershell
cd HotelListing.API
dotnet user-secrets init
```
This creates a machine-local secrets store (NOT in the repository).

### 2. Configure Your Local JWT Secret
```powershell
# Option A: Auto-generate a strong key
$bytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::new()
$rng.GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "JwtSettings:SecretKey" $key

# Option B: Use an existing team key (ask your team lead)
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_TEAM_KEY_HERE"
```

### 3. Verify Configuration
```powershell
dotnet user-secrets list
# Should show:
# JwtSettings:SecretKey = [hidden]
# JwtSettings:Issuer = https://localhost:7177
# JwtSettings:Audience = HotelListingApi
# JwtSettings:ExpiresInMinutes = 60
```

### 4. Run the API
```powershell
dotnet run
# Should start without JWT configuration errors
```

---

## 📡 API Endpoints

### Public (No Token Required)
```
GET  /api/countries       → List all countries
GET  /api/countries/{id}  → Get country by ID
GET  /api/hotels          → List all hotels
GET  /api/hotels/{id}     → Get hotel by ID
```

### Protected (Admin Token Required)
```
POST   /api/countries       → Create country (Admin only)
PUT    /api/countries/{id}  → Update country (Admin only)
DELETE /api/countries/{id}  → Delete country (Admin only)

POST   /api/hotels          → Create hotel (Admin only)
PUT    /api/hotels/{id}     → Update hotel (Admin only)
DELETE /api/hotels/{id}     → Delete hotel (Admin only)
```

---

## 🎟️ How to Get a JWT Token (Phase 2)

**Coming soon:** Auth controller will provide `/api/auth/login` endpoint.

Currently, there is **no token generation mechanism**. To test protected endpoints, you would need:
1. Identity/DbContext implementation (Phase 2)
2. Auth controller with user registration/login
3. A way to assign Admin roles to test users

---

## 🧪 Testing with cURL

### Test: Public Read (No Auth)
```bash
curl -X GET https://localhost:7177/api/countries
```

### Test: Protected Write (Mock Token)
```bash
curl -X POST https://localhost:7177/api/countries \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..." \
  -d '{"id":10,"name":"Test","code":"TS"}'
```

### Test: Protected Write (No Token)
```bash
curl -X POST https://localhost:7177/api/countries \
  -H "Content-Type: application/json" \
  -d '{"id":10,"name":"Test","code":"TS"}'
# Response: 401 Unauthorized
```

---

## 🔧 Troubleshooting

### "JWT SecretKey is not configured"
**Cause:** User secrets not initialized  
**Fix:**
```powershell
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_KEY_HERE"
```

### "401 Unauthorized" on public endpoints
**Cause:** [Authorize] attribute left on GET endpoints by mistake  
**Fix:** Verify GET methods have `[AllowAnonymous]` or no auth attribute

### "403 Forbidden" on write endpoints
**Cause:** User doesn't have "Admin" role in token  
**Fix:** Token claims must include `role: "Admin"` (Phase 2 feature)

### Token Validation Fails
**Common Causes:**
- Issuer in token doesn't match config (must be `https://localhost:7177`)
- Audience in token doesn't match config (must be `HotelListingApi`)
- Token expired (lifetime is 60 minutes)
- Token signature invalid (wrong secret key)

---

## 📋 JWT Token Structure (Reference)

A valid JWT token would look like:
```
eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiJzb21lLWlkIiwic3ViIjoic29tZS11c2VyIiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE3NyIsImF1ZCI6IkhvdGVsTGlzdGluZ0FwaSIsImlhdCI6IjE3MzY0NzY4MDAiLCJleHAiOiIxNzM2NDgwNDAwIiwicm9sZSI6IkFkbWluIn0.abc123...
```

**Decoded Header:**
```json
{
  "typ": "JWT",
  "alg": "HS256"
}
```

**Decoded Payload:**
```json
{
  "iss": "https://localhost:7177",
  "aud": "HotelListingApi",
  "exp": 1736480400,
  "iat": 1736476800,
  "role": "Admin"
}
```

---

## 📂 Related Files

- `appsettings.json` - JWT configuration (placeholder key only)
- `Program.cs` - Authentication registration + middleware
- `Controllers/HotelsController.cs` - Protected endpoints
- `Controllers/CountriesController.cs` - Protected endpoints
- `SECRETS_SETUP.md` - Detailed secrets configuration guide
- `PHASE1_COMPLETION_REPORT.md` - Full implementation report
- `.gitignore` - Excludes secrets.json, production configs

---

## 🚀 Next Phase

Phase 2 will add:
- User registration & login endpoints
- JWT token generation
- Role-based access control
- Identity/DbContext integration

Until then, only public read access is available without manual token generation.

---

**Last Updated:** January 2025  
**Status:** Phase 1 Complete  
**Questions?** See `SECRETS_SETUP.md` for detailed setup instructions.
