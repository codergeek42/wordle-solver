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
import { WordLength } from '../../../__data__/alphabet';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';

describe(generateAlphabetOfLength, () => {
    it.each(range(0, WordLength + 1))('generates alphabet of length %i', (alphabetLength) => {
        const expected = String.fromCharCode(...range(0, alphabetLength).map((ord) => 'A'.charCodeAt(0) + ord));
        const result = generateAlphabetOfLength(alphabetLength);
        expect(result).toStrictEqual(expected);
    });
});
