//npx playwright test --headed -> use to run tests in terminal

import { test, expect } from '@playwright/test';

test('API is reachable', async ({ request }) => {
    const response = await request.get('http://localhost:5000/swagger/index.html');
    expect(response.ok()).toBeTruthy();
});