/*
 * wordle-solver: A clever algorithm and automated tool to solve the
 * 	NYTimes daily Wordle puzzle game.
 * Copyright (C) 2023 Peter Gordon <codergeek42@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, namely the "LICENSE" text file.  If not,
 * see <https://www.gnu.org/licenses/gpl-3.0.html>.
 */

import { maxBy, uniq } from 'lodash';
import { WordGuessAndResult, WordGuessAndScore } from './wordGuessAndResult';
import WordList from './wordList';
import { NoMoreGuessesError } from './wordleSolverError';

/**
 * A base class for the guesser strategies, consolidating shared code.
 *
 * @example For calculating the score of a guess based on its ASCII character values
 * ```typescript
 *  class AsciiValueGuesserStrategy extends NextWordGuesserStrategyBase {
 *      constructor(...ctorParams: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
 *          super(...ctorParams);
 *      }
 *
 *      scoreForGuess(guess: string): number {
 *          return sumBy(guess, (chr) => chr.charCodeAt(0));
 *      }
 *  }
 * ```
 * ...and then using this to suggest guesses:
 * ```typescript
 *  const wordList = new wordList(methodThatReturnsAListOfWords())
 *  const asciiValueGuesser = new AsciiValueGuesserStrategy(wordList);
 *  const asciiValueGuesserAfterGuessingHELLO = asciiValueGuesser.withPreviousGuess({
 *      word: 'HELLO',
 *      result: someFunctionThatYieldsExclusionAndMandateRulesFromGuess('HELLO')
 *  });
 *  expect(asciiValueGuesserAfterGuessingHELLO.getAlreadyGuessedLetters()).toEqual(['H', 'E', 'L', 'O']);
 *  console.log('Next optimal word guess after HELLO is: ',
 *      asciiValueGuesserAfterGuessingHELLO.guessNextWordAndScore());
 * ```
 */
export abstract class NextWordGuesserStrategyBase {
    /**
     * Track the previously-guessed words and their resulting exclusion/mandate rules.
     */
    protected myPreviousGuesses: WordGuessAndResult[] = [];

    /**
     *
     * @param myWordList - the `WordList` object to use for choosing the next optimal guess.
     */
    constructor(protected myWordList: WordList) {}

    /**
     * Implement this with a metric for scoring for the given guess, determined by the guessing strategy in use.
     *
     * @param guess - The guess candidate word.
     *
     * @returns A score calculated by a given guessing strategy, roughly equal to the number of letter & position
     *  combinations' worth of information that would be given by this guess in that strategy.
     */
    abstract scoreForGuess(guess: string): number;

    /**
     * Modifies the calling object to procss the given candidate guess and store its resulting rules, such as certain
     *  letters being mandated at or excluded from certain positions.
     *
     * @param candidate - A pairing of the guessed word and its resulting `LetterAtPositionInWordRule` rules.
     *
     * @returns `this`, after having processed the previous guess and its resulting rules.
     */
    withPreviousGuess(candidate: WordGuessAndResult): this {
        this.myWordList.processExclusionsFromRules(candidate.result);
        this.myPreviousGuesses.push(candidate);
        return this;
    }

    /**
     * Calculates the score for all remaining possible candidate words and then returns the highest-scoring one.
     *
     * @returns The next optimal word to guess for this particular strategy.
     *
     * @throws `NoMoreGuessesError`
     * When there are no more guesses possible (i.e., the list of candidate words becomes empty).
     */
    guessNextWordAndScore(): WordGuessAndScore {
        if (this.myWordList.words.length < 1) {
            throw new NoMoreGuessesError();
        }
        const wordsWithGuesses = this.myWordList.words.map(
            (word: string): WordGuessAndScore => ({ word, score: this.scoreForGuess(word) }),
            this
        );

        return maxBy(wordsWithGuesses, 'score') as WordGuessAndScore;
    }

    /**
     * @returns the list of all unique letters already guessed, regardless of position.
     */
    getAlreadyGuessedLetters(): string[] {
        return uniq(this.previousGuesses.flatMap((guess) => Array.from(guess.word)));
    }

    get previousGuesses(): WordGuessAndResult[] {
        return this.myPreviousGuesses;
    }

    get wordList(): WordList {
        return this.myWordList;
    }
}
