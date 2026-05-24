import { defineConfig } from "cypress";

export default defineConfig({
  allowCypressEnv: false,

  e2e: {
    baseUrl: 'https://labsistem.serveblog.net',
    setupNodeEvents(on, config) {
      
      // implement node event listeners here
    },
  },
});
