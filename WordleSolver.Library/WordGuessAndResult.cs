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
/// <param name="word">The guessed word.</param>
/// <param name="score">The score for the word.</param>
public class WordGuessAndScore(string word, double score)
{
    /// <summary>
    /// The guessed word.
    /// </summary>
    public string Word { get; set; } = word;

    /// <summary>
    /// The score for the guessed word.
    /// </summary>
    public double Score { get; set; } = score;
}


/// <summary>
/// A pairing of guessed word and its resulting letter-position rules.
/// </summary>
/// <param name="word">The guessed word.</param>
/// <param name="result">The resulting letter-position rules.</param>
public class WordGuessAndResult(string word, List<LetterAtPositionInWordRule> result)
{
    /// <summary>
    /// The guessed word.
    /// </summary>
    public string Word { get; set; } = word;

    /// <summary>
    /// The letter-position rules resulting from the guess.
    /// </summary>
    public List<LetterAtPositionInWordRule> Result { get; set; } = result;
}
