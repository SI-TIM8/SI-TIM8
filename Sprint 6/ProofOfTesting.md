#  Proof of Testing 

Ovaj izvještaj dokumentuje validaciju sistema kroz dva nivoa testiranja: **Unit** (izolacija pomoću `Moq`) i **Integration** (interakcija sa `EF Core In-Memory`).

---

## 1. Modul: Autentifikacija & Korisnici (AuthService)
*Fokus: Sigurnosni protokoli, JWT rotacija i administrativna pravila.*

### Unit Tests (Logic Isolation)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **JWT Generation** | Provjera da li `IJwtService` ispravno kreira Claim-ove (ID, Role, Username). | **PASS** | `Assert.NotNull(token)` |
| **BCrypt Verification** | Validacija login procesa za hashirane lozinke. | **PASS** | `BCrypt.Verify` uspješan |
| **Business Rule: Admin** | Zabrana administratoru da deaktivira vlastiti nalog ili uređuje isti kroz panel. | **PASS** | `Success == false` |
| **Refresh Replay** | Detekcija i blokiranje pokušaja ponovnog korištenja istog Refresh tokena. | **PASS** | Druga upotreba vraća `null` |

### Integration Tests (Database & Persistence)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **Legacy Migration** | Provjera da li sistem automatski hashira i spašava lozinku u bazu ako je bila plain-text. | **PASS** | `Update` pozvan u bazi |
| **Token Persistence** | Potvrda da se Refresh tokeni ispravno upisuju u tabelu `RefreshTokens`. | **PASS** | `context.RefreshTokens` zapis |
| **Session Revocation** | Provjera da li deaktivacija korisnika u bazi zaista poništava sve njegove sesije. | **PASS** | `RevokedAtUtc != null` |

---

## 2. Modul: Inventura & Oprema (OpremaService)
*Fokus: Upravljanje resursima i praćenje fizičkog stanja.*

### Unit Tests (Logic Isolation)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **DTO Mapping** | Provjera da li se podaci iz `OpremaCreateDTO` ispravno mapiraju na entitet. | **PASS** | `Assert.Equal(dto.Naziv, entity.Naziv)` |
| **Status Logic** | Validacija promjene stanja opreme kroz `StatusOpreme` Enum. | **PASS** | `Verify(repo.UpdateAsync)` |

### Integration Tests (Database & Persistence)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **Relational Integrity** | Provjera da li oprema ispravno referencira `KabinetID` i `ZgradaID` u bazi. | **PASS** | Foreign Key provjera |
| **Inventory List** | Testiranje kompleksnog upita koji spaja tabele Oprema, Kabinet i Zgrada. | **PASS** | `GetAllWithKabinetAsync` |

---

## 3. Modul: Rezervacije (TerminService)
*Fokus: Raspored kabineta i rad sa vremenskim intervalima.*

### Unit Tests (Logic Isolation)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **TimeSpan Validation** | Provjera da li servis ispravno obrađuje `VrijemePocetka` i `VrijemeKraja`. | **PASS** | `Assert.Equal(TimeSpan, entity)` |
| **Non-Existent Check** | Osiguranje da brisanje nepostojećeg termina vraća `false` umjesto Exception-a. | **PASS** | Null handling |

### Integration Tests (Database & Persistence)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **Schedule Persistence** | Potvrda da je kreirani termin trajno sačuvan u bazi nakon `SaveChangesAsync()`. | **PASS** | `CountAsync() == 1` |
| **Full Details Query** | Provjera da li `GetAllWithDetails` ispravno povlači ime profesora i naziv kabineta. | **PASS** | Join result check |

---

## 4. Modul: Evidencija (EvidencijaService)
*Fokus: Monitoring aktivnosti i revizija (Audit).*

### Unit Tests (Logic Isolation)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **Status Update Logic** | Provjera da li se status evidencije (npr. "Završeno") ispravno mijenja u servisu. | **PASS** | `Assert.Equal("Završeno", status)` |
| **Repo Interaction** | Verifikacija da servis poziva `AddAsync` na repozitoriju tačno jednom. | **PASS** | `Times.Once` |

### Integration Tests (Database & Persistence)
| Test Case | Opis | Status | Dokaz |
| :--- | :--- | :--- | :--- |
| **Audit Trail** | Provjera da li se uz svaki zapis evidencije ispravno vežu `KorisnikID` i `OpremaID`. | **PASS** | DB Save Verification |
| **Complex Joins** | Testiranje performansi i tačnosti upita koji povezuje evidencije sa detaljima opreme. | **PASS** | `Select new { ... }` test |

---

## Korištene tehnologije i metodologija
* **XUnit & Moq:** Za izolaciju i testiranje biznis logike (Unit).
* **AutoFixture:** Za generisanje nasumičnih, ali validnih testnih objekata (izbjegavanje rekurzija).
* **EF Core In-Memory:** Za brzo izvršavanje integracijskih testova bez side-effekata.
* **Arrange-Act-Assert Pattern:** Standardna struktura za maksimalnu čitljivost testova.

---
**Zaključak:** Sistem je prošao sve faze validacije. Jasno razdvajanje na Unit i Integration nivoe potvrđuje stabilnost algoritama i pouzdanost perzistencije podataka.
