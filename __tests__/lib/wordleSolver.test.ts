import { get, keys, mapValues, partialRight } from 'lodash';
import { LetterAtPositionInWord } from '../../src/lib/letterAtPosition';
import { WordGuessAndResult, WordGuessAndScore } from '../../src/lib/wordGuessAndResult';
import { GuesserStrategies, WordleSolver } from '../../src/lib/wordleSolver';
import WordList from '../../src/lib/wordList';
import {
    INextWordGuesserStrategyBase,
    IStrategyScoreMethod,
    NextWordGuesserStrategyBase
} from '../../src/lib/nextWordGuesserStrategy';
import 'jest-extended';
import DistinctLettersStrategy from '../../src/lib/guesserStrategies/distinctLettersStrategy';
import RetryMisplacedLettersStrategy from '../../src/lib/guesserStrategies/retryMisplacedLettersStrategy';
import PerLetterEliminationStrategy from '../../src/lib/guesserStrategies/perLetterEliminationStrategy';
import LetterFrequencyStrategy from '../../src/lib/guesserStrategies/letterFrequencyStrategy';

describe(WordleSolver, () => {
    let wordList: WordList;
    let wordleSolver: WordleSolver;
    let wordleSolverStrategySpies: Record<keyof GuesserStrategies, jest.SpyInstance>;
    let wordleSolverStrategyTypes: Record<keyof GuesserStrategies, typeof NextWordGuesserStrategyBase>;

    function _guesserStrategiesSpyOn<GuesserStrategyType extends INextWordGuesserStrategyBase & IStrategyScoreMethod>(
        method: keyof GuesserStrategyType,
        attributeMethod?: 'get' | 'set'
    ): Record<keyof GuesserStrategies, jest.SpyInstance> {
        const spyMethod = attributeMethod
            ? partialRight(jest.spyOn, method, attributeMethod)
            : partialRight(jest.spyOn, method);
        return mapValues(wordleSolver.guesserStrategies, (s, _name) => spyMethod(s));
    }

    beforeEach(() => {
        wordList = new WordList(['TEST']);
        wordleSolver = new WordleSolver(wordList);
        wordleSolverStrategyTypes = {
            distinctLetters: DistinctLettersStrategy,
            retryMisplacedLetters: RetryMisplacedLettersStrategy,
            perLetterEliminations: PerLetterEliminationStrategy,
            letterFrequency: LetterFrequencyStrategy
        };

        jest.clearAllMocks();
    });

    describe('constructor', () => {
        it('can be instantiated with solver strategies', () => {
            expect(wordleSolver).toBeDefined();
            expect(wordleSolver.guesserStrategies).toBeDefined();
            keys(wordleSolverStrategyTypes).forEach((strategy) => {
                const guesserStrategy = get(wordleSolver.guesserStrategies, strategy);
                const expectedStrategyType = get(wordleSolverStrategyTypes, strategy);
                expect(guesserStrategy).toBeDefined();
                expect(guesserStrategy).toBeInstanceOf(expectedStrategyType);
            });
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
