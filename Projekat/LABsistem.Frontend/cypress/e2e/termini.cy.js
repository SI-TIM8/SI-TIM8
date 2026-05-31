describe('Tok Upravljanja Terminima', () => {
  beforeEach(() => {
    // Tehničar sesija
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'tehnicar-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'tehnicar');

    cy.intercept('GET', '**/Termin', {
      statusCode: 200,
      body: []
    }).as('getTermini');

    cy.intercept('GET', '**/Kabinet', {
      statusCode: 200,
      body: [
        { id: 1, naziv: "Kabinet 1", objekatLokacija: "Kampus", kapacitet: 20 }
      ]
    }).as('getKabineti');

    cy.intercept('POST', '**/Termin', {
      statusCode: 200,
      body: { message: "Termin uspjesno dodan." }
    }).as('createTermin');

    cy.visit('/termini');
  });

  it('Treba omogućiti dodavanje novog termina', () => {
    cy.wait('@getTermini');
    cy.wait('@getKabineti');

    // Otvori modal za kreiranje
    cy.contains('button', 'Dodaj termin').click();

    // Ispunjavanje forme
    // Date input zahtijeva YYYY-MM-DD format u Cypressu
    cy.get('input[name="datum"]').type('2026-10-10'); 
    cy.get('input[name="vrijemePocetka"]').type('10:00');
    cy.get('input[name="vrijemeKraja"]').type('12:00');
    cy.get('select[name="kabinetID"]').select('1');

    cy.contains('button', 'Sacuvaj').click();

    cy.wait('@createTermin');
    cy.contains('Termin uspjesno dodan.').should('be.visible');
  });
});