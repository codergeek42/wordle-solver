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

import { mapValues } from 'lodash';
import DistinctLettersStrategy from './guesserStrategies/distinctLettersStrategy';
import { WordGuessAndResult, WordGuessAndScore } from './wordGuessAndResult';
import WordList from './wordList';
import RetryMisplacedLettersStrategy from './guesserStrategies/retryMisplacedLettersStrategy';
import PerLetterEliminationStrategy from './guesserStrategies/perLetterEliminationStrategy';
import LetterFrequencyStrategy from './guesserStrategies/letterFrequencyStrategy';

export type GuesserStrategies = {
    distinctLetters: DistinctLettersStrategy;
    retryMisplacedLetters: RetryMisplacedLettersStrategy;
    perLetterEliminations: PerLetterEliminationStrategy;
    letterFrequency: LetterFrequencyStrategy;
};

export class WordleSolver {
    private myGuesserStrategies: GuesserStrategies;

    constructor(protected myWordList: WordList) {
        this.myGuesserStrategies = {
            distinctLetters: new DistinctLettersStrategy(myWordList),
            retryMisplacedLetters: new RetryMisplacedLettersStrategy(myWordList),
            perLetterEliminations: new PerLetterEliminationStrategy(myWordList),
            letterFrequency: new LetterFrequencyStrategy(myWordList)
        };
    }

    withPreviousGuess(candidate: WordGuessAndResult): this {
        this.myGuesserStrategies = mapValues(this.myGuesserStrategies, (guesserStrategy) =>
            guesserStrategy.withPreviousGuess(candidate)
        );
        return this;
    }

    guessNextWord(): Record<keyof GuesserStrategies, WordGuessAndScore> {
        const nextGuesses = mapValues(this.myGuesserStrategies, (guesserStrategy) =>
            guesserStrategy.guessNextWordAndScore()
        );
        return nextGuesses;
    }

    get guesserStrategies(): GuesserStrategies {
        return this.myGuesserStrategies;
    }
}
