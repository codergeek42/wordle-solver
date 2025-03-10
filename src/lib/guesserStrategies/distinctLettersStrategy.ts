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

import { difference, uniq } from 'lodash';
import NextWordGuesserStrategyBase from '../nextWordGuesserStrategy';

/**
 * A guessing strategy which scores the candidate words based on the number of distinct unguessed letters.
 */
export default class DistinctLettersStrategy extends NextWordGuesserStrategyBase {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    /**
     * Scores the guess based on the number of distinct unguessed letters, ignoring repetitions.
     * For example, if there are no guesses yet, `BREAD` would yield 5 and `BOOKS` would yield 4;
     * but if `BAKER` had already been guessed before those, then `BREAD` would yield only 1 (D), while
     * `BOOKS` would yield 2 (O and S).
     *
     * (Note that the letters shown in these example results are for demonstration only; and the actual
     * returned score does not include the information about those letters, only their total count.)
     *
     * @param guess - the candidate word
     * @returns the number of distinct letters in `guess` that have not yet been guessed.
     */
    scoreForGuess(guess: string): number {
        const prev = this.getAlreadyGuessedLetters();
        const score = uniq(difference(Array.from(guess), prev)).length;
        return score;
    }
}
