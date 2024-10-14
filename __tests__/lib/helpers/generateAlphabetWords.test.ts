import { WordLength } from '../../../__data__/alphabet';
import * as generateAlphabetWordsModule from '../../../src/lib/helpers/generateAlphabetWords';
import { WordleSolverTestError } from '../../../src/lib/wordleSolverError';

const { default: generateAlphabetWords } = generateAlphabetWordsModule;

describe(generateAlphabetWords, () => {
    it('should generate all possible words from the given alphabet', () => {
        const alphabet = 'ABC';
        const expectedAlphabetWords = ['AA', 'BA', 'CA', 'AB', 'BB', 'CB', 'AC', 'BC', 'CC'];
        const result = generateAlphabetWords(alphabet, 2);
        expect(result).toStrictEqual(expectedAlphabetWords);
    });

    it('should throw a WorldSolverTestError if the size argument is negative', () => {
        const alphabet = 'ABC';
        const testCall = () => generateAlphabetWords(alphabet, -1);
        expect(testCall).toThrow(WordleSolverTestError);
    });

    it('should use WordLength as the default wordSize parameter', () => {
        const alphabet = 'ABC';
        const result = generateAlphabetWords(alphabet);
        const expected = generateAlphabetWords(alphabet, WordLength);
        expect(result).toStrictEqual(expected);
    });
});
