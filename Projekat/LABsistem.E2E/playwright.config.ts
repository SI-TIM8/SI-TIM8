import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  // Mapa gdje će se nalaziti naši testovi
  testDir: './tests',
  
  // Maksimalno vrijeme trajanja jednog testa (30 sekundi)
  timeout: 30 * 1000,
  
  // Očekivanja (assertions) imaju timeout od 5 sekundi
  expect: {
    timeout: 5000
  },

  // Pokreni testove paralelno ako ih ima više
  fullyParallel: true,

  // Reporteri - 'html' će ti otvoriti super izvještaj u browseru ako test padne
  reporter: 'html',

  // Zajedničke postavke za sve browsere
  use: {
    baseURL: 'https://labsistem.duckdns.org',
    
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'on-first-retry',
  },

  
  projects: [

    {
      name: 'setup',
      testMatch: /.*\.setup\.ts/,
    },

    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
      dependencies: ['setup'],
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
      dependencies: ['setup'],
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
      dependencies: ['setup'],
    },
  ],
});