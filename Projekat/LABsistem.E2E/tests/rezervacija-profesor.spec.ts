import { test, expect } from '@playwright/test';

test.describe('Rezervacijski tok - Profesor', () => {

  test('Profesor može rezervirati, otkazati i kreirati skrivenu rezervaciju', async ({ page }) => {
    
    // 1. Prijava na sustav
    await page.goto('https://labsistem.duckdns.org/login');
    await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('profesor');
    await page.getByRole('textbox', { name: 'Lozinka:' }).fill('profesor123');
    await page.getByRole('button', { name: 'Prijavi se' }).click();

    // 2. Odlazak na Termine i prva rezervacija
    await expect(page.getByRole('link', { name: 'Termini' })).toBeVisible({ timeout: 10000 });
    await page.getByRole('link', { name: 'Termini' }).click();
    
    await expect(page.getByRole('button', { name: 'Rezervisi' }).first()).toBeVisible();
    await page.getByRole('button', { name: 'Rezervisi' }).first().click();
    await page.getByRole('button', { name: 'Potvrdi rezervaciju' }).click();

    // Navigacija na listu rezervacija (stabilna verzija za sve preglednike)
    await page.goto('https://labsistem.duckdns.org/rezervacije');

    // 3. Otkazivanje rezervacije (Slušatelj dijaloga se stavlja ODMAH PRIJE klika)
    page.once('dialog', dialog => {
      console.log(`Pojavio se prozor: ${dialog.message()}`);
      dialog.accept().catch(() => {}); // .accept() simulira klik na "U redu" / "Da"
    });
    await page.getByRole('button', { name: 'Otkazi' }).first().click(); // .first() osigurava da klikne prvi ako ih ima više

    // 4. Druga rezervacija s dodatnim opcijama
    await page.goto('https://labsistem.duckdns.org/termini'); // Izravno idemo na termine radi stabilnosti brzine
    
    await expect(page.getByRole('button', { name: 'Rezervisi' }).nth(1)).toBeVisible();
    await page.getByRole('button', { name: 'Rezervisi' }).nth(1).click();
    
    // Skidanje kvačice "Vidljivo studentima"
    await page.getByRole('checkbox', { name: 'Vidljivo studentima' }).uncheck();
    await page.getByRole('button', { name: 'Potvrdi rezervaciju' }).click();

    // 5. Konačni odlazak na listu rezervacija radi provjere
    await page.goto('https://labsistem.duckdns.org/rezervacije');
    //await expect(page).toHaveURL(/.*rezervacija.*/);
  });

});