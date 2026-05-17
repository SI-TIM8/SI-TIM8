describe('Tok Prijave (Login)', () => {
  beforeEach(() => {
    cy.clearLocalStorage();

    cy.intercept('POST', '**/Auth/login', {
      statusCode: 200,
      body: {
        token: "fake-jwt-token",
        refreshToken: "fake-refresh-token",
        accessTokenExpiresAtUtc: new Date(Date.now() + 3600000).toISOString(),
        refreshTokenExpiresAtUtc: new Date(Date.now() + 86400000).toISOString(),
        role: "student",
        username: "student.test",
        userId: 1
      }
    }).as('loginRequest');

    cy.visit('/login');
  });

  it('Treba uspješno prijaviti korisnika i spasiti sesiju', () => {
    // Korištenje tačnih ID-eva iz Login.jsx
    cy.get('input#username').type('student.test');
    cy.get('input#password').type('Password123!');
    
    cy.contains('button', 'Prijavi se').click(); 

    cy.wait('@loginRequest');

    cy.url().should('not.include', '/login');
    
    cy.window().then((win) => {
      expect(win.localStorage.getItem('token')).to.eq('fake-jwt-token');
      expect(win.localStorage.getItem('uloga')).to.eq('student');
    });
  });
});