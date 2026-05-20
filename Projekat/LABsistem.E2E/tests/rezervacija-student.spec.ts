import { test, expect } from '@playwright/test';

test('Student-slanje zahtjeva za rezervaciju i odobrenje', async ({ page }) => {


  await page.goto('https://labsistem.duckdns.org/login');
  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('student');
  await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  await page.getByRole('textbox', { name: 'Lozinka:' }).fill('student123');
  await page.getByRole('button', { name: 'Prijavi se' }).click();


  await page.getByRole('link', { name: 'Zakazi termin' }).click();
  await page.getByRole('button', { name: 'Pošalji zahtjev' }).click();
  await page.getByRole('button', { name: 'S student Student ▾' }).click();
  await page.getByRole('button', { name: 'Odjavi se' }).click();

  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('profesor');
  await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  await page.getByRole('textbox', { name: 'Lozinka:' }).fill('profesor123');
  await page.getByRole('button', { name: 'Prijavi se' }).click();


  await page.getByRole('link', { name: 'Zahtjevi studenata' }).click();
  await page.getByRole('button', { name: 'Odobri' }).click();
  await page.getByRole('button', { name: 'P profesor Profesor /' }).click();
  await page.getByRole('button', { name: 'Odjavi se' }).click();


  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('student');
  await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  await page.getByRole('textbox', { name: 'Lozinka:' }).fill('student123');
  await page.getByRole('button', { name: 'Prijavi se' }).click();

  
  await page.getByRole('link', { name: 'Moje rezervacije' }).click();
});