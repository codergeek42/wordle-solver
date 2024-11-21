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

import { range, times } from 'lodash';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';
import generateAlphabetWords from '../../../src/lib/helpers/generateAlphabetWords';
import RetryMisplacedLettersStrategy from '../../../src/lib/guesserStrategies/retryMisplacedLettersStrategy';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';
import { LetterAtPositionInWord, LetterAtPositionInWordRule } from '../../../src/lib/letterAtPosition';
import 'jest-extended';
import { WordLength } from '../../../__data__/alphabet';

describe(RetryMisplacedLettersStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList(['TEST']);
            const retryMisplacedLettersStrategy = new RetryMisplacedLettersStrategy(wordList);

            expect(retryMisplacedLettersStrategy).toBeDefined();
            expect(retryMisplacedLettersStrategy.wordList).toBeInstanceOf(WordList);
            expect(retryMisplacedLettersStrategy.scoreForGuess).toBeFunction();
            expect(retryMisplacedLettersStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(retryMisplacedLettersStrategy).toBeInstanceOf(RetryMisplacedLettersStrategy);
        });
    });

    describe(RetryMisplacedLettersStrategy.prototype.scoreForGuess, () => {
        const numMisplacedLettersCases = range(0, WordLength + 1);
        it.each(numMisplacedLettersCases)(
            'scores the guess based on the number of previously misplaced letters (%i misplaced)',
            (numMisplacedLetters) => {
                const alphabet = generateAlphabetOfLength(numMisplacedLetters + 3);
                const misplacedAlphabet = alphabet.slice(-1).concat(alphabet.slice(0, alphabet.length - 1));
                const wordList = new WordList(generateAlphabetWords(alphabet, 2));
                const retryMisplacedLettersStrategy = new RetryMisplacedLettersStrategy(wordList);

                const impossibleLetterRule: LetterAtPositionInWordRule = {
                    required: LetterAtPositionInWord.Impossible,
                    letter: alphabet.slice(-3, -2)
                };

                const nonmatchingMisplacedLetterRule: LetterAtPositionInWordRule = {
                    required: LetterAtPositionInWord.Misplaced,
                    letter: alphabet.slice(-2, -1),
                    position: numMisplacedLetters + 1
                };

                const previouslyMisplacedLetterRules: LetterAtPositionInWordRule[] = times(
                    numMisplacedLetters,
                    (position) => ({
                        position,
                        required: LetterAtPositionInWord.Misplaced,
                        letter: alphabet[position]
                    })
                );

                const wordListLetterRulesSpy = jest
                    .spyOn(wordList, 'letterRules', 'get')
                    .mockReturnValueOnce([
                        impossibleLetterRule,
                        nonmatchingMisplacedLetterRule,
                        ...previouslyMisplacedLetterRules
                    ]);

                const guess = misplacedAlphabet.slice(0, numMisplacedLetters + 1);

                const result = retryMisplacedLettersStrategy.scoreForGuess(guess);

                expect(wordListLetterRulesSpy).toHaveBeenCalledOnce();
                expect(result).toStrictEqual(numMisplacedLetters);
            }
        );
    });
});
