import { defineConfig } from '@playwright/test';

export default defineConfig({
    testDir: './tests',
    use: {
        baseURL: 'http://localhost:5000/swagger/index.html',
        extraHTTPHeaders: {
            'Content-Type': 'application/json',
        },
    },
});