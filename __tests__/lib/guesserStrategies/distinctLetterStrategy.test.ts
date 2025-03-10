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

import { range } from 'lodash';
import { WordLength } from '../../../__data__/alphabet';
import DistinctLettersStrategy from '../../../src/lib/guesserStrategies/distinctLettersStrategy';
import NextWordGuesserStrategyBase from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';
import generateAlphabetWords from '../../../src/lib/helpers/generateAlphabetWords';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';

describe(DistinctLettersStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList(['TEST']);
            const distinctLettersStrategy = new DistinctLettersStrategy(wordList);

            expect(distinctLettersStrategy).toBeDefined();
            expect(distinctLettersStrategy.scoreForGuess).toBeFunction();
            expect(distinctLettersStrategy.wordList).toBeInstanceOf(WordList);
            expect(distinctLettersStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(distinctLettersStrategy).toBeInstanceOf(DistinctLettersStrategy);
        });
    });

    describe(DistinctLettersStrategy.prototype.scoreForGuess, () => {
        const numGuessedLettersCases = range(0, WordLength + 1);
        describe('scores the guess based on the number of distinct unguessed letters in the word', () => {
            describe.each(numGuessedLettersCases)('(%i guessed already)', (numGuessedLetters) => {
                const numDistinctLettersCases = range(0, WordLength);
                it.each(numDistinctLettersCases)('(%i distinct unguessed)', (numDistinctLetters) => {
                    const previousGuessLetters = generateAlphabetOfLength(numGuessedLetters);
                    const distinctGuessLetters = generateAlphabetOfLength(numGuessedLetters + numDistinctLetters);

                    const distinctLettersStrategy = new DistinctLettersStrategy(
                        new WordList(generateAlphabetWords(distinctGuessLetters))
                    );

                    const getAlreadyGuessedLettersSpy = jest
                        .spyOn(distinctLettersStrategy, 'getAlreadyGuessedLetters')
                        .mockReturnValueOnce(Array.from(previousGuessLetters));
                    const result = distinctLettersStrategy.scoreForGuess(distinctGuessLetters);

                    expect(getAlreadyGuessedLettersSpy).toHaveBeenCalledOnce();
                    expect(result).toStrictEqual(numDistinctLetters);
                });
            });
        });
    });
});
