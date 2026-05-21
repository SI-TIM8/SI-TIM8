import { test, expect } from '@playwright/test';

const professorUsername = process.env.PLAYWRIGHT_PROFESSOR_USERNAME || 'profesor';
const professorPassword = process.env.PLAYWRIGHT_PROFESSOR_PASSWORD || 'profesor123';

test.describe('Rezervacijski tok - profesor', () => {
  test('Profesor moze rezervirati, otkazati i kreirati skrivenu rezervaciju', async ({ page }) => {
    await page.goto('/login');
    await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).fill(professorUsername);
    await page.getByLabel('Lozinka:').fill(professorPassword);
    await page.getByRole('button', { name: 'Prijavi se' }).click();

    await expect(page.getByRole('link', { name: 'Termini' })).toBeVisible({ timeout: 10000 });
    await page.getByRole('link', { name: 'Termini' }).click();

    await expect(page.getByRole('button', { name: 'Rezervisi' }).first()).toBeVisible();
    await page.getByRole('button', { name: 'Rezervisi' }).first().click();
    await page.getByRole('button', { name: 'Potvrdi rezervaciju' }).click();

    await page.goto('/rezervacije');

    page.once('dialog', (dialog) => {
      dialog.accept().catch(() => {});
    });
    await page.getByRole('button', { name: 'Otkazi' }).first().click();

    await page.goto('/termini');

    await expect(page.getByRole('button', { name: 'Rezervisi' }).nth(1)).toBeVisible();
    await page.getByRole('button', { name: 'Rezervisi' }).nth(1).click();
    await page.getByRole('checkbox', { name: 'Vidljivo studentima' }).uncheck();
    await page.getByRole('button', { name: 'Potvrdi rezervaciju' }).click();

    await page.goto('/rezervacije');
  });
});
