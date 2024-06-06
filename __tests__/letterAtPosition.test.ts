import {
    LetterAtPosition,
    LetterAtPositionRule,
    LetterRule,
    doesLetterMatchRule,
    doesWordMatchLetterAtPosition
} from '../src/lib/letterAtPosition';

type DoesLetterMatchRuleTestCase = {
    caseName: string;
    caseRule: LetterRule;
    testLetter: string;
    expectedResult: boolean;
};

type DoesWordMatchLetterAtPositionTestCase = {
    caseName: string;
    caseRule: LetterAtPositionRule;
    expectedResult: boolean;
};

describe(doesLetterMatchRule.name, () => {
    const mandatoryCases: DoesLetterMatchRuleTestCase[] = [
        {
            caseName: 'letter is mandatory and matches given letter',
            caseRule: {
                letter: 'A',
                required: LetterAtPosition.Mandatory
            },
            testLetter: 'A',
            expectedResult: true
        },
        {
            caseName: 'letter is mandatory but does not match given letter',
            caseRule: {
                letter: 'Z',
                required: LetterAtPosition.Mandatory
            },
            testLetter: 'A',
            expectedResult: false
        }
    ];

    const impossibleCases: DoesLetterMatchRuleTestCase[] = [
        {
            caseName: 'letter is impossible but matches given letter',
            caseRule: {
                required: LetterAtPosition.Impossible,
                letter: 'Z'
            },
            testLetter: 'A',
            expectedResult: true
        },
        {
            caseName: 'letter is impossible and does not match given letter',
            caseRule: {
                required: LetterAtPosition.Impossible,
                letter: 'Z'
            },
            testLetter: 'A',
            expectedResult: true
        }
    ];

    const possibleCases: DoesLetterMatchRuleTestCase[] = [
        {
            caseName: 'letter is possible and matches given letter',
            caseRule: {
                letter: 'A',
                required: LetterAtPosition.Possible
            },
            testLetter: 'A',
            expectedResult: true
        },
        {
            caseName: 'letter is possible and does not match given letter',
            caseRule: {
                letter: 'A',
                required: LetterAtPosition.Possible
            },
            testLetter: 'Z',
            expectedResult: true
        }
    ];

    it.each([...mandatoryCases, ...possibleCases, ...impossibleCases])(
        '$caseName',
        ({ caseRule, testLetter, expectedResult }) => {
            const result = doesLetterMatchRule(testLetter, caseRule);

            expect(result).toBe(expectedResult);
        }
    );
});

describe(doesWordMatchLetterAtPosition.prototype.name, () => {
    const testWord = 'TEST';
    const trueTestCases: DoesWordMatchLetterAtPositionTestCase[] = [
        {
            caseName: 'T as first letter is Possible',
            caseRule: {
                letter: 'T',
                required: LetterAtPosition.Possible,
                position: 0
            },
            expectedResult: true
        },
        {
            caseName: 'T as last letter is Mandatory',
            caseRule: {
                letter: 'T',
                required: LetterAtPosition.Mandatory,
                position: 3
            },
            expectedResult: true
        },
        {
            caseName: 'T as second letter is Impossible',
            caseRule: {
                letter: 'T',
                required: LetterAtPosition.Impossible,
                position: 1
            },
            expectedResult: true
        }
    ];

    const falseTestCases: DoesWordMatchLetterAtPositionTestCase[] = [
        {
            caseName: 'Z as last letter is Mandatory',
            caseRule: {
                letter: 'Z',
                required: LetterAtPosition.Mandatory,
                position: 3
            },
            expectedResult: false
        },
        {
            caseName: 'A as first letter is Mandatory',
            caseRule: {
                letter: 'A',
                required: LetterAtPosition.Mandatory,
                position: 0
            },
            expectedResult: false
        },
        {
            caseName: 'E as second letter is Impossible',
            caseRule: {
                letter: 'E',
                required: LetterAtPosition.Impossible,
                position: 1
            },
            expectedResult: false
        }
    ];

    it.each([...trueTestCases, ...falseTestCases])(
        '$caseName is $expectedResult',
        ({ caseRule, expectedResult }) => {
            const result = doesWordMatchLetterAtPosition(testWord, caseRule);

            expect(result).toBe(expectedResult);
        }
    );
});
