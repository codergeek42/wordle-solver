import { maxBy, sumBy, uniq } from 'lodash';
import { LetterAtPositionInWord } from '../../src/lib/letterAtPosition';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../../src/lib/nextWordGuesserStrategy';
import { WordGuessAndResult } from '../../src/lib/wordGuessAndResult';
import { NoMoreGuessesError } from '../../src/lib/wordleSolverError';
import WordList from '../../src/lib/wordList';
import 'jest-extended';

// Allows us to instantiate the base class as-is.
class NextWordGuesserStrategyBaseTest extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...ctorParams: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...ctorParams);
    }

    scoreForGuess(guess: string): number {
        return sumBy(guess, (chr) => chr.charCodeAt(0));
    }
}

describe(NextWordGuesserStrategyBase, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated from empty WordList with default constructor', () => {
            const emptyWordList = new WordList([]);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(emptyWordList);

            expect(nextWordGuesserStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(nextWordGuesserStrategy.wordList).toBeInstanceOf(WordList);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.withPreviousGuess, () => {
        it('processes exclusions and appends previous guess result', () => {
            const testWordList = new WordList(['AAAAA', 'BBBBB']);
            const previousGuess: WordGuessAndResult = {
                word: 'CCCCC',
                result: [
                    {
                        letter: 'C',
                        required: LetterAtPositionInWord.Impossible
                    }
                ]
            };

            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(testWordList);
            const processExclusionsFromRulesSpy = jest
                .spyOn(nextWordGuesserStrategy.wordList, 'processExclusionsFromRules')
                .mockImplementationOnce((_lettersAtPositionRules) => {
                    return;
                });
            const arrayPushSpy = jest
                .spyOn(nextWordGuesserStrategy.previousGuesses, 'push')
                .mockImplementationOnce((_item) => {
                    return 1;
                });

            const result = nextWordGuesserStrategy.withPreviousGuess(previousGuess);

            expect(processExclusionsFromRulesSpy).toHaveBeenCalledExactlyOnceWith(previousGuess.result);
            expect(arrayPushSpy).toHaveBeenCalledExactlyOnceWith(previousGuess);
            expect(result).toStrictEqual(nextWordGuesserStrategy);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.guessNextWordAndScore, () => {
        it('throws NoMoreGuessesError if result words list is empty', () => {
            const emptyWordList = new WordList([]);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(emptyWordList);
            const testCall = () => nextWordGuesserStrategy.guessNextWordAndScore();
            expect(testCall).toThrow(NoMoreGuessesError);
        });

        it('returns the next highest-score guess if result word list is not empty', () => {
            const wordList = new WordList(['AAAAA', 'BBBBB', 'CCCCC']);
            const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(wordList);
            const wordsWithScores = wordList.words.map((word) => ({
                word,
                score: nextWordGuesserStrategy.scoreForGuess(word)
            }));
            const expected = maxBy(wordsWithScores, 'score');

            const result = nextWordGuesserStrategy.guessNextWordAndScore();

            expect(result).toStrictEqual(expected);
        });
    });

    describe(NextWordGuesserStrategyBase.prototype.getAlreadyGuessedLetters, () => {
        const wordList = new WordList();
        const nextWordGuesserStrategy = new NextWordGuesserStrategyBaseTest(wordList);
        it('returns a flattened list of unique guessed letters', () => {
            const previousGuesses: WordGuessAndResult[] = [
                { word: 'ABC', result: [] },
                { word: 'BCD', result: [] },
                { word: 'DEF', result: [] },
                { word: 'GHI', result: [] }
            ];
            const expected = uniq(previousGuesses.flatMap((guess) => Array.from(guess.word)));
            const previousGuessesSpy = jest
                .spyOn(nextWordGuesserStrategy, 'previousGuesses', 'get')
                .mockReturnValueOnce(previousGuesses);

            const result = nextWordGuesserStrategy.getAlreadyGuessedLetters();

            expect(previousGuessesSpy).toHaveBeenCalledOnce();
            expect(result).toStrictEqual(expected);
        });
    });
});
