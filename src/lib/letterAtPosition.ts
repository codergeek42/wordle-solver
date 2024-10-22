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

/**
 * Stores the requirement for a given letter at some position in the word.
 */
export enum LetterAtPositionInWord {
    // Right letter, right position.
    Mandatory = 'Mandatory',
    // Right letter, wrong position.
    Misplaced = 'Misplaced',
    // Wrong letter entirely (not in word).
    Impossible = 'Impossible'
}

/**
 * A comparator for sorting two `LetterAtPositionInWordRule` objects, following the usual
 * pattern of positive (if the first rule is greater), positive (if the second rule is
 * greater), or zero (if they are equal).
 *
 * In the cae of `LetterAtPositionInWordRule` objects, this comparison is based on which
 * of the given rules has their `required` property set to `Mandatory`: If one of the rules
 * has a `Mandatory` and the other does not, then the one which does is the considered the
 * greater of the two; and if neither or both of them have a `Mandatory`, then they are
 * considered equal.
 *
 * @param posRuleA - The first rule.
 * @param posRuleB - The second rule.
 * @returns +1 if `posRuleA` > `posRuleB`; -1 if `posRuleA` < `posRuleB`; or 0 otherwise.
 */
export function letterAtPositionInWordRuleComparator(
    posRuleA: LetterAtPositionInWordRule,
    posRuleB: LetterAtPositionInWordRule
): number {
    if (posRuleA.required === LetterAtPositionInWord.Mandatory) {
        return posRuleB.required === LetterAtPositionInWord.Mandatory ? 0 : +1;
    }
    return posRuleB.required == LetterAtPositionInWord.Mandatory ? -1 : 0;
}

/**
 * A combination of letter and its associated `LetterAtPositionInWord` requirement.
 * @see {@link LetterAtPositionInWord}
 */
export type LetterRule = {
    letter: string;
    required: LetterAtPositionInWord;
};

/**
 * A combination of a given `LetterRule` with its associated position.
 * @see {@link LetterRule}
 */
export type LetterAtPositionInWordRule = LetterRule & {
    position?: number;
};

/**
 * The same type as `LetterAtPositionInWordRule`, but with the the `position` property
 * not optional.
 * @see {@link LetterAtPositionInWordRule}
 */
export type LetterWithPosition = Required<Omit<LetterAtPositionInWordRule, 'required'>>;
