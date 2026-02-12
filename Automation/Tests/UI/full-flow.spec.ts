//npx playwright test --headed -> use to run all tests in terminal
//npx playwright test --headed -g "Full Flow via Swagger UI" -> use to run only this specific test 

import { test, expect } from '@playwright/test';
import fs from 'fs';

test('Full Flow via Swagger UI', async ({ page }) => {

    const longUrl = 'https://github.com/sxncz/tiLink/tree/main';

    // 1. Open Swagger
    await page.goto('http://localhost:5000/swagger/index.html');
    await page.waitForTimeout(3000);

    // 2. Wait for Swagger root
    await page.waitForSelector('#swagger-ui');

    // 3. Wait until at least one operation is rendered
    const operation = page.locator('.opblock', {
        has: page.locator('.opblock-summary-path', { hasText: '/shorten' })
    });
    await expect(operation).toBeVisible();

    // 4. Expand the operation
    await operation.locator('.opblock-summary').click();

    // 5. Wait for the body (this is CRITICAL)
    const opBody = operation.locator('.opblock-body');
    await expect(opBody).toBeVisible();

    // 6. Click "Try it out"
    await opBody.getByRole('button', { name: 'Try it out' }).click();

    // 7. Fill request body (Swagger uses textarea)
    await opBody.locator('textarea').fill(
        JSON.stringify(
            { longUrl: longUrl },
            null,
            2
        )
    );
    await page.waitForTimeout(3000);

    // 8. Execute
    await opBody.getByRole('button', { name: 'Execute' }).click();
    await page.waitForTimeout(3000);

    // 9. Assert response appears
    const responseBlock = opBody.locator('.responses-wrapper');
    await expect(responseBlock).toBeVisible();
    await page.waitForTimeout(3000);

    //10. Extract Shortened Link
    const responseText = await responseBlock
        .locator('.response-col_description')
        .locator('pre')
        .first()
        .innerText();
    const responseJson = JSON.parse(responseText);

    const shortUrl = responseJson.shortUrl;

    //11. Open shortened link
    await page.goto(shortUrl);
    await page.waitForTimeout(3000);

    //12. Click on Visit Site if it appears
    await page.waitForLoadState('domcontentloaded');

    const visitElement = page.locator('text=Visit Site');

    if (await visitElement.first().isVisible().catch(() => false)) {
        await visitElement.first().click();
    }

    //13. Validate redirect
    await expect(page.url()).toContain(longUrl);
    await page.waitForTimeout(3000);


    //14. Open xml file and check data
    await page.goto('file:///C:/tiLink/backend/Export/Test.xml'); //might fail here due to path not being the same on all machines
    const xmlContent = await page.content();
    await page.waitForTimeout(3000);

});