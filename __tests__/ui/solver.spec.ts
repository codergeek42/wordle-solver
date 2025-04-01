import { test } from 'playwright/test';
import { WordleIndexPage } from './pages/wordleIndex.page';

test('Wordle Solver', async ({ page }) => {
    const wordleIndexPage = new WordleIndexPage(page);

    await wordleIndexPage.openAndClickThroughModals();
});
