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

export enum LetterAtPositionInWord {
    // Right letter, right position.
    Mandatory = 'Mandatory',
    // Right letter, wrong position.
    Misplaced = 'Misplaced',
    // Wrong letter entirely (not in word).
    Impossible = 'Impossible'
}

export function letterAtPositionInWordRuleComparator(
    posRuleA: LetterAtPositionInWordRule,
    posRuleB: LetterAtPositionInWordRule
): number {
    if (posRuleA.required === LetterAtPositionInWord.Mandatory) {
        return posRuleB.required === LetterAtPositionInWord.Mandatory ? 0 : +1;
    }
    return posRuleB.required == LetterAtPositionInWord.Mandatory ? -1 : 0;
}

export type LetterRule = {
    letter: string;
    required: LetterAtPositionInWord;
};

export type LetterAtPositionInWordRule = LetterRule & {
    position?: number;
};

export type LetterWithPosition = Required<Omit<LetterAtPositionInWordRule, 'required'>>;
