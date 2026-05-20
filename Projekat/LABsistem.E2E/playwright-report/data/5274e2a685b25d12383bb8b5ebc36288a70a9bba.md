# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: rezervacija-student.spec.ts >> test
- Location: tests\rezervacija-student.spec.ts:3:5

# Error details

```
Test timeout of 30000ms exceeded.
```

```
Error: locator.click: Test timeout of 30000ms exceeded.
Call log:
  - waiting for getByRole('button', { name: 'Pošalji zahtjev' })

```

# Page snapshot

```yaml
- generic [ref=e3]:
  - complementary [ref=e4]:
    - link "LABsistem" [ref=e5] [cursor=pointer]:
      - /url: /dashboard
      - generic [ref=e6]: LABsistem
    - navigation [ref=e7]:
      - generic [ref=e8]: Student
      - link "Kalendar termina" [ref=e9] [cursor=pointer]:
        - /url: /kalendar
      - link "Zakazi termin" [ref=e10] [cursor=pointer]:
        - /url: /zakazivanje
      - link "Moje rezervacije" [ref=e11] [cursor=pointer]:
        - /url: /rezervacije
  - generic [ref=e12]:
    - banner [ref=e13]:
      - button "S student Student ▾" [ref=e15] [cursor=pointer]:
        - generic [ref=e16]: S
        - generic [ref=e17]:
          - strong [ref=e18]: student
          - generic [ref=e19]: Student
        - generic [ref=e20]: ▾
    - main [ref=e21]:
      - generic [ref=e22]:
        - heading "Zakazi termin" [level=1] [ref=e23]
        - paragraph [ref=e24]: Prijavite se na termine koje su profesori ucinili dostupnim.
      - generic [ref=e25]:
        - generic [ref=e26]:
          - generic [ref=e27]: Datum
          - generic [ref=e28]: Vrijeme
          - generic [ref=e29]: Kabinet
          - generic [ref=e30]: Profesor
          - generic [ref=e31]: Popunjenost
          - generic [ref=e32]: Vidljivost
          - generic [ref=e33]: Akcija
        - generic [ref=e34]: Trenutno nema dostupnih termina za prijavu.
```

# Test source

```ts
  1  | import { test, expect } from '@playwright/test';
  2  | 
  3  | test('test', async ({ page }) => {
  4  |   await page.goto('https://labsistem.duckdns.org/login');
  5  |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  6  |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('student');
  7  |   await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  8  |   await page.getByRole('textbox', { name: 'Lozinka:' }).fill('student123');
  9  |   await page.getByRole('button', { name: 'Prijavi se' }).click();
  10 |   await page.getByRole('link', { name: 'Zakazi termin' }).click();
> 11 |   await page.getByRole('button', { name: 'Pošalji zahtjev' }).click();
     |                                                               ^ Error: locator.click: Test timeout of 30000ms exceeded.
  12 |   await page.getByRole('button', { name: 'S student Student ▾' }).click();
  13 |   await page.getByRole('button', { name: 'Odjavi se' }).click();
  14 |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  15 |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('profesor');
  16 |   await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  17 |   await page.getByRole('textbox', { name: 'Lozinka:' }).fill('profesor123');
  18 |   await page.getByRole('button', { name: 'Prijavi se' }).click();
  19 |   await page.getByRole('link', { name: 'Zahtjevi studenata' }).click();
  20 |   await page.getByRole('button', { name: 'Odobri' }).click();
  21 |   await page.getByRole('button', { name: 'P profesor Profesor /' }).click();
  22 |   await page.getByRole('button', { name: 'Odjavi se' }).click();
  23 |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).click();
  24 |   await page.getByRole('textbox', { name: 'Korisničko ime ili email' }).fill('student');
  25 |   await page.getByRole('textbox', { name: 'Lozinka:' }).click();
  26 |   await page.getByRole('textbox', { name: 'Lozinka:' }).fill('student123');
  27 |   await page.getByRole('button', { name: 'Prijavi se' }).click();
  28 |   await page.getByRole('link', { name: 'Moje rezervacije' }).click();
  29 | });
```