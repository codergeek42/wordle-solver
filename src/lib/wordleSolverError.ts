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

/**
 * A generic error in the Wordle Solver, not captured by some more specific
 * exception type.
 */
export class WordleSolverError extends Error {}

/**
 * A `LetterAtPositionInWordRule` was attempted to be processed that required a `position`
 * property but did not have it.
 */
export class MissingPositionError extends WordleSolverError {}

/**
 * There are no more possible guesses (i.e., the set of possible words has become empty).
 */
export class NoMoreGuessesError extends WordleSolverError {}

/**
 * An error that occurs in the test helpers, such as `generateAlphabetWords`.
 */
export class WordleSolverTestError extends WordleSolverError {}
