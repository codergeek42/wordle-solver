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

import { maxBy, sumBy, uniq } from 'lodash';
import { LetterAtPositionInWord } from '../../src/lib/letterAtPosition';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../../src/lib/nextWordGuesserStrategy';
import { WordGuessAndResult } from '../../src/lib/wordGuessAndResult';
import { NoMoreGuessesError } from '../../src/lib/wordleSolverError';
import WordList from '../../src/lib/wordList';
import 'jest-extended';

// Allows us to instantiate the base class as-is.
class NextWordGuesserStrategyBaseTest extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...ctorParams: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...ctorParams);
    }

    scoreForGuess(guess: string): number {
        return sumBy(guess, (chr) => chr.charCodeAt(0));
    }
}

describe(NextWordGuesserStrategyBase, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated from empty WordList with default constructor', () => {
            const emptyWordList = new WordList([]);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(emptyWordList);

            expect(nextWordGuesserStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(nextWordGuesserStrategy.wordList).toBeInstanceOf(WordList);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.withPreviousGuess, () => {
        it('processes exclusions and appends previous guess result', () => {
            const testWordList = new WordList(['AAAAA', 'BBBBB']);
            const previousGuess: WordGuessAndResult = {
                word: 'CCCCC',
                result: [
                    {
                        letter: 'C',
                        required: LetterAtPositionInWord.Impossible
                    }
                ]
            };

            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(testWordList);
            const processExclusionsFromRulesSpy = jest
                .spyOn(nextWordGuesserStrategy.wordList, 'processExclusionsFromRules')
                .mockImplementationOnce((_lettersAtPositionRules) => {
                    return;
                });
            const arrayPushSpy = jest
                .spyOn(nextWordGuesserStrategy.previousGuesses, 'push')
                .mockImplementationOnce((_item) => {
                    return 1;
                });

            const result = nextWordGuesserStrategy.withPreviousGuess(previousGuess);

            expect(processExclusionsFromRulesSpy).toHaveBeenCalledExactlyOnceWith(previousGuess.result);
            expect(arrayPushSpy).toHaveBeenCalledExactlyOnceWith(previousGuess);
            expect(result).toStrictEqual(nextWordGuesserStrategy);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.guessNextWordAndScore, () => {
        it('throws NoMoreGuessesError if result words list is empty', () => {
            const emptyWordList = new WordList([]);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(emptyWordList);
            const testCall = () => nextWordGuesserStrategy.guessNextWordAndScore();
            expect(testCall).toThrow(NoMoreGuessesError);
        });

        it('returns the next highest-score guess if result word list is not empty', () => {
            const wordList = new WordList(['AAAAA', 'BBBBB', 'CCCCC']);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(wordList);
            const wordsWithScores = wordList.words.map((word) => ({
                word,
                score: nextWordGuesserStrategy.scoreForGuess(word)
            }));
            const expected = maxBy(wordsWithScores, 'score');

            const result = nextWordGuesserStrategy.guessNextWordAndScore();

            expect(result).toStrictEqual(expected);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.getAlreadyGuessedLetters, () => {
        const wordList = new WordList();
        const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(wordList);
        it('returns a flattened list of unique guessed letters', () => {
            const previousGuesses: WordGuessAndResult[] = [
                { word: 'ABC', result: [] },
                { word: 'BCD', result: [] },
                { word: 'DEF', result: [] },
                { word: 'GHI', result: [] }
            ];
            const expected = uniq(previousGuesses.flatMap((guess) => Array.from(guess.word)));
            const previousGuessesSpy = jest
                .spyOn(nextWordGuesserStrategy, 'previousGuesses', 'get')
                .mockReturnValueOnce(previousGuesses);

            const result = nextWordGuesserStrategy.getAlreadyGuessedLetters();

            expect(previousGuessesSpy).toHaveBeenCalledOnce();
            expect(result).toStrictEqual(expected);
        });
    });
});
