import { defineConfig } from '@playwright/test';

export default defineConfig({
    testDir: './Automation/Tests',
    use: {
        baseURL: 'http://localhost:5000/swagger/index.html',
        extraHTTPHeaders: {
            'Content-Type': 'application/json',
        },
        headless: false,

        launchOptions: {
            slowMo: 400,
        }
    },
});