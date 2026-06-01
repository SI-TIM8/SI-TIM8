describe('Tok Dodavanja Opreme', () => {
  beforeEach(() => {
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'tehnicar-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'tehnicar');

    cy.intercept('GET', '**/Oprema?prikaz=aktivna', {
      statusCode: 200,
      body: []
    }).as('getOprema');

    cy.intercept('GET', '**/Objekat', {
      statusCode: 200,
      body: [
        {
          id: 1,
          lokacija: "Kampus",
          kabineti: [{ id: 10, naziv: "Lab 1" }]
        }
      ]
    }).as('getObjekti');

    cy.intercept('POST', '**/Oprema', {
      statusCode: 200,
      body: { message: "Oprema je uspješno dodana." }
    }).as('createOprema');

    cy.visit('/oprema');
  });

  it('Treba omogućiti tehničaru dodavanje nove opreme', () => {
    cy.wait('@getOprema');
    cy.wait('@getObjekti');

    cy.contains('button', 'Dodaj opremu').click();

    cy.get('input[name="naziv"]').type('Novi Mikroskop');
    cy.get('input[name="kategorija"]').type('Mikroskop');
    cy.get('select[name="stanje"]').select('1'); // 1 = Ispravno
    
    // U Oprema.jsx postoje 3 select polja u modalu: Stanje, Objekat, Kabinet. 
    // Selektujemo Objekat (koji je drugi select po redu):
    cy.get('select').eq(1).select('1'); 
    
    // Zatim selektujemo kabinet koji se otključao:
    cy.get('select[name="kabinetID"]').select('10');

    cy.contains('button', 'Sačuvaj').click();
    
    cy.wait('@createOprema');
    cy.contains('Oprema je uspješno dodana.').should('be.visible');
  });
});