# Authentication strategies

## 1. What Is JWT
- JWT (JSON Web Token) is a compact, self-contained token format often used to
    - Authenticate users in APIs.
    - Avoid server-side session storage.
    - Pass user identity and claims between services.
- The **JWT Header** contains metadata about the type of token and the cryptographic algorithm used to sign the token. It usually includes two fields:

    - alg: Specifies the signing algorithm (e.g., HMACSHA256, RSA, etc.).
    - typ: Specifies the token type, which is usually “JWT”.    
- Key Concepts
    - **Refresh tokens** are longer-lived and stored securely by the client.
    - You should revoke **refresh tokens** after use (rotate them)


## 2. Multi-Tenant Authentication
### Why Multi-Tenant Authentication?
In multi-tenant SaaS systems, you often need to:
- Support different identity providers per tenant (e.g., Google, Azure AD)
- Enforce tenant isolation for users and data
- Allow dynamic onboarding of tenants with their own authentication configuration
### Architectural 
A multi-tenant authentication flow typically includes:
- Tenant Resolution — Identify which tenant the request belongs to
- Tenant Context — Load tenant-specific configuration (e.g., IdP, connection strings)
- Authentication — Use the tenant’s assigned authentication scheme
- Authorization — Enforce tenant-scoped access and permissions


## 3. Multi-Factor Authentication (MFA)
### Why Multi-Factor Authentication (MFA)?
Single-step login (username + password) is vulnerable to credential theft. MFA mitigates this by requiring an additional verification step:

- Authenticator app code (TOTP)
- SMS or email verification
- Recovery codes for fallback

```mermaid
flowchart TD
    A[User logs in with username + password] --> B[System prompts for MFA code]
    B --> C[User enters Authenticator / SMS / Recovery code]
    C --> D[Code validated by Identity system]
    D --> E[Sign-in complete]