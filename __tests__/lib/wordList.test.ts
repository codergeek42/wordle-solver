import WordList from '../../src/lib/wordList';
import fs from 'node:fs/promises';
import { cloneDeep, times } from 'lodash';
import {
    LetterAtPositionInWord,
    LetterAtPositionInWordRule,
    letterAtPositionInWordRuleComparator
} from '../../src/lib/letterAtPosition';
import { WordLength } from '../../__data__/alphabet';
import generateAlphabetWords from '../../src/lib/helpers/generateAlphabetWords';
import 'jest-extended';
import { MissingPositionError } from '../../src/lib/wordleSolverError';

describe(WordList, () => {
    beforeEach(() => {
        jest.resetAllMocks();
        jest.restoreAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated as empty from default constructor', () => {
            const emptyWordList = new WordList();

            expect(emptyWordList).toBeDefined();
            expect(emptyWordList).toBeInstanceOf(WordList);
            expect(emptyWordList.words).toBeEmpty();
            expect(emptyWordList.alphabet).toBeEmpty();
        });

        it('can be instantiated from a list of words', () => {
            const threeWords = ['AAAA', 'BBB', 'CCC'];

            const threeWordList = new WordList(threeWords);

            expect(threeWordList).toBeDefined();
            expect(threeWordList).toBeInstanceOf(WordList);
            expect(threeWordList.words).toStrictEqual(threeWords);
            expect(threeWordList.alphabet).toStrictEqual(new Set(['A', 'B', 'C']));
        });

        it('should uppercase, trim, and sort the given list of words', () => {
            const badWords = ['UNSORTED', 'lowercase', ' NOTTRIMMED '];
            const goodWords = badWords.map((word) => word.trim().toUpperCase()).sort();

            const goodWordList = new WordList(badWords);

            expect(goodWordList).toBeDefined();
            expect(goodWordList).toBeInstanceOf(WordList);
            expect(goodWordList.words).toStrictEqual(goodWords);
            expect(goodWordList.alphabet).toStrictEqual(new Set(Array.from('UNSORTEDLOWERCASENOTTRIMMED').sort()));
        });
        it('should create the alphabet from the given list of words', () => {
            const alphabetBuilderWords = [
                'ABCDE',
                'FGHIJ', // does it correctly use all of the words, instead of just one?
                'ABCDK', // does it correctly ignore duplicate letters across words?
                'AABLL' // does it correctly ignore duplicate letters within the same word?
            ];
            const expectedAlphabet = new Set(['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L']);

            const testWordList = new WordList(alphabetBuilderWords);

            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toStrictEqual([...alphabetBuilderWords].sort());
            expect(testWordList.alphabet).toStrictEqual(expectedAlphabet);
        });
    });

    describe(WordList.fromFile, () => {
        it('can instantiate as factory method from the lines of a text file', async () => {
            const wordsInFile = ['ABC', 'DEF', 'GHI'];
            const testFileName = 'test.txt';

            const readFileMock = jest.spyOn(fs, 'readFile').mockResolvedValueOnce(wordsInFile.join('\n'));

            const testWordList = await WordList.fromFile(testFileName);

            expect(readFileMock).toHaveBeenCalledExactlyOnceWith(testFileName, { encoding: 'utf8' });
            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toStrictEqual(wordsInFile);
        });
    });

    describe(WordList.prototype.withPositionLetterRules, () => {
        it('should copy the given WordList and process the rules exclusions and filter out the non-matching words', () => {
            const excludeBs: LetterAtPositionInWordRule[] = [
                {
                    letter: 'B',
                    required: LetterAtPositionInWord.Impossible
                }
            ];
            const processExclusionsFromRulesSpy = jest
                .spyOn(WordList.prototype, 'processExclusionsFromRules')
                .mockImplementationOnce(() => {
                    return;
                });
            const doesWordMatchAllRulesSpy = jest
                .spyOn(WordList.prototype, 'doesWordMatchAllRules')
                .mockImplementation((word) => word === 'AAA');

            const wordList = new WordList(['AAA', 'BBB']);
            const wordListBeforeRules = cloneDeep(wordList);
            const wordListWithRulesApplied = wordList.withPositionLetterRules(excludeBs);

            expect(wordList).toStrictEqual(wordListBeforeRules);
            expect(wordListWithRulesApplied.words).toStrictEqual(['AAA']);
            expect(processExclusionsFromRulesSpy).toHaveBeenCalledExactlyOnceWith(excludeBs);
            expect(doesWordMatchAllRulesSpy).toHaveBeenCalledTimes(wordList.words.length);
            wordList.words.forEach((word) => {
                expect(doesWordMatchAllRulesSpy).toHaveBeenCalledWith(word);
            });
        });
    });

    describe(WordList.prototype.doesWordMatchAllRules, () => {
        const testCases: {
            caseName: string;
            alphabet: string;
            positionLetterRules: LetterAtPositionInWordRule[];
            expectedWordMatches: { word: string; isMatch: boolean }[];
        }[] = [
            {
                caseName: 'excludes A at every position (Impossible)',
                alphabet: 'AB',
                positionLetterRules: [
                    {
                        letter: 'A',
                        required: LetterAtPositionInWord.Impossible
                    }
                ],
                expectedWordMatches: [
                    { word: 'A'.repeat(WordLength), isMatch: false },
                    ...times(WordLength, (idx) => 'B'.repeat(idx) + 'A' + 'B'.repeat(WordLength - idx)).map((word) => ({
                        word,
                        isMatch: false
                    })),
                    { word: 'B'.repeat(WordLength), isMatch: true }
                ]
            },
            {
                caseName: 'requires B at every position (Mandatory)',
                alphabet: 'AB',
                positionLetterRules: times(WordLength, (idx) => ({
                    position: idx,
                    letter: 'B',
                    required: LetterAtPositionInWord.Mandatory
                })),
                expectedWordMatches: [
                    {
                        word: 'A'.repeat(WordLength),
                        isMatch: false
                    },
                    ...times(WordLength, (idx) => 'B'.repeat(idx) + 'A' + 'B'.repeat(WordLength - idx)).map((word) => ({
                        word,
                        isMatch: false
                    })),
                    {
                        word: 'B'.repeat(WordLength),
                        isMatch: true
                    }
                ]
            },
            {
                caseName: 'requires C at any position except 0 (Misplaced)',
                alphabet: 'ABC',
                positionLetterRules: [
                    {
                        letter: 'C',
                        required: LetterAtPositionInWord.Misplaced,
                        position: 0
                    }
                ],
                expectedWordMatches: [
                    {
                        word: 'C'.repeat(WordLength),
                        isMatch: false // because C cannot be position 0
                    },
                    {
                        word: 'B'.repeat(WordLength),
                        isMatch: false // because the word must contain at least one C
                    },
                    {
                        word: 'B' + 'C'.repeat(WordLength - 1),
                        isMatch: true
                    }
                ]
            }
        ];
        it.each(testCases)('$caseName', ({ alphabet, positionLetterRules, expectedWordMatches }) => {
            const wordList = new WordList(generateAlphabetWords(alphabet));
            wordList.processExclusionsFromRules(positionLetterRules);
            expectedWordMatches.forEach(({ word, isMatch }) => {
                expect(wordList.doesWordMatchAllRules(word)).toBe(isMatch);
            });
        });
    });

    describe(WordList.prototype.processExclusionsFromRules, () => {
        it('should sort the rules used, with Mandatory last', () => {
            const testRules: LetterAtPositionInWordRule[] = [
                {
                    letter: 'A',
                    position: 0,
                    required: LetterAtPositionInWord.Mandatory
                },
                {
                    letter: 'B',
                    position: 0,
                    required: LetterAtPositionInWord.Misplaced
                },
                {
                    letter: 'C',
                    required: LetterAtPositionInWord.Impossible
                }
            ];
            const arraySortSpy = jest.spyOn(Array.prototype, 'sort');
            const wordList = new WordList(generateAlphabetWords('ABC'));

            expect(() => wordList.processExclusionsFromRules(testRules)).not.toThrow();
            expect(arraySortSpy).toHaveBeenLastCalledWith(letterAtPositionInWordRuleComparator);
            expect(wordList.letterRules[testRules.length - 1].required).toStrictEqual(LetterAtPositionInWord.Mandatory);
        });

        it.each([LetterAtPositionInWord.Mandatory, LetterAtPositionInWord.Misplaced])(
            'should throw a MissingPositionError when non-Impossible rule has no position (%s)',
            (ruleRequired: LetterAtPositionInWord) => {
                const testRuleWithoutPosition: Exclude<LetterAtPositionInWordRule, 'position'> = {
                    letter: 'A',
                    required: ruleRequired
                };
                const wordList = new WordList(generateAlphabetWords('AB'));

                expect(() => wordList.processExclusionsFromRules([testRuleWithoutPosition])).toThrow(
                    new MissingPositionError(JSON.stringify(testRuleWithoutPosition))
                );
            }
        );
        it('should not throw a MissingPositionError when Impossible rule has no position', () => {
            const testRuleWithoutPosition: Exclude<LetterAtPositionInWordRule, 'position'> = {
                letter: 'A',
                required: LetterAtPositionInWord.Impossible
            };
            const wordList = new WordList(generateAlphabetWords('AB'));

            expect(() => wordList.processExclusionsFromRules([testRuleWithoutPosition])).not.toThrow();
        });

        const letterAtPositionTestCases: {
            caseName: string;
            alphabet: string;
            positionLetterRules: LetterAtPositionInWordRule[];
            expectedLetterPossibilities: string[];
        }[] = [
            {
                caseName: 'should set singleton letter for Mandatory',
                alphabet: 'ABC',
                positionLetterRules: [
                    {
                        required: LetterAtPositionInWord.Mandatory,
                        letter: 'A',
                        position: 0
                    }
                ],
                expectedLetterPossibilities: ['A', ...times(WordLength - 1, () => 'ABC')]
            },
            {
                caseName: 'should remove individual letters for Misplaced',
                alphabet: 'ABC',
                positionLetterRules: [
                    {
                        required: LetterAtPositionInWord.Misplaced,
                        letter: 'A',
                        position: 0
                    },
                    {
                        required: LetterAtPositionInWord.Misplaced,
                        letter: 'B',
                        position: 1
                    }
                ],
                expectedLetterPossibilities: ['BC', 'AC', ...times(WordLength - 2, () => 'ABC')]
            },
            {
                caseName: 'should remove letters from entire word for Impossible',
                alphabet: 'ABC',
                positionLetterRules: [
                    {
                        required: LetterAtPositionInWord.Impossible,
                        letter: 'A'
                    }
                ],
                expectedLetterPossibilities: times(WordLength, () => 'BC')
            },
            {
                caseName:
                    'should account for when Misplaced letter is later found (Mandatory there, Impossible otherwise)',
                alphabet: 'ABC',
                positionLetterRules: [
                    {
                        required: LetterAtPositionInWord.Misplaced,
                        letter: 'A',
                        position: 1
                    },
                    {
                        required: LetterAtPositionInWord.Mandatory,
                        letter: 'A',
                        position: 0
                    },
                    {
                        required: LetterAtPositionInWord.Impossible,
                        letter: 'A'
                    }
                ],
                expectedLetterPossibilities: ['A', ...times(WordLength - 1, () => 'BC')]
            }
        ];
        it.each(letterAtPositionTestCases)(
            '$caseName',
            ({ alphabet, positionLetterRules, expectedLetterPossibilities }) => {
                const wordList = new WordList(generateAlphabetWords(alphabet));
                wordList.processExclusionsFromRules(positionLetterRules);
                expectedLetterPossibilities.forEach((letterPossibilities, position) => {
                    expect(wordList.possibleLetters[position]).toStrictEqual(new Set(Array.from(letterPossibilities)));
                });
            }
        );
        // const testCases: Array<{
        //     caseName: string;
        //     alphabet: string;
        //     positionLetterRules: Array<LetterAtPositionInWordRule>;
        //     expectedLettersAtPositions: {
        //         missing?: Array<Set<string>>;
        //         required?: Array<Set<string>>;
        //     };
        // }> = [
        //     {
        //         caseName: 'excludes A at every position (Impossible)',
        //         alphabet: 'AB',
        //         positionLetterRules: [
        //             {
        //                 letter: 'A',
        //                 required: LetterAtPositionInWord.Impossible
        //             }
        //         ],
        //         expectedLettersAtPositions: {
        //             missing: times(WordLength, () => new Set(['A']))
        //         }
        //     },
        //     {
        //         caseName: 'requires B at every position (Mandatory)',
        //         alphabet: 'AB',
        //         positionLetterRules: times(WordLength, (idx) => ({
        //             position: idx,
        //             letter: 'B',
        //             required: LetterAtPositionInWord.Mandatory
        //         })),
        //         expectedLettersAtPositions: {
        //             required: times(WordLength, () => new Set(['B']))
        //         }
        //     },
        //     {
        //         caseName: 'misplaces A at every position (Misplaced)',
        //         alphabet: 'AB',
        //         positionLetterRules: times(WordLength, (idx) => ({
        //             position: idx,
        //             letter: 'B',
        //             required: LetterAtPositionInWord.Misplaced
        //         })),
        //         expectedLettersAtPositions: {
        //             missing: times(WordLength, () => new Set(['B']))
        //         }
        //     },
        //     {
        //         caseName:
        //             'misplaces B at all but the last position (Misplaced), then requires it there (Mandatory/Impossible)',
        //         alphabet: 'AB',
        //         positionLetterRules: [
        //             ...times(WordLength - 1, (idx) => ({
        //                 position: idx,
        //                 letter: 'B',
        //                 required: LetterAtPositionInWord.Misplaced
        //             })),
        //             {
        //                 position: WordLength - 1,
        //                 letter: 'B',
        //                 required: LetterAtPositionInWord.Mandatory
        //             }
        //         ],
        //         expectedLettersAtPositions: {
        //             required: [...times(WordLength - 1, () => new Set(['A'])), new Set(['B'])],
        //             missing: [...times(WordLength - 1, () => new Set(['B'])), new Set(['A'])]
        //         }
        //     }
        // ];
        // it.each(testCases)('$caseName', ({ alphabet, positionLetterRules, expectedLettersAtPositions }) => {
        //     const wordList = new WordList(generateAlphabetWords(alphabet));
        //     wordList.processExclusionsFromRules(positionLetterRules);
        //     expect(wordList.letterRules).toStrictEqual(positionLetterRules);
        //     // TODO: When upgrading to Node 22+, switch to built-in Set intersection functionality instead of using
        //     // Lodash's ArrayLike methods.
        //     expectedLettersAtPositions.missing?.forEach((missingLetters, position) => {
        //         expect(
        //             intersection(Array.from(wordList.possibleLetters[position]), Array.from(missingLetters))
        //         ).toStrictEqual([]);
        //     });
        //     expectedLettersAtPositions.required?.forEach((requiredLetters, position) => {
        //         expect(
        //             intersection(Array.from(wordList.possibleLetters[position]), Array.from(requiredLetters))
        //         ).toStrictEqual(Array.from(requiredLetters));
        //     });
        // });
    });
});
