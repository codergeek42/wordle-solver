import { type Locator, type Page } from 'playwright';
import { PlaywrightPage } from './playwrightPage';

export class WordleIndexPage extends PlaywrightPage {
    readonly baseUrl = 'https://www.nytimes.com/games/wordle/index.html';

    readonly howToPlayCloseModalButton: Locator;
    readonly playButton: Locator;
    readonly updatedTermsModalButton: Locator;

    constructor(page: Page) {
        super(page);
        this.howToPlayCloseModalButton = this.page.locator('div', { hasText: 'How To Play' }).getByLabel('Close');
        this.playButton = this.page.locator('button', { hasText: 'Play' });
        this.updatedTermsModalButton = this.page
            .locator('div.purr-blocker-card', { hasText: 'We’ve updated our terms' })
            .locator('button', { hasText: 'Continue' });
    }

    async openAndClickThroughModals() {
        await this.page.goto(this.baseUrl);
        await this.updatedTermsModalButton.click();
        await this.playButton.click();
        await this.howToPlayCloseModalButton.click();
    }
}
