import { test, expect } from '@playwright/test';

test.describe('Tok Dodavanja Opreme', () => {
  test.beforeEach(async ({ page }) => {
    // Tehničar sesija
    await page.addInitScript(() => {
      const futureDate = Date.now() + 86400000;
      window.localStorage.setItem('token', 'tehnicar-token');
      window.localStorage.setItem('tokenExpiry', futureDate.toString());
      window.localStorage.setItem('uloga', 'tehnicar');
    });

    await page.route('**/Oprema?prikaz=aktivna', async (route) => {
      await route.fulfill({ status: 200, json: [] });
    });

    await page.route('**/Objekat', async (route) => {
      await route.fulfill({
        status: 200,
        json: [
          {
            id: 1,
            lokacija: 'Kampus',
            kabineti: [{ id: 10, naziv: 'Lab 1' }]
          }
        ]
      });
    });

    await page.route('**/Oprema', async (route) => {
      if (route.request().method() === 'POST') {
        await route.fulfill({
          status: 200,
          json: { message: 'Oprema je uspješno dodana.' }
        });
      } else {
        await route.continue();
      }
    });

    await page.goto('http://localhost:5173/oprema');
  });

  test('Treba omogućiti tehničaru dodavanje nove opreme', async ({ page }) => {
    await page.click('button:has-text("Dodaj opremu")');

    await page.fill('input[name="naziv"]', 'Novi Mikroskop');
    await page.fill('input[name="kategorija"]', 'Mikroskop');
    
    // Select opcije po value atributu
    await page.selectOption('select[name="stanje"]', '1'); // Ispravno
    
    // Selektovanje Objekta (prvi dostupan <select> koji nema atribut name, oslanjamo se na indeks ili sličan selektor)
    // Budući da react komponenta obično ima ugniježđeni select, koristit ćemo page.locator sa filterom.
    await page.locator('select').nth(1).selectOption('1'); 
    
    // Zatim selektujemo kabinet koji se otključao
    await page.selectOption('select[name="kabinetID"]', '10');

    await page.click('button:has-text("Sačuvaj")');
    
    await expect(page.locator('text=Oprema je uspješno dodana.')).toBeVisible();
  });
});