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

import { sumBy } from 'lodash';
import NextWordGuesserStrategyBase from '../nextWordGuesserStrategy';
import WordList from '../wordList';
import { LetterAtPositionInWord, LetterAtPositionInWordRule } from '../letterAtPosition';

/**
 * A guessing strategy which scores the candidate words based on the letter frequency at each position.
 */
export default class PerLetterEliminationStrategy extends NextWordGuesserStrategyBase {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    /**
     * Counts the number of total possible letters (with repetition) in the given word list.
     *
     * @param wordList - the word list from which to count possible letters.
     * @returns the count of total possible letters in the given word list.
     */
    static totalPossibleLettersInWordList(wordList: WordList): number {
        return sumBy(wordList.possibleLetters, 'length');
    }

    /**
     * Maps the given guess to an array of `LetterAtPositionInWordRule` with `required` set to `Misplaced`.
     *
     * @param guess - the candidate guess
     * @returns an array of rules with each letter at its position in `guess` marked as `Misplaced`.
     */
    static generateGuessPositionLetterRules(guess: string): LetterAtPositionInWordRule[] {
        return Array.from(guess).map((letter, position) => ({
            position,
            letter,
            required: LetterAtPositionInWord.Misplaced
        }));
    }

    /**
     * Scores the candidate word based on the letter frequency at each position, where the resulting score
     * for a given guess is the number of position-letter combinations that the current solver rules have not
     * specified that the given guess could potentially eliminate.
     *
     * For example, if only `GUESS` has previously been guessed and all of the letters had an `Impossible` result,
     * then a guess of `GUEST` would only score 1 because that could only potentially eliminate one letter-position
     * pair (T at position 5), and all of G, U, E, and S are already known to be nowhere in the word; whereas a guess
     * of `SEGUE` would also score 5 because it would potentially eliminate each of the `G`, `U`, `E`, and `S` at
     * positions different from `GUESS`.
     *
     * @param guess - the candidate guess
     * @returns the number of position-letter combinations that `guess` could eliminate.
     */
    scoreForGuess(guess: string): number {
        const previousScore = PerLetterEliminationStrategy.totalPossibleLettersInWordList(this.wordList);
        const guessWordList = this.wordList.withPositionLetterRules(
            PerLetterEliminationStrategy.generateGuessPositionLetterRules(guess)
        );
        const guessScore = PerLetterEliminationStrategy.totalPossibleLettersInWordList(guessWordList);
        return previousScore - guessScore;
    }
}
