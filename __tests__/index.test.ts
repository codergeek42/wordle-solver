import mockConsole from 'jest-mock-console';
import { helloWorld, goodByeWorld } from '../src/index';

describe('index', () => {
    let restoreConsole: ReturnType<typeof mockConsole>;

    beforeEach(() => {
        restoreConsole = mockConsole();
    });

    afterEach(() => {
        restoreConsole();
    });

    describe('index', () => {
        it.each([
            {
                caseName: 'helloWorld',
                method: helloWorld,
                expectedOutput: 'Hello, World!'
            },
            {
                caseName: 'goodByeWorld',
                method: goodByeWorld,
                expectedOutput: 'Goodbye, World! ...'
            }
        ])('$caseName', async ({ method, expectedOutput }) => {
            const methodCall = method();

            expect(methodCall).resolves.not.toThrow();
            expect(console.log).toHaveBeenCalledTimes(1);
            expect(console.log).toHaveBeenCalledWith(expectedOutput);
        });
    });
});
