import { test } from '@playwright/test';

const studentUsername = process.env.PLAYWRIGHT_STUDENT_USERNAME || 'student';
const studentPassword = process.env.PLAYWRIGHT_STUDENT_PASSWORD || 'student123';
const professorUsername = process.env.PLAYWRIGHT_PROFESSOR_USERNAME || 'profesor';
const professorPassword = process.env.PLAYWRIGHT_PROFESSOR_PASSWORD || 'profesor123';

test('Student salje zahtjev za rezervaciju i profesor ga odobrava', async ({ page }) => {
  await page.goto('/login');
  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).fill(studentUsername);
  await page.getByLabel('Lozinka:').click();
  await page.getByLabel('Lozinka:').fill(studentPassword);
  await page.getByRole('button', { name: 'Prijavi se' }).click();

  await page.getByRole('link', { name: 'Zakazi termin' }).click();
  await page.getByRole('button', { name: 'Posalji zahtjev' }).click();
  await page.getByRole('button', { name: /student/i }).click();
  await page.getByRole('button', { name: 'Odjavi se' }).click();

  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).fill(professorUsername);
  await page.getByLabel('Lozinka:').click();
  await page.getByLabel('Lozinka:').fill(professorPassword);
  await page.getByRole('button', { name: 'Prijavi se' }).click();

  await page.getByRole('link', { name: 'Zahtjevi studenata' }).click();
  await page.getByRole('button', { name: 'Odobri' }).click();
  await page.getByRole('button', { name: /profesor/i }).click();
  await page.getByRole('button', { name: 'Odjavi se' }).click();

  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).click();
  await page.getByRole('textbox', { name: 'Korisnicko ime ili email' }).fill(studentUsername);
  await page.getByLabel('Lozinka:').click();
  await page.getByLabel('Lozinka:').fill(studentPassword);
  await page.getByRole('button', { name: 'Prijavi se' }).click();

  await page.getByRole('link', { name: 'Moje rezervacije' }).click();
});
