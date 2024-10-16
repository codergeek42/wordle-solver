/*****
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
 *****/

import { get, keys, mapValues } from 'lodash';
import { LetterAtPositionInWord } from '../../src/lib/letterAtPosition';
import { WordGuessAndResult, WordGuessAndScore } from '../../src/lib/wordGuessAndResult';
import { GuesserStrategies, WordleSolver } from '../../src/lib/wordleSolver';
import WordList from '../../src/lib/wordList';
import { NextWordGuesserStrategyBase } from '../../src/lib/nextWordGuesserStrategy';
import 'jest-extended';
import DistinctLettersStrategy from '../../src/lib/guesserStrategies/distinctLettersStrategy';
import RetryMisplacedLettersStrategy from '../../src/lib/guesserStrategies/retryMisplacedLettersStrategy';
import PerLetterEliminationStrategy from '../../src/lib/guesserStrategies/perLetterEliminationStrategy';
import LetterFrequencyStrategy from '../../src/lib/guesserStrategies/letterFrequencyStrategy';

describe(WordleSolver, () => {
    let wordList: WordList;
    let wordleSolver: WordleSolver;
    let wordleSolverStrategySpies: Record<keyof GuesserStrategies, jest.SpyInstance>;
    let wordleSolverStrategyTypes: Record<keyof GuesserStrategies, typeof NextWordGuesserStrategyBase>;

    beforeEach(() => {
        wordList = new WordList(['TEST']);
        wordleSolver = new WordleSolver(wordList);
        wordleSolverStrategyTypes = {
            distinctLetters: DistinctLettersStrategy,
            retryMisplacedLetters: RetryMisplacedLettersStrategy,
            perLetterEliminations: PerLetterEliminationStrategy,
            letterFrequency: LetterFrequencyStrategy
        };

        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated with solver strategies', () => {
            expect(wordleSolver).toBeDefined();
            expect(wordleSolver.guesserStrategies).toBeDefined();
            keys(wordleSolverStrategyTypes).forEach((strategy) => {
                const guesserStrategy = get(wordleSolver.guesserStrategies, strategy);
                const expectedStrategyType = get(wordleSolverStrategyTypes, strategy);
                expect(guesserStrategy).toBeDefined();
                expect(guesserStrategy).toBeInstanceOf(expectedStrategyType);
            });
        });
    });

    describe(WordleSolver.prototype.withPreviousGuess, () => {
        it('is called on each guesser strategy', () => {
            const previousGuess: WordGuessAndResult = {
                word: 'ABCD',
                result: [
                    {
                        letter: 'A',
                        required: LetterAtPositionInWord.Impossible
                    }
                ]
            };
            wordleSolverStrategySpies = mapValues(wordleSolver.guesserStrategies, (guesserStrategy) =>
                jest.spyOn(guesserStrategy, 'withPreviousGuess').mockReturnThis()
            );

            const result = wordleSolver.withPreviousGuess(previousGuess);

            mapValues(wordleSolverStrategySpies, (withPreviousGuessSpy) => {
                expect(withPreviousGuessSpy).toHaveBeenCalledExactlyOnceWith(previousGuess);
            });
            expect(result).toStrictEqual(wordleSolver);
        });
    });

    describe(WordleSolver.prototype.guessNextWord, () => {
        it('is called on each solver strategy', () => {
            const nextGuessAndScore: WordGuessAndScore = {
                word: 'TEST',
                score: 42
            };
            const wordleSolverStrategySpies = mapValues(wordleSolver.guesserStrategies, (guesserStrategy) =>
                jest.spyOn(guesserStrategy, 'guessNextWordAndScore').mockReturnValueOnce(nextGuessAndScore)
            );

            const result = wordleSolver.guessNextWord();

            mapValues(wordleSolverStrategySpies, (withPreviousGuessSpy) => {
                expect(withPreviousGuessSpy).toHaveBeenCalledOnce();
            });
            expect(result).toStrictEqual(
                mapValues(wordleSolver.guesserStrategies, (_guesserStrategy) => nextGuessAndScore)
            );
        });
    });
});
