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

import { countBy, sum } from 'lodash';
import { NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';

export default class LetterFrequencyStrategy extends NextWordGuesserStrategyBase {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    scoreForGuess(guess: string): number {
        const countLettersAtPosition = (position: number): Record<string, number> =>
            countBy(this.wordList.words.flatMap((word) => [word[position]]));

        return sum(
            Array.from(guess).map((letter, position) => {
                const lettersAtPosition = countLettersAtPosition(position);
                return lettersAtPosition[letter] / this.wordList.possibleLetters[position].length;
            })
        );
    }
}
