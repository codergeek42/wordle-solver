import { times } from 'lodash';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';
import generateAlphabetWords from '../../../src/lib/helpers/generateAlphabetWords';
import RetryMisplacedLettersStrategy from '../../../src/lib/guesserStrategies/retryMisplacedLettersStrategy';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';
import { LetterAtPositionInWord, LetterAtPositionInWordRule } from '../../../src/lib/letterAtPosition';

describe(RetryMisplacedLettersStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList(['TEST']);
            const retryMisplacedLettersStrategy = new RetryMisplacedLettersStrategy(wordList);

            expect(retryMisplacedLettersStrategy).toBeDefined();
            expect(retryMisplacedLettersStrategy.wordList).toBeInstanceOf(WordList);
            expect(retryMisplacedLettersStrategy.scoreForGuess).toBeFunction();
            expect(retryMisplacedLettersStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(retryMisplacedLettersStrategy).toBeInstanceOf(RetryMisplacedLettersStrategy);
        });
    });

    describe(RetryMisplacedLettersStrategy.prototype.scoreForGuess, () => {
        const numMisplacedLettersCases = [2]; // range(0, WordLength + 1)
        it.each(numMisplacedLettersCases)(
            'scores the guess based on the number of previously misplaced letters (%i misplaced)',
            (numMisplacedLetters) => {
                const alphabet = generateAlphabetOfLength(numMisplacedLetters + 3);
                const misplacedAlphabet = alphabet.slice(-1).concat(alphabet.slice(0, alphabet.length - 1));
                const wordList = new WordList(generateAlphabetWords(alphabet, 2));
                const retryMisplacedLettersStrategy = new RetryMisplacedLettersStrategy(wordList);

                const impossibleLetterRule: LetterAtPositionInWordRule = {
                    required: LetterAtPositionInWord.Impossible,
                    letter: alphabet.slice(-3, -2)
                };

                const nonmatchingMisplacedLetterRule: LetterAtPositionInWordRule = {
                    required: LetterAtPositionInWord.Misplaced,
                    letter: alphabet.slice(-2, -1),
                    position: numMisplacedLetters + 1
                };

                const previouslyMisplacedLetterRules: LetterAtPositionInWordRule[] = times(
                    numMisplacedLetters,
                    (position) => ({
                        position,
                        required: LetterAtPositionInWord.Misplaced,
                        letter: alphabet[position]
                    })
                );

                const wordListLetterRulesSpy = jest
                    .spyOn(wordList, 'letterRules', 'get')
                    .mockReturnValueOnce([
                        impossibleLetterRule,
                        nonmatchingMisplacedLetterRule,
                        ...previouslyMisplacedLetterRules
                    ]);

                const guess = misplacedAlphabet.slice(0, numMisplacedLetters + 1);

                const result = retryMisplacedLettersStrategy.scoreForGuess(guess);

                expect(wordListLetterRulesSpy).toHaveBeenCalledOnce();
                expect(result).toStrictEqual(numMisplacedLetters);
            }
        );
    });
});
