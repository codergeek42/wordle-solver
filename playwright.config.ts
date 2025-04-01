import { defineConfig, devices } from 'playwright/test';

export default defineConfig({
    testDir: '__tests__/ui',

    forbidOnly: true,

    // TODO: For >6 attempts, cache guesses + result in file and use Playwright retries to get answer.
    retries: 0,

    // TODO: use parallel runners for guesser algorithm comparison?
    fullyParallel: false,
    workers: 1,

    reporter: 'list',

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
