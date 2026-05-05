## Proof of Testing - Sprint6

### Testirane su sljedeće funkcionalne cjeline:
 * JWT Service testovi
 * AuthService – Login i session management
 * Refresh token logika
 * Upravljanje korisnicima
 * Aktivacija / deaktivacija korisnika
 * Revoked token store
 * Seeder testovi
 * Controller testovi
 * Integracijski testovi (API + DB)

#### JWT Service testovi
| Test slučaj                                    | Očekivani rezultat                        | Status | Dokaz                            |
| ---------------------------------------------- | ----------------------------------------- | ------ | -------------------------------- |
| Generisanje access tokena sa validnim podacima | Vraćen validan JWT token                  | PASS   | Token nije prazan i sadrži “.”   |
| Generisanje refresh tokena više puta           | Svaki token je jedinstven                 | PASS   | `Assert.NotEqual(first, second)` |
| Validacija validnog tokena                     | Vraćen ClaimsPrincipal sa tačnim podacima | PASS   | ClaimTypes (ID, Username, Role)  |
| Dohvatanje expiration vremena                  | Vrijeme u budućnosti                      | PASS   | `expiresAtUtc > DateTime.UtcNow` |
| Validacija neispravnog tokena                  | Vraća null                                | PASS   | Invalid i empty token            |
| Validacija null tokena                         | Vraća null                                | PASS   | Null handling                    |

#### AuthService – Login i session management
| Test slučaj                             | Očekivani rezultat                 | Status | Dokaz                     |
| --------------------------------------- | ---------------------------------- | ------ | ------------------------- |
| Login sa validnim hashiranim passwordom | Generisani access i refresh token  | PASS   | Token + DB zapis          |
| Login sa neaktivnim korisnikom          | Login odbijen                      | PASS   | `result == null`          |
| Login sa plaintext passwordom (legacy)  | Password hashiran i login uspješan | PASS   | BCrypt verify + DB update |
| Persistiranje refresh tokena            | Token sačuvan u bazi               | PASS   | DB provjera               |

#### Refresh token logika
| Test slučaj                        | Očekivani rezultat              | Status | Dokaz                  |
| ---------------------------------- | ------------------------------- | ------ | ---------------------- |
| Validan refresh token              | Novi access + refresh token     | PASS   | Rotacija tokena        |
| Korištenje starog refresh tokena   | Odbijeno                        | PASS   | Replay protection      |
| Refresh za deaktiviranog korisnika | Odbijen i token revoked         | PASS   | DB revoked flag        |
| Rotacija tokena                    | Stari token označen kao revoked | PASS   | `RevokedAtUtc != null` |

#### Upravljanje korisnicima
| Test slučaj                               | Očekivani rezultat   | Status | Dokaz                 |
| ----------------------------------------- | -------------------- | ------ | --------------------- |
| Update profila sa zauzetim username       | Neuspješno           | PASS   | Validaciona poruka    |
| Update profila sa predugim emailom        | Neuspješno           | PASS   | Max length validacija |
| Admin uređuje vlastiti nalog (restricted) | Odbijeno             | PASS   | Business rule         |
| Update bez nove lozinke                   | Lozinka ostaje ista  | PASS   | Hash nepromijenjen    |
| Update sa novom lozinkom                  | Lozinka promijenjena | PASS   | BCrypt verify         |

#### Aktivacija / deaktivacija korisnika
| Test slučaj                          | Očekivani rezultat      | Status | Dokaz                   |
| ------------------------------------ | ----------------------- | ------ | ----------------------- |
| Deaktivacija korisnika               | Korisnik deaktiviran    | PASS   | `DeactivatedAt != null` |
| Deaktivacija revokuje refresh tokene | Tokeni nevažeći         | PASS   | DB revoked              |
| Deaktivacija samog sebe              | Odbijeno                | PASS   | Business rule           |
| Deaktivacija admina                  | Odbijena                | PASS   | Role protection         |
| Aktivacija korisnika                 | Korisnik ponovo aktivan | PASS   | `DeactivatedAt == null` |

#### Revoked token store
| Test slučaj                   | Očekivani rezultat        | Status | Dokaz                    |
| ----------------------------- | ------------------------- | ------ | ------------------------ |
| Revokacija tokena             | Token označen kao revoked | PASS   | DB zapis                 |
| Persistencija između instanci | Token i dalje revoked     | PASS   | Novi context vidi stanje |

#### Seeder testovi
| Test slučaj                    | Očekivani rezultat                          | Status | Dokaz         |
| ------------------------------ | ------------------------------------------- | ------ | ------------- |
| Seed sa postojećim korisnicima | Podaci overwrite-ani                        | PASS   | DB provjera   |
| Seed default korisnika         | Admin, Profesor, Student, Tehničar kreirani | PASS   | 4 korisnika   |
| Password hashing u seedu       | Lozinke hashirane                           | PASS   | BCrypt verify |

#### Controller testovi
| Test slučaj                      | Očekivani rezultat | Status | Dokaz                    |
| -------------------------------- | ------------------ | ------ | ------------------------ |
| Login endpoint validan           | 200 OK             | PASS   | OkObjectResult           |
| Refresh invalid token            | 401 Unauthorized   | PASS   | UnauthorizedObjectResult |
| Update user                      | 200 OK             | PASS   | Poruka o uspjehu         |
| Deactivate user                  | 200 OK             | PASS   | Status promijenjen       |
| Activate user                    | 200 OK             | PASS   | Status aktivan           |
| Verify token za neaktivnog usera | 401                | PASS   | Unauthorized             |
| Logout bez jti                   | 400 BadRequest     | PASS   | Error message            |
| Logout validan                   | Tokeni revoked     | PASS   | Verify calls             |

#### Integracijski testovi (API + DB)
| Test slučaj                        | Očekivani rezultat           | Status | Dokaz                |
| ---------------------------------- | ---------------------------- | ------ | -------------------- |
| Login endpoint validan             | Tokeni vraćeni               | PASS   | HTTP 200             |
| Login neaktivan korisnik           | 401                          | PASS   | Unauthorized         |
| Refresh endpoint                   | Novi tokeni                  | PASS   | Token rotacija       |
| Logout endpoint                    | Refresh token nevažeći       | PASS   | 401 nakon logout     |
| Verify endpoint (deaktiviran user) | 401                          | PASS   | Unauthorized         |
| Deactivate endpoint                | Blokira refresh              | PASS   | 401                  |
| Update profile endpoint            | Podaci izmijenjeni           | PASS   | Response body        |
| Change password endpoint           | Login sa novom lozinkom radi | PASS   | Old fails, new works |
| Verify token invalid               | 401                          | PASS   | Unauthorized         |
