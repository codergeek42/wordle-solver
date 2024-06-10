import { WordList } from '../src/lib/wordList';
import fs from 'node:fs/promises';
import { filter, reject } from 'lodash';
import { LetterAtPosition } from '../src/lib/letterAtPosition';

describe(WordList.name, () => {
    beforeEach(() => {
        jest.resetAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated as empty from default constructor', () => {
            const emptyWordList = new WordList();

            expect(emptyWordList).toBeDefined();
            expect(emptyWordList).toBeInstanceOf(WordList);
            expect(emptyWordList.words).toEqual<string[]>([]);
        });

        it('can be instantiated from a list of words', () => {
            const threeWords = ['AAA', 'BBB', 'CCC'];

            const threeWordList = new WordList(threeWords);

            expect(threeWordList).toBeDefined();
            expect(threeWordList).toBeInstanceOf(WordList);
            expect(threeWordList.words).toEqual<string[]>(threeWords);
        });

        it('should uppercase, trim, and sort the given list of words', () => {
            const badWords = ['unsorted', 'lowercase', ' not trimmed '];
            const goodWords = badWords.map((word) => word.trim().toUpperCase()).toSorted();

            const goodWordList = new WordList(badWords);

            expect(goodWordList).toBeDefined();
            expect(goodWordList).toBeInstanceOf(WordList);
            expect(goodWordList.words).toEqual<string[]>(goodWords);
        });
    });

    describe(WordList.fromFile.name, () => {
        it('can instantiate as factory method from the lines of a text file', async () => {
            const wordsInFile = ['ABC', 'DEF', 'GHI'];
            const testFileName = 'test.txt';

            const readFileMock = jest
                .spyOn(fs, 'readFile')
                .mockResolvedValueOnce(wordsInFile.join('\n'));

            const testWordList = await WordList.fromFile(testFileName);

            expect(readFileMock).toHaveBeenCalledTimes(1);
            expect(readFileMock).toHaveBeenCalledWith(testFileName, {
                encoding: 'utf8'
            });
            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toEqual(wordsInFile);
        });
    });

    describe(WordList.prototype.withPositionLetterRules.name, () => {
        it('creates a new list containing only the words from the original which match the given rules', () => {
            const initialWords = [
                'APPLE',
                'APPLY',
                'BLOOD',
                'BREAD',
                'BREED',
                'BROOD',
                'CLICK',
                'CLOUD',
                'CROWD',
                'CRUDE'
            ];

            const testWordList = new WordList(initialWords);

            const excludeStartingAs = testWordList.withPositionLetterRules([
                {
                    letter: 'A',
                    position: 0,
                    required: LetterAtPosition.Impossible
                }
            ]);

            const retainStartingBs = excludeStartingAs.withPositionLetterRules([
                {
                    letter: 'B',
                    position: 0,
                    required: LetterAtPosition.Possible
                }
            ]);

            const enforceStartingCs = retainStartingBs.withPositionLetterRules([
                {
                    letter: 'C',
                    position: 0,
                    required: LetterAtPosition.Mandatory
                }
            ]);

            const withOnlyEndingDs = enforceStartingCs.withPositionLetterRules([
                {
                    letter: 'K',
                    position: 4,
                    required: LetterAtPosition.Impossible
                },
                {
                    letter: 'E',
                    position: 4,
                    required: LetterAtPosition.Possible
                },
                {
                    letter: 'D',
                    position: 4,
                    required: LetterAtPosition.Mandatory
                }
            ]);

            const enforceOnlyCrowdRemaining = withOnlyEndingDs.withPositionLetterRules([
                {
                    letter: 'O',
                    position: 2,
                    required: LetterAtPosition.Mandatory
                },
                {
                    letter: 'L',
                    position: 1,
                    required: LetterAtPosition.Impossible
                }
            ]);

            expect(testWordList.words).toEqual(initialWords);
            expect(excludeStartingAs.words).toEqual(
                reject(initialWords, (word) => word.startsWith('A'))
            );
            expect(retainStartingBs.words).toEqual(excludeStartingAs.words);
            expect(enforceStartingCs.words).toEqual(
                filter(retainStartingBs.words, (word) => word.startsWith('C'))
            );
            expect(withOnlyEndingDs.words).toEqual(
                reject(enforceStartingCs.words, (word) => word.endsWith('K')).filter((word) =>
                    word.endsWith('D')
                )
            );
            expect(enforceOnlyCrowdRemaining.words).toEqual(['CROWD']);
        });
    });
});
