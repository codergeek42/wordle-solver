import {
    MissingPositionError,
    NoMoreGuessesError,
    WordleSolverError,
    WordleSolverTestError
} from '../../src/lib/wordleSolverError';

const wordleSolverErrorTestCases: {
    ErrorType: typeof WordleSolverError;
    ExtendsType: typeof Error | typeof WordleSolverError;
}[] = [
    {
        ErrorType: WordleSolverError,
        ExtendsType: Error
    },
    ...[MissingPositionError, NoMoreGuessesError, WordleSolverTestError].map((ErrorType) => ({
        ErrorType,
        ExtendsType: WordleSolverError
    }))
];

it.each(wordleSolverErrorTestCases)(
    '$ErrorType.name can be instantiated and extends $ExtendsType.name class',
    ({ ErrorType, ExtendsType }) => {
        const wordleSolverError = new ErrorType('test');

        expect(wordleSolverError).toBeDefined();
        expect(wordleSolverError).toBeInstanceOf(ErrorType);
        expect(wordleSolverError).toBeInstanceOf(ExtendsType);
    }
);
