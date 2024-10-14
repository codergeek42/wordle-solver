import { mapValues } from 'lodash';
import DistinctLettersStrategy from './guesserStrategies/distinctLettersStrategy';
import { WordGuessAndResult, WordGuessAndScore } from './wordGuessAndResult';
import WordList from './wordList';
import RetryMisplacedLettersStrategy from './guesserStrategies/retryMisplacedLettersStrategy';
import PerLetterEliminationStrategy from './guesserStrategies/perLetterEliminationStrategy';
import LetterFrequencyStrategy from './guesserStrategies/letterFrequencyStrategy';

export type GuesserStrategies = {
    distinctLetters: DistinctLettersStrategy;
    retryMisplacedLetters: RetryMisplacedLettersStrategy;
    perLetterEliminations: PerLetterEliminationStrategy;
    letterFrequency: LetterFrequencyStrategy;
};

export class WordleSolver {
    private myGuesserStrategies: GuesserStrategies;

    constructor(protected myWordList: WordList) {
        this.myGuesserStrategies = {
            distinctLetters: new DistinctLettersStrategy(myWordList),
            retryMisplacedLetters: new RetryMisplacedLettersStrategy(myWordList),
            perLetterEliminations: new PerLetterEliminationStrategy(myWordList),
            letterFrequency: new LetterFrequencyStrategy(myWordList)
        };
    }

    withPreviousGuess(candidate: WordGuessAndResult): this {
        this.myGuesserStrategies = mapValues(this.myGuesserStrategies, (guesserStrategy) =>
            guesserStrategy.withPreviousGuess(candidate)
        );
        return this;
    }

    guessNextWord(): Record<keyof GuesserStrategies, WordGuessAndScore> {
        const nextGuesses = mapValues(this.myGuesserStrategies, (guesserStrategy) =>
            guesserStrategy.guessNextWordAndScore()
        );
        return nextGuesses;
    }

    get guesserStrategies(): GuesserStrategies {
        return this.myGuesserStrategies;
    }
}
