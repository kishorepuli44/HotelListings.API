# Authentication & Authorization - Quick Implementation Checklist

## 🔴 CRITICAL: Must Fix Before ANY Network Exposure

### 1. Add UseAuthentication() Middleware
**File:** `HotelListing.API/Program.cs`
- [x] Add `using Microsoft.AspNetCore.Authentication.JwtBearer;` ✅ DONE
- [x] Call `app.UseAuthentication();` BEFORE `app.UseAuthorization()` ✅ DONE
- [x] **Order matters:** Auth → AuthZ → Controllers ✅ VERIFIED

**Code:**
```csharp
app.UseHttpsRedirection();
app.UseAuthentication();      // ← Add this line
app.UseAuthorization();
app.MapControllers();
```

---

### 2. Install Authentication Packages
**Command:**
```bash
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

✅ **STATUS:** COMPLETE
- [x] System.IdentityModel.Tokens.Jwt v8.19.2 installed
- [x] Microsoft.AspNetCore.Authentication.JwtBearer v10.0.12 installed
- [x] Package versions aligned and restore successful

---

### 3. Register JWT Authentication
**File:** `Program.cs`
- [x] Create `AuthenticationExtensions` static class ✅ DONE
- [x] Configure JWT Bearer with strict validation ✅ DONE
- [x] Register in builder.Services ✅ DONE

**Minimal Code:**
```csharp
static class AuthenticationExtensions
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
	{
		var key = Encoding.ASCII.GetBytes(config["JwtSettings:SecretKey"]);

		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = config["JwtSettings:Issuer"],
					ValidateAudience = true,
					ValidAudience = config["JwtSettings:Audience"],
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};
			});

		return services;
	}
}
```

Add to Program.cs:
```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
```

---

### 4. Add JWT Configuration to appsettings.json
**File:** `HotelListing.API/appsettings.json`
- [x] Add JwtSettings section (with placeholder key) ✅ DONE

**Note:** Placeholder `YOUR_SECRET_KEY_MIN_32_CHARS_PLACEHOLDER_FOR_DEVELOPMENT_ONLY` is intentionally non-secret.
Real JWT secret is stored in user-secrets (not in repo).

```json
{
  "JwtSettings": {
	"SecretKey": "YOUR_SECRET_KEY_HERE_MINIMUM_32_CHARACTERS",
	"Issuer": "https://localhost:7177",
	"Audience": "HotelListingApi",
	"ExpiresInMinutes": 60
  },
  "Logging": { ... },
  "AllowedHosts": "*"
}
```

---

### 5. Secure JWT Key with User Secrets
**Commands:**
```bash
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_GENERATED_KEY"
dotnet user-secrets set "JwtSettings:Issuer" "https://localhost:7177"
dotnet user-secrets set "JwtSettings:Audience" "HotelListingApi"
dotnet user-secrets set "JwtSettings:ExpiresInMinutes" "60"
```

✅ **STATUS:** COMPLETE
- [x] User Secrets ID: `24c96e41-1384-4c29-95ea-b7db810dfe37` ✅
- [x] JWT Secret Key: 64-byte Base64 generated & stored securely ✅
- [x] All JWT settings configured in user-secrets (NOT committed) ✅
- [x] Verified with `dotnet user-secrets list` ✅

---

### 6. Add [Authorize] to Protected Endpoints
**File:** `HotelListing.API/Controllers/HotelsController.cs`
- [x] Add `using Microsoft.AspNetCore.Authorization;` ✅ DONE
- [x] Mark write endpoints (POST, PUT, DELETE) with `[Authorize(Roles = "Admin")]` ✅ DONE
- [x] Mark read endpoints with `[AllowAnonymous]` ✅ DONE

**File:** `HotelListing.API/Controllers/CountriesController.cs`
- [x] Add `using Microsoft.AspNetCore.Authorization;` ✅ DONE
- [x] Mark write endpoints (POST, PUT, DELETE) with `[Authorize(Roles = "Admin")]` ✅ DONE
- [x] Mark read endpoints with `[AllowAnonymous]` ✅ DONE

---

### 7. Update .gitignore
**File:** `.gitignore`
- [x] Add secrets patterns ✅ DONE

**Added patterns:**
```
**/secrets.json
**/appsettings.*.secrets.json
**/appsettings.Production.json
**/appsettings.Staging.json
.env
.env.local
.env.*.local
*.pfx
*.p12
*.pem
*.key
```

---

## Priority Completion Order

1. **FIRST (5 minutes):** Add `UseAuthentication()` to middleware
2. **SECOND (30 minutes):** Install packages & register JWT authentication
3. **THIRD (15 minutes):** Add JWT config to appsettings.json
4. **FOURTH (15 minutes):** Set JWT key via user-secrets
5. **FIFTH (20 minutes):** Add `[Authorize]` decorators to all controllers
6. **SIXTH (5 minutes):** Update `.gitignore`
7. **SEVENTH (10 minutes):** Test build & verify no auth errors

**Total Time:** ~2 hours for minimal implementation

---

## Testing After Each Step

### Test 1: Verify Middleware Order
```bash
dotnet run
# Should start without middleware order errors
```

### Test 2: Verify Auth Disabled without Token
```bash
curl -X POST https://localhost:7177/api/hotels \
  -H "Content-Type: application/json" \
  -d '{"id":999,"name":"Test"}'
# Should return 401 Unauthorized
```

### Test 3: Verify Swagger Shows Auth
```
https://localhost:7177/openapi/v1.json
# Should NOT show security requirements yet (fix in Phase 2)
```

---

## Common Errors & Fixes

### Error: "UseAuthentication not found"
**Fix:** Add `using Microsoft.AspNetCore.Authentication.JwtBearer;`

### Error: "JwtBearerDefaults not found"
**Fix:** Install `System.IdentityModel.Tokens.Jwt` NuGet package

### Error: "Authorize attribute not found"
**Fix:** Add `using Microsoft.AspNetCore.Authorization;`

### Error: "Exception: No application error middleware has been configured"
**Fix:** Add `UseAuthentication()` BEFORE `UseAuthorization()`

---

## Security Validation Checklist

✅ **ALL ITEMS COMPLETE:**
- [x] `UseAuthentication()` called before `UseAuthorization()` ✅
- [x] JWT validation includes issuer check ✅
- [x] JWT validation includes audience check ✅
- [x] JWT validation includes lifetime check ✅
- [x] Secret key NOT in source control (using user-secrets) ✅
- [x] Secret key NOT in appsettings.json (placeholder only) ✅
- [x] `[Authorize(Roles = "Admin")]` on POST endpoints ✅
- [x] `[Authorize(Roles = "Admin")]` on PUT endpoints ✅
- [x] `[Authorize(Roles = "Admin")]` on DELETE endpoints ✅
- [x] `[AllowAnonymous]` explicit on GET endpoints ✅
- [x] Build succeeds with no auth-related warnings ✅ (Build succeeded in 4.4s)
- [x] `.gitignore` includes `secrets.json` and production configs ✅

---

## Phase 2: Identity & Roles (Next)

Once Phase 1 is complete and tested:
- [ ] Create `ApplicationDbContext` with Identity
- [ ] Add `[Authorize(Roles = "User")]` where needed
- [ ] Create Auth controller (register/login)
- [ ] Implement role seeding

**But FIRST, focus on Phase 1 completion.**

---

## Files to Modify

1. ✅ `HotelListing.API/Program.cs` — Add auth registration & middleware
2. ✅ `HotelListing.API/appsettings.json` — Add JWT config section
3. ✅ `HotelListing.API/Controllers/HotelsController.cs` — Add [Authorize]
4. ✅ `HotelListing.API/Controllers/CountriesController.cs` — Add [Authorize]
5. ✅ `.gitignore` — Add secrets patterns
6. ✅ `HotelListing.API.csproj` — Install auth packages (via dotnet cli)

---

**Audit:** api-auth-hardening skill  
**Status:** Implementation guide for Phase 1  
**Estimated Time:** ~2 hours  
**Success Criteria:** All 6 critical fixes complete + build succeeds
