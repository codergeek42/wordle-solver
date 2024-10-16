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

import {
    LetterAtPositionInWord,
    LetterAtPositionInWordRule,
    letterAtPositionInWordRuleComparator
} from '../../src/lib/letterAtPosition';
import 'jest-extended';

describe(letterAtPositionInWordRuleComparator, () => {
    const impossibleRuleA: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Impossible,
        letter: 'A'
    };

    const impossibleRuleB: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Impossible,
        letter: 'A'
    };

    const misplacedRuleAAt0: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Misplaced,
        letter: 'A',
        position: 0
    };

    const misplacedRuleAAt1: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Misplaced,
        letter: 'A',
        position: 0
    };

    const mandatoryRuleBAt0: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Mandatory,
        letter: 'B',
        position: 0
    };

    const mandatoryRuleBAt1: LetterAtPositionInWordRule = {
        required: LetterAtPositionInWord.Mandatory,
        letter: 'B',
        position: 1
    };

    it.each([impossibleRuleA, misplacedRuleAAt0])('should sort Mandatory rule after $required', (nonMandatoryRule) => {
        const sortResultLeftAssoc = letterAtPositionInWordRuleComparator(mandatoryRuleBAt0, nonMandatoryRule);
        const sortResultRightAssoc = letterAtPositionInWordRuleComparator(nonMandatoryRule, mandatoryRuleBAt0);

        expect(sortResultLeftAssoc).toBePositive();
        expect(sortResultRightAssoc).toBeNegative();
    });

    const equalPriorityTestCases: {
        caseName: string;
        twoRules: LetterAtPositionInWordRule[];
    }[] = [
        {
            caseName: 'Mandatory',
            twoRules: [mandatoryRuleBAt0, mandatoryRuleBAt1]
        },
        {
            caseName: 'non-Mandatory (both Impossible)',
            twoRules: [impossibleRuleA, impossibleRuleB]
        },
        {
            caseName: 'non-Mandatory (both Misplaced)',
            twoRules: [misplacedRuleAAt0, misplacedRuleAAt1]
        },
        {
            caseName: 'non-Mandatory (one Misplaced, one Impossible)',
            twoRules: [misplacedRuleAAt0, impossibleRuleB]
        }
    ];
    it.each(equalPriorityTestCases)('should sort equal priority for $caseName rules', ({ twoRules }) => {
        expect(twoRules).toBeArrayOfSize(2);

        const sortResultLeftAssoc = letterAtPositionInWordRuleComparator(twoRules[0], twoRules[1]);
        const sortResultRightAssoc = letterAtPositionInWordRuleComparator(twoRules[1], twoRules[0]);

        expect(sortResultLeftAssoc).toBe(0);
        expect(sortResultRightAssoc).toBe(0);
    });
});
