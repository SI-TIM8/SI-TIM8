import { test, expect } from '@playwright/test';

test.describe('Tok Upravljanja Korisnicima', () => {
  test.beforeEach(async ({ page }) => {
    // Admin sesija
    await page.addInitScript(() => {
      const futureDate = Date.now() + 86400000;
      window.localStorage.setItem('token', 'admin-token');
      window.localStorage.setItem('tokenExpiry', futureDate.toString());
      window.localStorage.setItem('uloga', 'admin');
    });

    // Mockanje dohvatanja liste korisnika
    await page.route('**/Auth/users', async (route) => {
      await route.fulfill({
        status: 200,
        json: [
          {
            userId: 1,
            imePrezime: 'Postojeći Korisnik',
            email: 'postojeci@test.com',
            username: 'postojeci.k',
            role: 'Student',
            emailVerified: true
          }
        ]
      });
    });

    // Mockanje kreiranja korisnika
    await page.route('**/Auth/create-user*', async (route) => {
      await route.fulfill({
        status: 200,
        json: { message: 'Korisnik je uspješno kreiran.' }
      });
    });

    await page.goto('http://localhost:5173/korisnici');
  });

  test('Treba prikazati korisnike i omogućiti dodavanje novog', async ({ page }) => {
    // Provjera inicijalnog učitavanja
    await expect(page.locator('text=Postojeći Korisnik')).toBeVisible();

    // Otvaranje modala
    await page.click('button:has-text("Dodaj novog korisnika")');
    await expect(page.locator('h2:has-text("Dodaj novog korisnika")')).toBeVisible();

    // Unos podataka
    await page.fill('input#imePrezime', 'Novi Korisnik');
    await page.fill('input#email', 'novi@test.com');
    await page.fill('input#username', 'novi.korisnik');
    await page.selectOption('select#uloga', '3'); // 3 = Student
    await page.fill('input#newPassword', 'SigurnaLozinka123!');

    // Slanje zahtjeva
    await page.click('button:has-text("Kreiraj korisnika")');

    // Provjera uspjeha
    await expect(page.locator('text=Korisnik je uspješno kreiran.')).toBeVisible();
  });
});