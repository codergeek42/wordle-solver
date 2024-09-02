import { MissingPositionError, WordleSolverError } from '../../src/lib/wordleSolverError';

describe(WordleSolverError, () => {
    it('can be instantiated and extends Error class', () => {
        const wordleSolverError = new WordleSolverError('test');

        expect(wordleSolverError).toBeDefined();
        expect(wordleSolverError).toBeInstanceOf(WordleSolverError);
        expect(wordleSolverError).toBeInstanceOf(Error);
    });
});

describe(MissingPositionError, () => {
    it('can be instantiated and extends WordleSolverError class', () => {
        const missingPositionError = new MissingPositionError('test');

        expect(missingPositionError).toBeDefined();
        expect(missingPositionError).toBeInstanceOf(MissingPositionError);
        expect(missingPositionError).toBeInstanceOf(WordleSolverError);
    });
});
