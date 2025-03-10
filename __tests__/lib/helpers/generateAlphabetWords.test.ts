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
import * as generateAlphabetWordsModule from '../../../src/lib/helpers/generateAlphabetWords';
import { WordleSolverTestError } from '../../../src/lib/wordleSolverError';

const { default: generateAlphabetWords } = generateAlphabetWordsModule;

describe(generateAlphabetWords, () => {
    it('should generate all possible words from the given alphabet', () => {
        const alphabet = 'ABC';
        const expectedAlphabetWords = ['AA', 'BA', 'CA', 'AB', 'BB', 'CB', 'AC', 'BC', 'CC'];
        const result = generateAlphabetWords(alphabet, 2);
        expect(result).toStrictEqual(expectedAlphabetWords);
    });

    it('should throw a WorldSolverTestError if the size argument is negative', () => {
        const alphabet = 'ABC';
        const testCall = () => generateAlphabetWords(alphabet, -1);
        expect(testCall).toThrow(WordleSolverTestError);
    });

    it('should use WordLength as the default wordSize parameter', () => {
        const alphabet = 'ABC';
        const result = generateAlphabetWords(alphabet);
        const expected = generateAlphabetWords(alphabet, WordLength);
        expect(result).toStrictEqual(expected);
    });
});
