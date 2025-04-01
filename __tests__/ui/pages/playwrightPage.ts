import { type Page } from 'playwright';

export class PlaywrightPage {
    readonly page: Page;

    constructor(page: Page) {
        this.page = page;
    }
}
