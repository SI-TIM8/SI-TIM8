describe('Tok Odobravanja Zahtjeva', () => {
  beforeEach(() => {
    const futureDate = Date.now() + 86400000;
    window.localStorage.setItem('token', 'profesor-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'profesor');

    // ISPRAVLJENA RUTA
    cy.intercept('GET', '**/Rezervacija/dolazni-zahtjevi', {
      statusCode: 200,
      body: [
        {
          id: 5,
          studentIme: "Marko Marković",
          kabinetNaziv: "Laboratorija 1",
          datum: "2026-12-01T00:00:00",
          vrijemePocetka: "10:00:00",
          vrijemeKraja: "12:00:00",
          statusZahtjeva: "Na Cekanju" // dodali smo property koji vaša app koristi
        }
      ]
    }).as('getZahtjevi');

    // Ruta za odgovaranje u Zahtjevi.jsx je POST /Rezervacija/odgovor/{id}?odobri={bool}
    cy.intercept('POST', '**/Rezervacija/odgovor/5?odobri=true', {
      statusCode: 200,
      body: "Zahtjev odobren."
    }).as('odobriZahtjev');

    cy.visit('/zahtjevi');
  });

  it('Treba prikazati zahtjeve studenata i omogućiti odobravanje', () => {
    cy.wait('@getZahtjevi');

    cy.contains('Marko Marković').should('be.visible');
    
    // Vaša komponenta ima dugmad 'Odobri' i 'Odbij'
    cy.contains('Odobri').click(); 
    
    cy.wait('@odobriZahtjev');
    
    cy.contains('Zahtjev odobren.').should('be.visible');
  });
});