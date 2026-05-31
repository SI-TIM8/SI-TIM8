import { test, expect } from '@playwright/test';

test.describe('Tok Upravljanja Terminima', () => {
  test.beforeEach(async ({ page }) => {
    // Tehničar sesija
    await page.addInitScript(() => {
      const futureDate = Date.now() + 86400000;
      window.localStorage.setItem('token', 'tehnicar-token');
      window.localStorage.setItem('tokenExpiry', futureDate.toString());
      window.localStorage.setItem('uloga', 'tehnicar');
    });

    // Mockanje rute za termine
    await page.route('**/Termin', async (route) => {
      if (route.request().method() === 'GET') {
        await route.fulfill({ status: 200, json: [] });
      } else if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 200,
          json: { message: 'Termin uspjesno dodan.' }
        });
      } else {
        await route.continue();
      }
    });

    // Mockanje rute za kabinete
    await page.route('**/Kabinet', async (route) => {
      await route.fulfill({
        status: 200,
        json: [
          { id: 1, naziv: 'Kabinet 1', objekatLokacija: 'Kampus', kapacitet: 20 }
        ]
      });
    });

    await page.goto('http://localhost:5173/termini');
  });

  test('Treba omogućiti dodavanje novog termina', async ({ page }) => {
    // Otvori modal za kreiranje
    await page.click('button:has-text("Dodaj termin")');

    // Ispunjavanje forme
    await page.fill('input[name="datum"]', '2026-10-10');
    await page.fill('input[name="vrijemePocetka"]', '10:00');
    await page.fill('input[name="vrijemeKraja"]', '12:00');
    await page.selectOption('select[name="kabinetID"]', '1');

    await page.click('button:has-text("Sacuvaj")');

    // Provjera prikaza toast/uspješne poruke
    await expect(page.locator('text=Termin uspjesno dodan.')).toBeVisible();
  });
});