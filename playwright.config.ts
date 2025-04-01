import { defineConfig, devices } from 'playwright/test';

export default defineConfig({
    testDir: '__tests__/ui',

    forbidOnly: true,

    // TODO: For >6 attempts, cache guesses + result in file and use Playwright retries to get answer.
    retries: 0,

    reporter: 'json',

    use: {
        baseURL: 'https://www.nytimes.com/games/wordle',
        trace: 'on'
    },
    projects: [
        {
            name: 'firefox',
            use: { ...devices['Desktop Firefox'] }
        }
    ]
});
