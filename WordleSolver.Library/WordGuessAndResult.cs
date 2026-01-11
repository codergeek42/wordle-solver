/*
 * WordleSolver: A clever algorithm and automated tool to solve the
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
 * along with this program, namely the "LICENSE.txt" text file.  If not,
 * see <https://www.gnu.org/licenses/gpl-3.0.html>.
 */

namespace WordleSolver.Library;

/// <summary>
/// A pairing of a guessed word with its associated score.
/// </summary>
/// <param name="Word">The guessed word.</param>
/// <param name="Score">The score for the word.</param>
public record WordGuessAndScore(string Word, double Score);

/// <summary>
/// A pairing of guessed word and its resulting letter-position rules.
/// </summary>
/// <param name="Word">The guessed word.</param>
/// <param name="Result">The resulting letter-position rules.</param>
/// <param name="WasValidGuess">Whether or not the guess was valid.</param>
public record WordGuessAndResult(string Word, List<LetterAtPositionInWordRule> Result, bool WasValidGuess = true);