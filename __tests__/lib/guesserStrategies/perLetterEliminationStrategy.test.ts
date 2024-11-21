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

import { range, sum, times } from 'lodash';
import PerLetterEliminationStrategy from '../../../src/lib/guesserStrategies/perLetterEliminationStrategy';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';
import 'jest-extended';
import 'jest-mock-extended';
import { WordLength } from '../../../__data__/alphabet';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';
import { LetterAtPositionInWord } from '../../../src/lib/letterAtPosition';

describe(PerLetterEliminationStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList(['TEST']);
            const perLetterEliminationStrategy = new PerLetterEliminationStrategy(wordList);

            expect(perLetterEliminationStrategy).toBeDefined();
            expect(perLetterEliminationStrategy.wordList).toBeInstanceOf(WordList);
            expect(perLetterEliminationStrategy.scoreForGuess).toBeFunction();
            expect(perLetterEliminationStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(perLetterEliminationStrategy).toBeInstanceOf(PerLetterEliminationStrategy);
        });
    });

    describe(PerLetterEliminationStrategy.totalPossibleLettersInWordList, () => {
        it('sums the number of letters in the given word list', () => {
            const wordList = new WordList();
            const possibleLettersSpy = jest
                .spyOn(wordList, 'possibleLetters', 'get')
                .mockReturnValueOnce(times(WordLength, (maxPos) => Array.from(generateAlphabetOfLength(maxPos + 1))));

            const result = PerLetterEliminationStrategy.totalPossibleLettersInWordList(wordList);
            expect(possibleLettersSpy).toHaveBeenCalledOnce();
            expect(result).toStrictEqual(sum(range(1, WordLength + 1)));
        });
    });

    describe(PerLetterEliminationStrategy.generateGuessPositionLetterRules, () => {
        it('creates a rule for each letter required as Misplaced from the given guess', () => {
            const guess = generateAlphabetOfLength(WordLength);
            const result = PerLetterEliminationStrategy.generateGuessPositionLetterRules(guess);
            expect(result).toStrictEqual(
                Array.from(guess).map((letter, position) => ({
                    position,
                    letter,
                    required: LetterAtPositionInWord.Misplaced
                }))
            );
        });
    });

    describe(PerLetterEliminationStrategy.prototype.scoreForGuess, () => {
        it('scores the guess based on the number of letter possibilities eliminated', () => {
            const guess = 'TEST';
            const wordList = new WordList(['WORDS']);
            const wordListWithGuess = new WordList(['GUESS']);
            const perLetterEliminationStrategy = new PerLetterEliminationStrategy(wordList);
            const expectedScore = 42;

            const withPositionLetterRulesSpy = jest
                .spyOn(wordList, 'withPositionLetterRules')
                .mockReturnValueOnce(wordListWithGuess);

            const totalPossibleLettersInWordListSpy = jest
                .spyOn(PerLetterEliminationStrategy, 'totalPossibleLettersInWordList')
                .mockReturnValueOnce(1 + expectedScore)
                .mockReturnValueOnce(1);

            // The guesser uses a Misplaced at each position; so we'll use an Impossible instead
            // just as a pass-through for the mock.
            const generateGuessPositionLetterRulesReturn = [
                { letter: '_', required: LetterAtPositionInWord.Impossible }
            ];
            const generateGuessPositionLetterRulesSpy = jest
                .spyOn(PerLetterEliminationStrategy, 'generateGuessPositionLetterRules')
                .mockReturnValueOnce(generateGuessPositionLetterRulesReturn);

            const result = perLetterEliminationStrategy.scoreForGuess(guess);

            expect(result).toStrictEqual(expectedScore);

            expect(totalPossibleLettersInWordListSpy).toHaveBeenCalledTimes(2);
            expect(totalPossibleLettersInWordListSpy).toHaveBeenNthCalledWith(1, wordList);
            expect(generateGuessPositionLetterRulesSpy).toHaveBeenCalledExactlyOnceWith(guess);
            expect(withPositionLetterRulesSpy).toHaveBeenCalledExactlyOnceWith(generateGuessPositionLetterRulesReturn);
            expect(totalPossibleLettersInWordListSpy).toHaveBeenNthCalledWith(2, wordListWithGuess);
        });
    });
});
