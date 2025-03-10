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

import { sum } from 'lodash';
import NextWordGuesserStrategyBase from '../nextWordGuesserStrategy';

/**
 * A guessing strategy which scores the candidate words based on the letter frequency at each position.
 */
export default class LetterFrequencyStrategy extends NextWordGuesserStrategyBase {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    /**
     * Scores the guess based on the letter frequency at each position, using the given dictionary.
     * For example, if the dictionary is:
     * ```
     *  BREAD,
     *  BROOD,
     *  BLOOD,
     *  CROOK,
     * ```
     * then the most frequently guessed letters at each position are `B`, `R`, `O`, `O`, and `D`, respectively,
     * so `BROOD` would be the highest-scored candidate.
     *
     * @param guess - the candidate word
     * @returns the score for `guess` based on the sum of the relative frequency of each of its letters.
     */
    scoreForGuess(guess: string): number {
        const lettersCount = this.wordList.countLetters();
        return (
            sum(
                Array.from(guess).map(
                    (letter, position) =>
                        lettersCount[position][letter] / this.wordList.possibleLetters[position].length
                )
            ) / guess.length
        );
    }
}
