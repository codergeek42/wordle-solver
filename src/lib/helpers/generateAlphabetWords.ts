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

import { WordLength } from '../../../__data__/alphabet';
import { WordleSolverTestError } from '../wordleSolverError';

/**
 * Recursively calculates all possible words from the given `alphabet` of the length `wordLength`
 *
 * @param alphabet - the string of all possible characters
 * @param wordLength - the length of the resulting words
 *
 * @returns the calculated list of all such possible words.
 *
 * @throws `WordleSolverTestError`
 * if the `wordLength` is zero or negative
 *
 * @example All possible 2-letter permutations of A and B
 * ```typescript
 * const AsAndBs = generateAlphabetWords('AB', 2).sorted();
 * expect(AsAndBs).toEqual(['AA', 'AB', 'BA', 'BB']);
 * ```
 */
export default function generateAlphabetWords(alphabet: string, wordLength: number = WordLength): string[] {
    const prefixLetters = Array.from(alphabet);
    if (wordLength <= 0) {
        throw new WordleSolverTestError('generateAlphabetWords: invalid size argument');
    }
    return wordLength == 1
        ? prefixLetters
        : generateAlphabetWords(alphabet, wordLength - 1).flatMap((word) =>
              prefixLetters.map((letter) => letter.concat(word))
          );
}
