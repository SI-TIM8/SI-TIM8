import { test, expect } from '@playwright/test';

test.describe('Tok Uređivanja Profila', () => {
  test.beforeEach(async ({ page }) => {
    // 1. Mockanje sesije u localStorage-u prije učitavanja stranice
    await page.addInitScript(() => {
      const futureDate = Date.now() + 86400000;
      window.localStorage.setItem('token', 'fake-token');
      window.localStorage.setItem('tokenExpiry', futureDate.toString());
      window.localStorage.setItem('uloga', 'student');
    });

    // 2. Presretanje GET poziva za učitavanje profila
    await page.route('**/Auth/profile', async (route) => {
      if (route.request().method() === 'GET') {
        await route.fulfill({
          status: 200,
          json: {
            imePrezime: 'Test Student',
            email: 'test@student.com',
            username: 'student.test',
            role: 'Student',
            status: 'Aktivan',
            emailVerified: true
          }
        });
      } else if (route.request().method() === 'PUT') {
        // 3. Presretanje PUT poziva za ažuriranje profila
        await route.fulfill({
          status: 200,
          json: {
            message: 'Profil je uspješno ažuriran.',
            profile: {
              imePrezime: 'Test Student Izmijenjeno',
              email: 'test@student.com',
              username: 'student.test'
            }
          }
        });
      } else {
        await route.continue();
      }
    });

    await page.goto('http://localhost:5173/profil'); // Zamijeni portom na kojem ti radi frontend
  });

  test('Treba prikazati podatke profila i uspješno ih ažurirati', async ({ page }) => {
    // Provjera da li su podaci pravilno učitani
    await expect(page.locator('text=Test Student')).toBeVisible();
    await expect(page.locator('input#imePrezime')).toHaveValue('Test Student');

    // Ažuriranje imena
    await page.fill('input#imePrezime', 'Test Student Izmijenjeno');
    await page.click('button:has-text("Sačuvaj")');

    // Provjera uspješne poruke
    await expect(page.locator('text=Profil je uspješno ažuriran.')).toBeVisible();
  });
});