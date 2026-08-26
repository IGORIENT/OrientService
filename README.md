# OrienteeringService

Web service for sports orienteering.

## Local authentication

The development environment contains:

- PostgreSQL for application data on `localhost:5432`;
- Keycloak on `http://localhost:8080`;
- a preconfigured `orienteering` realm;
- an OAuth client for Swagger using Authorization Code + PKCE.

The credentials below are deliberately simple and must never be used outside local development:

| Purpose | Login | Password |
| --- | --- | --- |
| Keycloak administration | `admin` | `admin` |
| Demo application user | `demo` | `demo` |

Start PostgreSQL and Keycloak:

```powershell
docker compose up -d
```

Start the API:

```powershell
dotnet run --project src/OrienteeringService/OrienteeringService.Web
```

Open `https://localhost:7163/swagger`, click **Authorize**, and sign in as `demo`.
The first authenticated request creates a local domain `User` and an `ExternalIdentity`
link identified by the Keycloak `(iss, sub)` pair.

The API validates token signature, issuer, audience, and expiration. Production must override
the connection string and `Authentication:Authority` through protected configuration and must
use HTTPS for Keycloak.
