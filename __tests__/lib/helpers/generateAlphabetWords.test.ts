import { times } from 'lodash';
import { WordLength } from '../../../__data__/alphabet';
import generateAlphabetWords from '../../../src/lib/helpers/generateAlphabetWords';

describe(generateAlphabetWords, () => {
    const alphabetWord = (length: number, chunkNum = 0) =>
        String.fromCodePoint(...times(length, (ord) => 'A'.charCodeAt(0) + chunkNum * WordLength + ord));

    const testCases: Array<{
        caseName: string;
        alphabet: string;
        expectedWords: Array<string>;
    }> = [
        {
            caseName: 'should pad the given alphabet to WordLength length',
            alphabet: alphabetWord(1),
            expectedWords: [alphabetWord(1).repeat(WordLength)]
        },
        {
            caseName: 'should split a word larger than WordLength into words of that length',
            alphabet: alphabetWord(2 * WordLength),
            expectedWords: times(2, (chunkNum) => alphabetWord(WordLength, chunkNum))
        },
        {
            caseName: 'should return the given alphabet as a singleton string if it has exactly WordLength length',
            alphabet: alphabetWord(WordLength),
            expectedWords: [alphabetWord(WordLength)]
        }
    ];
    it.each(testCases)('$caseName', ({ alphabet, expectedWords }) => {
        expect(generateAlphabetWords(alphabet)).toStrictEqual(expectedWords);
    });
});
