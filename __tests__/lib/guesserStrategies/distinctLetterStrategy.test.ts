import { range } from 'lodash';
import { WordLength } from '../../../__data__/alphabet';
import DistinctLettersStrategy from '../../../src/lib/guesserStrategies/distinctLettersStrategy';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';
import generateAlphabetWords from '../../../src/lib/helpers/generateAlphabetWords';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';

describe(DistinctLettersStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList(['TEST']);
            const distinctLettersStrategy = new DistinctLettersStrategy(wordList);

            expect(distinctLettersStrategy).toBeDefined();
            expect(distinctLettersStrategy.scoreForGuess).toBeFunction();
            expect(distinctLettersStrategy.wordList).toBeInstanceOf(WordList);
            expect(distinctLettersStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(distinctLettersStrategy).toBeInstanceOf(DistinctLettersStrategy);
        });
    });

    describe(DistinctLettersStrategy.prototype.scoreForGuess, () => {
        const numGuessedLettersCases = range(0, WordLength + 1);
        describe('scores the guess based on the number of distinct unguessed letters in the word', () => {
            describe.each(numGuessedLettersCases)('(%i guessed already)', (numGuessedLetters) => {
                const numDistinctLettersCases = range(0, WordLength);
                it.each(numDistinctLettersCases)('(%i distinct unguessed)', (numDistinctLetters) => {
                    const previousGuessLetters = generateAlphabetOfLength(numGuessedLetters);
                    const distinctGuessLetters = generateAlphabetOfLength(numGuessedLetters + numDistinctLetters);

                    const distinctLettersStrategy = new DistinctLettersStrategy(
                        new WordList(generateAlphabetWords(distinctGuessLetters))
                    );

                    const getAlreadyGuessedLettersSpy = jest
                        .spyOn(distinctLettersStrategy, 'getAlreadyGuessedLetters')
                        .mockReturnValueOnce(Array.from(previousGuessLetters));
                    const result = distinctLettersStrategy.scoreForGuess(distinctGuessLetters);

                    expect(getAlreadyGuessedLettersSpy).toHaveBeenCalledOnce();
                    expect(result).toStrictEqual(numDistinctLetters);
                });
            });
        });
    });
});
