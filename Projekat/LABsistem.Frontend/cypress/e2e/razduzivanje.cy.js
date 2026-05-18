describe('Tok Razduživanja i Evidencije Opreme', () => {
  beforeEach(() => {
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'tehnicar-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'tehnicar');

    // MOCKANJE ZA OPREMU (Precizna ruta bez *)
    cy.intercept('GET', '**/Oprema', {
      statusCode: 200,
      body: [
        {
          id: 10,
          naziv: "Osciloskop",
          serijskiBroj: "OSC-123",
          stanje: 1,
          kabinetNaziv: "Lab 1",
          zgradaNaziv: "Glavna zgrada"
        }
      ]
    }).as('getOprema');

    // MOCKANJE ZA OBJEKTE (Precizna ruta bez *)
    cy.intercept('GET', '**/Objekat', {
      statusCode: 200,
      body: []
    }).as('getObjekti');

    // Ovdje zaustavljamo greške ako aplikacija pokuša zatražiti nešto drugo u pozadini
    cy.intercept('POST', '**/Evidencija', {
      statusCode: 200,
      body: "Kvar evidentiran."
    }).as('prijaviKvar');

    cy.visit('/oprema');
  });

  it('Treba omogućiti evidenciju kvara nakon završetka termina', () => {
    // Razdvojeno čekanje je mnogo stabilnije u Cypressu za Promise.all pozive
    cy.wait('@getOprema');
    cy.wait('@getObjekti');

    // Provjerimo da li se tabela učitala
    cy.contains('Osciloskop').should('be.visible');
    
    // Tehničar klika na 'Uredi'
    cy.contains('✎ Uredi').click(); 

    // Ovdje provjeravamo da li se modal otvorio
    cy.contains('Uredi opremu').should('be.visible');
    
    // Mijenjamo status u kvar i čuvamo
    cy.get('select[name="stanje"]').select('2'); // '2' je opcija 'U kvaru'
    cy.contains('button', 'Sačuvaj').click();
  });
});