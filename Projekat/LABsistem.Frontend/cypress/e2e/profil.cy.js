describe('Tok Uređivanja Profila', () => {
  beforeEach(() => {
    // 1. MOCKANJE SESIJE
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'fake-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'student');

    // 2. Mockanje dohvatanja profila
    cy.intercept('GET', '**/Auth/profile', {
      statusCode: 200,
      body: {
        imePrezime: "Test Student",
        email: "test@student.com",
        username: "student.test",
        role: "Student",
        status: "Aktivan",
        emailVerified: true
      }
    }).as('getProfile');

    // 3. Mockanje ažuriranja profila
    cy.intercept('PUT', '**/Auth/profile', {
      statusCode: 200,
      body: {
        message: "Profil je uspješno ažuriran.",
        profile: {
          imePrezime: "Test Student Izmijenjeno",
          email: "test@student.com",
          username: "student.test"
        }
      }
    }).as('updateProfile');

    cy.visit('/profil');
  });

  it('Treba prikazati podatke profila i uspješno ih ažurirati', () => {
    cy.wait('@getProfile');

    // Provjera da li su podaci pravilno učitani
    cy.contains('Test Student').should('be.visible');
    cy.get('input#imePrezime').should('have.value', 'Test Student');

    // Ažuriranje imena
    cy.get('input#imePrezime').clear().type('Test Student Izmijenjeno');
    cy.contains('button', 'Sačuvaj').click();

    cy.wait('@updateProfile');

    // Provjera uspješne poruke
    cy.contains('Profil je uspješno ažuriran.').should('be.visible');
  });
});