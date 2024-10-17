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

/*
 * TODO: Solve styles / architecture notes to self:
 * 
 * Abstract class with base solver stuff;
 * Strategy methods for score methods implemented by extenders
 * Guesser class for each method which implements strategy, extends base class;
 * Algorithm uses all guessers and picks which one removes most possibilities.
 *
 * 1. guess word by eliminating highest count of distinct unguessed letters (e.g., BREAD (5) > BREED (4) = BLEED (4) > PAPAS (3))
 * 2. guess word by highest count of additional letter+position exclusions (total # eliminations at each position maximized);
 * 3. guess word by highest count of re-used misplaced letters; (if known TSO__ misplaced, try ST_O_, etc.)
 * 4. guess word by highest count by most frequent letters (calculate frequency list of each position, eliminate letters highest on that, total # = metric)

 * then pick from among these which removes the most amount of words total from the candidate list.
 */

export interface INextWordGuesserStrategyBase {
    getAlreadyGuessedLetters(): string[];

    guessNextWordAndScore(): WordGuessAndScore;

    withPreviousGuess(candidate: WordGuessAndResult): this;

    get previousGuesses(): WordGuessAndResult[];
    get wordList(): WordList;
}

export interface IStrategyScoreMethod {
    scoreForGuess(guess: string): number;
}

export abstract class NextWordGuesserStrategyBase implements INextWordGuesserStrategyBase {
    protected myPreviousGuesses: WordGuessAndResult[] = [];

    constructor(protected myWordList: WordList) {}

    abstract scoreForGuess(guess: string): number;

    withPreviousGuess(candidate: WordGuessAndResult) {
        this.myWordList.processExclusionsFromRules(candidate.result);
        this.myPreviousGuesses.push(candidate);
        return this;
    }

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
