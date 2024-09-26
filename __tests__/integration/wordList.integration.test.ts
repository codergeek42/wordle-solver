import { times } from 'lodash';
import { WordLength } from '../../__data__/alphabet';
import { LetterAtPositionInWord, LetterAtPositionInWordRule } from '../../src/lib/letterAtPosition';
import WordList from '../../src/lib/wordList';
import 'jest-extended';

describe(`${WordList.name} integration`, () => {
    const testCases: {
        caseName: string;
        startingWords: string[];
        positionLetterRules: LetterAtPositionInWordRule[];
        expectedWords: string[];
    }[] = [
        {
            caseName: 'excludes A at every position (Impossible)',
            startingWords: [
                'A'.repeat(WordLength),
                ...times(WordLength, (idx) => 'B'.repeat(idx) + 'A' + 'B'.repeat(WordLength - idx)),
                'B'.repeat(WordLength)
            ],
            positionLetterRules: [
                {
                    letter: 'A',
                    required: LetterAtPositionInWord.Impossible
                }
            ],
            expectedWords: ['B'.repeat(WordLength)]
        },
        {
            caseName: 'requires B at every position (Mandatory)',
            startingWords: [
                'A'.repeat(WordLength),
                ...times(WordLength, (idx) => 'B'.repeat(idx) + 'A' + 'B'.repeat(WordLength - idx)),
                'B'.repeat(WordLength)
            ],
            positionLetterRules: times(WordLength, (idx) => ({
                position: idx,
                letter: 'B',
                required: LetterAtPositionInWord.Mandatory
            })),
            expectedWords: ['B'.repeat(WordLength)]
        },
        {
            caseName: 'requires B at one position other than 0 (Misplaced)',
            startingWords: [
                'A'.repeat(WordLength),
                ...times(WordLength, (idx) => 'A'.repeat(idx) + 'B' + 'A'.repeat(WordLength - idx)),
                'B'.repeat(WordLength)
            ],
            positionLetterRules: [
                {
                    letter: 'B',
                    position: 0,
                    required: LetterAtPositionInWord.Misplaced
                }
            ],
            expectedWords: [
                ...times(WordLength - 1, (idx) => 'A'.repeat(idx + 1) + 'B' + 'A'.repeat(WordLength - (idx + 1)))
            ]
        },
        {
            caseName:
                'finds B at position 1 (Mandatory there, Impossible elsewhere) after excluding it from position 0 (Misplaced)',
            startingWords: [
                'A'.repeat(WordLength),
                ...times(WordLength, (idx) => 'A'.repeat(idx) + 'B' + 'A'.repeat(WordLength - idx - 1)),
                'B'.repeat(WordLength)
            ],
            positionLetterRules: [
                {
                    letter: 'B',
                    position: 0,
                    required: LetterAtPositionInWord.Misplaced
                },
                {
                    letter: 'B',
                    position: 1,
                    required: LetterAtPositionInWord.Mandatory
                },
                {
                    letter: 'B',
                    required: LetterAtPositionInWord.Impossible
                }
            ],
            expectedWords: ['AB' + 'A'.repeat(WordLength - 2)]
        }
    ];
    it.each(testCases)('$caseName', ({ startingWords, positionLetterRules, expectedWords }) => {
        const wordList = new WordList(startingWords);
        const wordListWithRulesApplied = wordList.withPositionLetterRules(positionLetterRules);
        expect(wordListWithRulesApplied.words).toBeArrayOfSize(expectedWords.length);
        expect(wordListWithRulesApplied.words).toIncludeAllMembers(expectedWords);
    });
});
