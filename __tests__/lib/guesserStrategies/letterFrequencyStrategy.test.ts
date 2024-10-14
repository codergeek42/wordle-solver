import LetterFrequencyStrategy from '../../../src/lib/guesserStrategies/letterFrequencyStrategy';
import { NextWordGuesserStrategyBase } from '../../../src/lib/nextWordGuesserStrategy';
import WordList from '../../../src/lib/wordList';

describe(LetterFrequencyStrategy, () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated and extends NextWordGuesserStrategyBase class', () => {
            const wordList = new WordList([]);
            const perLetterEliminationStrategy = new LetterFrequencyStrategy(wordList);

            expect(perLetterEliminationStrategy).toBeDefined();
            expect(perLetterEliminationStrategy.wordList).toBeInstanceOf(WordList);
            expect(perLetterEliminationStrategy.scoreForGuess).toBeFunction();
            expect(perLetterEliminationStrategy).toBeInstanceOf(NextWordGuesserStrategyBase);
            expect(perLetterEliminationStrategy).toBeInstanceOf(LetterFrequencyStrategy);
        });
    });

    describe(LetterFrequencyStrategy.prototype.scoreForGuess, () => {
        it('scores the guess based on letter frequency', () => {
            const wordList = new WordList(['AAA', 'BZZ', 'CCZ']);
            const letterFrequencyStrategy = new LetterFrequencyStrategy(wordList);

            const result = Array.from(wordList.words, (word) => ({
                word,
                score: letterFrequencyStrategy.scoreForGuess(word)
            }));
            console.log('@@ res = ', JSON.stringify(result, null, 2));
        });
    });
});
