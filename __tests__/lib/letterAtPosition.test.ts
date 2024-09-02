import {
    LetterAtPositionInWord,
    LetterAtPositionInWordRule,
    letterAtPositionInWordRuleComparator
} from '../../src/lib/letterAtPosition';

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
