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

import LetterFrequencyStrategy from '../../../src/lib/guesserStrategies/letterFrequencyStrategy';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';

describe(LetterFrequencyStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList([]);
            const perLetterEliminationStrategy = new LetterFrequencyStrategy(wordList);

            expect(perLetterEliminationStrategy).toBeDefined();
            expect(perLetterEliminationStrategy.wordList).toBeInstanceOf(WordList);
            expect(perLetterEliminationStrategy.scoreForGuess).toBeFunction();
            expect(perLetterEliminationStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(perLetterEliminationStrategy).toBeInstanceOf(LetterFrequencyStrategy);
        });
    });

    describe(LetterFrequencyStrategy.prototype.scoreForGuess, () => {
        it('scores the guess based on letter frequency', () => {
            const wordList = new WordList(['AAA', 'BZZ', 'CCZ']);
            const letterFrequencyStrategy = new LetterFrequencyStrategy(wordList);

            const result = Array.from(wordList.words, (word) => ({
                word,
                score: letterFrequencyStrategy.scoreForGuess(word)
            }));
            console.log('@@ res = ', JSON.stringify(result, null, 2));
        });
    });
});
