import { mapValues } from 'lodash';
import DistinctLettersStrategy from '../../src/lib/guesserStrategies/distinctLettersStrategy';
import RetryMisplacedLettersStrategy from '../../src/lib/guesserStrategies/retryMisplacedLettersStrategy';
import { LetterAtPositionInWord } from '../../src/lib/letterAtPosition';
import { WordGuessAndResult, WordGuessAndScore } from '../../src/lib/wordGuessAndResult';
import { GuesserStrategies, WordleSolver } from '../../src/lib/wordleSolver';
import WordList from '../../src/lib/wordList';

describe(WordleSolver, () => {
    let wordList: WordList;
    let wordleSolver: WordleSolver;
    let wordleSolverStrategySpies: Record<keyof GuesserStrategies, jest.SpyInstance>;

    beforeEach(() => {
        wordList = new WordList(['TEST']);
        wordleSolver = new WordleSolver(wordList);
        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated with solver strategies', () => {
            expect(wordleSolver).toBeDefined();
            expect(wordleSolver.guesserStrategies).toBeDefined();
            expect(wordleSolver.guesserStrategies.distinctLetters).toBeInstanceOf(DistinctLettersStrategy);
            expect(wordleSolver.guesserStrategies.retryMisplacedLetters).toBeInstanceOf(RetryMisplacedLettersStrategy);
        });
    });

    describe(WordleSolver.prototype.withPreviousGuess, () => {
        it('is called on each guesser strategy', () => {
            const previousGuess: WordGuessAndResult = {
                word: 'ABCD',
                result: [
                    {
                        letter: 'A',
                        required: LetterAtPositionInWord.Impossible
                    }
                ]
            };
            wordleSolverStrategySpies = mapValues(wordleSolver.guesserStrategies, (guesserStrategy) =>
                jest.spyOn(guesserStrategy, 'withPreviousGuess').mockReturnThis()
            );

            const result = wordleSolver.withPreviousGuess(previousGuess);

            mapValues(wordleSolverStrategySpies, (withPreviousGuessSpy) => {
                expect(withPreviousGuessSpy).toHaveBeenCalledExactlyOnceWith(previousGuess);
            });
            expect(result).toStrictEqual(wordleSolver);
        });
    });

    describe(WordleSolver.prototype.guessNextWord, () => {
        it('is called on each solver strategy', () => {
            const nextGuessAndScore: WordGuessAndScore = {
                word: 'TEST',
                score: 42
            };
            const wordleSolverStrategySpies = mapValues(wordleSolver.guesserStrategies, (guesserStrategy) =>
                jest.spyOn(guesserStrategy, 'guessNextWordAndScore').mockReturnValueOnce(nextGuessAndScore)
            );

            const result = wordleSolver.guessNextWord();

            mapValues(wordleSolverStrategySpies, (withPreviousGuessSpy) => {
                expect(withPreviousGuessSpy).toHaveBeenCalledOnce();
            });
            expect(result).toStrictEqual(
                mapValues(wordleSolver.guesserStrategies, (_guesserStrategy) => nextGuessAndScore)
            );
        });
    });
});
