describe('Tok Upravljanja Korisnicima', () => {
  beforeEach(() => {
    // Admin sesija
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'admin-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'admin');

    cy.intercept('GET', '**/Auth/users', {
      statusCode: 200,
      body: [
        {
          userId: 1,
          imePrezime: "Postojeći Korisnik",
          email: "postojeci@test.com",
          username: "postojeci.k",
          role: "Student",
          emailVerified: true
        }
      ]
    }).as('getUsers');

    cy.intercept('POST', '**/Auth/create-user*', {
      statusCode: 200,
      body: { message: "Korisnik je uspješno kreiran." }
    }).as('createUser');

    cy.visit('/korisnici');
  });

  it('Treba prikazati korisnike i omogućiti dodavanje novog', () => {
    cy.wait('@getUsers');
    cy.contains('Postojeći Korisnik').should('be.visible');

    // Otvaranje modala
    cy.contains('button', 'Dodaj novog korisnika').click();
    cy.contains('h2', 'Dodaj novog korisnika').should('be.visible');

    // Unos podataka
    cy.get('input#imePrezime').type('Novi Korisnik');
    cy.get('input#email').type('novi@test.com');
    cy.get('input#username').type('novi.korisnik');
    cy.get('select#uloga').select('3'); // 3 = Student
    cy.get('input#newPassword').type('SigurnaLozinka123!');

    // Slanje zahtjeva
    cy.contains('button', 'Kreiraj korisnika').click();
    cy.wait('@createUser');

    // Provjera uspjeha
    cy.contains('Korisnik je uspješno kreiran.').should('be.visible');
  });
});