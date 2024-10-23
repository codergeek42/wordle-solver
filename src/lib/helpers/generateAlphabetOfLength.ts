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
import { WordleSolverTestError } from '../wordleSolverError';

/**
 * Generates an alphabet of `numLetters` size, starting from 'A', up to as many letters as desired in ascending order.
 *
 * @param {number} numLetters the number of letters in the resulting alphabet
 *
 * @returns {string} the left-most substring of `A...Z` of length `numLetters`
 *
 * @example Generating the first five letters
 * ```typescript
 *  const AtoE = generateAlphabetOfLength(5);
 *  expect(AtoE).toStrictEqual('ABCDE');
 * ```
 */
export default function generateAlphabetOfLength(numLetters: number): string {
    if (numLetters < 0 || numLetters >= 26) {
        throw new WordleSolverTestError('generateAlphabetOfLength: numLetters out of range');
    }
    return String.fromCharCode(...range(0, numLetters).map((ord) => 'A'.charCodeAt(0) + ord));
}
