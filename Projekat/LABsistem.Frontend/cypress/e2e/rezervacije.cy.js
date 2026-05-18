describe('Tok Zakazivanja Termina', () => {
  beforeEach(() => {
    // 1. MOCKANJE SESIJE (Simuliramo da je Student prijavljen)
    const futureDate = Date.now() + 86400000; // Token ističe za 1 dan
    window.localStorage.setItem('token', 'lazni-test-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'student');

    // 2. Mockanje API poziva za dohvatanje dostupnih termina
    cy.intercept('GET', '**/Rezervacija/dostupni-studentima', {
      statusCode: 200,
      body: [
        {
          id: 1,
          datum: "2026-12-01T00:00:00",
          vrijemePocetka: "10:00:00",
          vrijemeKraja: "12:00:00",
          kabinetNaziv: "Laboratorija 1",
          kabinetID: 101,
          profesorIme: "Dr. Ime Prezime",
          brojOdobrenih: 5,
          limitOsoba: 15,
          statusPrijave: null
        }
      ]
    }).as('getDostupniTermini');

    // 3. Mockanje API poziva za slanje zahtjeva
    cy.intercept('POST', '**/Rezervacija/zahtjev/1', {
      statusCode: 200,
      body: "Zahtjev uspjesno poslan."
    }).as('posaljiZahtjev');

    // Posjeti stranicu za zakazivanje
    cy.visit('/zakazivanje'); 
  });

  it('Treba prikazati dostupne termine i uspješno poslati zahtjev', () => {
    cy.wait('@getDostupniTermini');

    // Provjeri da li je termin prikazan u tabeli
    cy.contains('Laboratorija 1').should('be.visible');
    cy.contains('Dr. Ime Prezime').should('be.visible');

    // Klikni na dugme za slanje zahtjeva
    cy.contains('Pošalji zahtjev').click();

    // Provjeri da li je API pozvan
    cy.wait('@posaljiZahtjev');

    // Provjeri optimistično ažuriranje UI-ja
    cy.contains('Zahtjev u obradi').should('be.visible');
    
    // Provjeri poruku o uspjehu
    cy.contains('Zahtjev uspjesno poslan.').should('have.class', 'form-success');
  });
});

describe('Tok Otkazivanja Rezervacije', () => {
  beforeEach(() => {
    // 1. MOCKANJE SESIJE (Simuliramo da je Profesor prijavljen)
    const futureDate = Date.now() + 86400000; // Token ističe za 1 dan
    window.localStorage.setItem('token', 'lazni-test-token');
    window.localStorage.setItem('tokenExpiry', futureDate.toString());
    window.localStorage.setItem('uloga', 'profesor'); 

    // 2. Mockanje API poziva za dohvatanje rezervacija
    cy.intercept('GET', '**/Rezervacija/moje', {
      statusCode: 200,
      body: [
        {
          id: 2,
          datum: "2026-12-05T00:00:00",
          vrijemePocetka: "14:00:00",
          vrijemeKraja: "16:00:00",
          kabinetNaziv: "Laboratorija 2",
          statusTermina: "Zauzet"
        }
      ]
    }).as('getMojeRezervacije');

    // 3. Mockanje API poziva za otkazivanje
    cy.intercept('POST', '**/Rezervacija/otkazi/2', {
      statusCode: 200
    }).as('otkaziRezervaciju');

    cy.visit('/rezervacije'); 
  });

  it('Treba prikazati upozorenje i otkazati rezervaciju nakon potvrde', () => {
    cy.wait('@getMojeRezervacije');

    // Provjeri da li se termin učitao
    cy.contains('Laboratorija 2').should('be.visible');

    // Eksplicitno prihvatanje window.confirm
    cy.on('window:confirm', (str) => {
      expect(str).to.equal('Da li ste sigurni da zelite otkazati rezervaciju?');
      return true;
    });

    // Klikni na dugme Otkazi
    cy.contains('Otkazi').click();

    // Sačekaj backend zahtjev
    cy.wait('@otkaziRezervaciju');

    // Provjeri success poruku
    cy.contains('Rezervacija uspjesno otkazana.').should('be.visible');
  });
});