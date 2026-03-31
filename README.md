# Quantity Measurement App
## Project Structure
```
QuantityMeasurementApp/
├── Backend/          ← ASP.NET Core Web API (.NET 10)
└── Frontend/         ← Angular 17 SPA
```

---

## ✅ UC18 – Authentication (JWT + Google OAuth)

### What's implemented
| Feature | Status |
|---|---|
| Register with email/password (SHA-512 salted hash) | ✅ Done (your original) |
| Login with email/password → JWT | ✅ Done (your original) |
| Google Sign-In (ID token verification server-side) | ✅ Added |
| CORS for Angular (localhost:4200) | ✅ Added |
| JWT attached to all protected API calls | ✅ Done |

---

## 🚀 Setup Instructions

### Step 1 – Google Cloud Console
1. Go to https://console.cloud.google.com
2. Create a project → APIs & Services → Credentials
3. Create **OAuth 2.0 Client ID** → Web Application
4. Authorized JavaScript origins: `http://localhost:4200`
5. Authorized redirect URIs: `http://localhost:4200`
6. Copy your **Client ID**

### Step 2 – Backend Configuration
Edit `Backend/WebApiLayer/appsettings.json`:
```json
"Google": {
  "ClientId": "PASTE_YOUR_CLIENT_ID_HERE.apps.googleusercontent.com"
},
"Jwt": {
  "Key": "YourSecretKeyMinimum32CharactersLong!!",
  "Issuer": "QuantityMeasurementApp",
  "Audience": "QuantityMeasurementApp",
  "ExpiryMinutes": 60
}
```

### Step 3 – Run Database Migration
```bash
cd Backend/WebApiLayer
dotnet restore
dotnet ef database update --project ../DataAccessLayer
```
> This applies the new `AddGoogleAuthFields` migration which adds:
> - `GoogleId` column
> - `ProfilePicture` column  
> - `AuthProvider` column (default: "local")
> - Makes `HashedPassword` and `Salt` nullable

### Step 4 – Start Backend
```bash
cd Backend/WebApiLayer
dotnet run
# Runs on http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Step 5 – Configure Frontend
Edit `Frontend/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  googleClientId: 'PASTE_YOUR_CLIENT_ID_HERE.apps.googleusercontent.com'
};
```

### Step 6 – Start Frontend
```bash
cd Frontend
npm install
ng serve
# Runs on http://localhost:4200
```

---

## 📡 API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | None | Register with email/password |
| POST | `/api/auth/login` | None | Login → returns JWT |
| POST | `/api/auth/google` | None | Google ID token → returns JWT |
| POST | `/api/quantitymeasurement/convert` | Bearer JWT | Unit conversion |
| POST | `/api/quantitymeasurement/add` | Bearer JWT | Add two quantities |
| POST | `/api/quantitymeasurement/subtract` | Bearer JWT | Subtract two quantities |
| POST | `/api/quantitymeasurement/compare` | Bearer JWT | Compare two quantities |

---

## 🔐 How Google Auth Works (Flow)

```
Angular (Login page)
  → User clicks "Sign in with Google"
  → Google popup appears (Google Identity Services SDK)
  → User picks account & consents
  → Google returns an ID Token (JWT signed by Google)

Angular
  → Sends ID Token to POST /api/auth/google

Backend (UserService.GoogleLoginAsync)
  → Calls GoogleJsonWebSignature.ValidateAsync(idToken)
  → Verifies token signature + audience (your ClientId)
  → Extracts: email, name, picture, GoogleId (sub)
  → Creates user in DB if first time (AuthProvider = "google")
  → Issues your own JWT token

Angular
  → Stores JWT in localStorage
  → Navigates to /dashboard
```

---

## UC19 – Frontend (Angular)

| Feature | Status |
|---|---|
| Login page (email/password + Google button) | ✅ |
| Register page (validation, password confirm) | ✅ |
| JWT interceptor (auto-attaches Bearer token) | ✅ |
| Auth guard (protects /dashboard route) | ✅ |
| Dashboard – Convert length/weight/temperature/volume | ✅ |
| Dashboard – Add, Subtract, Compare operations | ✅ |
| Navbar with user name + profile picture (Google) | ✅ |
| Responsive layout (mobile-friendly) | ✅ |
| Redirect if already logged in | ✅ |
