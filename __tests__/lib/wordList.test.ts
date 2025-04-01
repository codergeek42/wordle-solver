/*
 * wordle-solver: A clever algorithm and automated tool to solve the
 * 	NYTimes daily Wordle puzzle game.
 * Copyright (C) 2023 Peter Gordon <codergeek42@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, namely the "LICENSE" text file.  If not,
 * see <https://www.gnu.org/licenses/gpl-3.0.html>.
 */

import WordList from '../../src/lib/wordList';
import fs from 'node:fs/promises';
import { times } from 'lodash';
import {
    LetterAtPositionInWord,
    LetterAtPositionInWordRule,
    letterAtPositionInWordRuleComparator
} from '../../src/lib/letterAtPosition';
import { WordLength } from '../../__data__/alphabet';
import generateAlphabetWords from '../../src/lib/helpers/generateAlphabetWords';
import 'jest-extended';
import { MissingPositionError, NoMoreGuessesError } from '../../src/lib/wordleSolverError';

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
            const threeLetters = ['A', 'B', 'C'];

            const threeWords = generateAlphabetWords(threeLetters.join('')).sort();

            const threeWordList = new WordList(threeWords);
            expect(threeWordList).toBeDefined();
            expect(threeWordList).toBeInstanceOf(WordList);
            expect(threeWordList.words).toStrictEqual(threeWords);
            expect(threeWordList.alphabet).toStrictEqual(threeLetters);
            threeWordList.possibleLetters.forEach((possibleLetters) =>
                expect(possibleLetters).toStrictEqual(threeLetters)
            );
            expect(threeWordList.letterRules).toStrictEqual([]);
        });

        it('should uppercase, trim, and sort the given list of words', () => {
            const badWords = ['UNSORTED', 'lowercase', ' NOTTRIMMED '];
            const goodWords = badWords.map((word) => word.trim().toUpperCase()).sort();

            const goodWordList = new WordList(badWords);

            expect(goodWordList).toBeDefined();
            expect(goodWordList).toBeInstanceOf(WordList);
            expect(goodWordList.words).toStrictEqual(goodWords);
        });
        it('should create the alphabet from the given list of words', () => {
            const alphabetBuilderWords = [
                'ABCDE',
                'FGHIJ', // does it correctly use all of the words, instead of just one?
                'ABCDK', // does it correctly ignore duplicate letters across words?
                'AABLL' // does it correctly ignore duplicate letters within the same word?
            ];
            const expectedAlphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L'];

            const testWordList = new WordList(alphabetBuilderWords);

            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toStrictEqual([...alphabetBuilderWords].sort());
            expect(testWordList.alphabet).toStrictEqual(expectedAlphabet);
        });
    });

    describe(WordList.fromCopyOf, () => {
        it('can instantiate as factory method from a copy of another WordList', () => {
            const originalWordList = new WordList(['TEST', 'FROM', 'COPY', 'OF']);
            const resultCopy = WordList.fromCopyOf(originalWordList);
            expect(resultCopy).toStrictEqual(originalWordList);
            expect(resultCopy).not.toBe(originalWordList);
        });
    });

    describe(WordList.fromFile, () => {
        const wordsInFile = ['ABC', 'DEF', 'GHI', 'JK', 'LMNO'];
        const testFileName = 'test.txt';

        it('can instantiate as factory method from the lines of a text file', async () => {
            const readFileMock = jest.spyOn(fs, 'readFile').mockResolvedValueOnce(wordsInFile.join('\n'));
            const testWordList = await WordList.fromFile(testFileName);

            expect(readFileMock).toHaveBeenCalledExactlyOnceWith(testFileName, { encoding: 'utf8' });
            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toStrictEqual(wordsInFile);
        });

        it('filters for only words of the wordLength parameter if provided', async () => {
            const readFileMock = jest.spyOn(fs, 'readFile').mockResolvedValueOnce(wordsInFile.join('\n'));
            const testWordList = await WordList.fromFile(testFileName, 3);

            expect(readFileMock).toHaveBeenCalledExactlyOnceWith(testFileName, { encoding: 'utf8' });
            expect(testWordList).toBeDefined();
            expect(testWordList).toBeInstanceOf(WordList);
            expect(testWordList.words).toStrictEqual(wordsInFile.filter((word) => word.length === 3));
        });
    });

    describe(WordList.prototype.withPositionLetterRules, () => {
        it('should copy the given WordList and call processExclusionsFromRules to process the rules exclusions', () => {
            const excludeBs: LetterAtPositionInWordRule[] = [
                {
                    letter: 'B',
                    required: LetterAtPositionInWord.Impossible
                }
            ];
            // const doesWordMatchAllRulesSpy = jest
            //     .spyOn(WordList.prototype, 'doesWordMatchAllRules')
            //     .mockImplementation((word) => word === 'AAA');

            const wordList = new WordList(['AAA', 'BBB']);
            const processExclusionsFromRulesSpy = jest
                .spyOn(wordList, 'processExclusionsFromRules')
                .mockImplementationOnce(() => {
                    return;
                });
            // const wordListBeforeRules = cloneDeep(wordList);
            const wordListWithRulesApplied = wordList.withPositionLetterRules(excludeBs);

            // expect(wordList).toStrictEqual(wordListBeforeRules);
            // expect(wordList).not.toBe(wordListBeforeRules);
            expect(wordListWithRulesApplied).toStrictEqual(wordList);
            expect(wordListWithRulesApplied).not.toBe(wordList);
            expect(processExclusionsFromRulesSpy).toHaveBeenCalledExactlyOnceWith(excludeBs);
            // expect(doesWordMatchAllRulesSpy).toHaveBeenCalledTimes(wordList.words.length);
            // wordList.words.forEach((word) => {
            //     expect(doesWordMatchAllRulesSpy).toHaveBeenCalledWith(word);
            // });
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
                    expect(wordList.possibleLetters[position]).toStrictEqual(Array.from(letterPossibilities));
                });
            }
        );

        // const filterOutNonMatchingWordsTestCases: {
        //     caseName: string;
        //     words: string[];
        //     letterAtPositionInWordRule: LetterAtPositionInWordRule;
        //     expectedWords: string[];
        // }[] = [
        //     {
        //         caseName: 'should filter out letter in every position when Impossible',
        //         words: [
        //             times(WordLength, () => 'A').join(''),
        //             ...times(WordLength, (positionA) => {
        //                 const allBs = times(WordLength, () => 'A');
        //                 allBs[positionA] = 'A';
        //                 return allBs.join('');
        //             }),
        //             times(WordLength, () => 'B').join('')
        //         ],
        //         letterAtPositionInWordRule: {
        //             letter: 'A',
        //             required: LetterAtPositionInWord.Impossible
        //         },
        //         expectedWords: [times(WordLength, () => 'B').join('')]
        //     },
        //     {
        //         caseName: 'should filter out letter in one position when Impossible',
        //         words: [
        //             times(WordLength, () => 'A').join(''),
        //             ...times(WordLength, (positionA) => {
        //                 const allBs = times(WordLength, () => 'A');
        //                 allBs[positionA] = 'A';
        //                 return allBs.join('');
        //             }),
        //             times(WordLength, () => 'B').join('')
        //         ],
        //         letterAtPositionInWordRule: {
        //             letter: 'A',
        //             required: LetterAtPositionInWord.Impossible
        //         },
        //         expectedWords: [times(WordLength, () => 'B').join('')]
        //     }
        // ];
        // it.only.each(filterOutNonMatchingWordsTestCases)(
        //     '$caseName',
        //     ({ words, letterAtPositionInWordRule, expectedWords }) => {
        //         const wordList = new WordList(words);
        //         wordList.processExclusionsFromRules([letterAtPositionInWordRule]);
        //         expect(wordList.words).toStrictEqual(expectedWords);
        //     }
        // );
    });

    describe(WordList.prototype.countLetters, () => {
        it('should throw error if word list is empty', () => {
            const emptyWordList = new WordList([]);

            const testCall = () => emptyWordList.countLetters();

            expect(testCall).toThrow(NoMoreGuessesError);
        });

        it('should return the letters count when the word list is non-empty but all words are same length', () => {
            const wordList = new WordList(['AAA', 'BBA', 'ACA']);
            const expectedCounts = [{ A: 2, B: 1 }, { A: 1, B: 1, C: 1 }, { A: 3 }];

            const result = wordList.countLetters();

            expect(result).toStrictEqual(expectedCounts);
        });

        it('should filter out undefined keys from count (unexpected edge case: words are not same length', () => {
            const unevenWordList = new WordList(['A', 'AA', 'AB', 'ABC']);
            const expectedCounts = [{ A: 4 }, { A: 1, B: 2 }, { C: 1 }];

            const result = unevenWordList.countLetters();

            expect(result).toStrictEqual(expectedCounts);
        });
    });
});
