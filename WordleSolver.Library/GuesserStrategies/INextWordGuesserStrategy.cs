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

namespace WordleSolver.Library.GuesserStrategies;

/// <summary>
/// The common interface for guesser strategies.
/// </summary>
public interface INextWordGuesserStrategy
{
    /// <summary>
    /// The list of previous guesses and their corresponding results (exclusion rules).
    /// </summary>
    public List<WordGuessAndResult> PreviousGuesses { get; }

    /// <summary>
    /// The list of candidate words that are still possible.
    /// </summary>
    public IWordList CandidateWordList { get; }

    /// <summary>
    /// Stores the previous guess and result, then returns the caller for fluent chaining.
    /// </summary>
    /// <param name="guessAndResult">The previous guess and its resulting exclusion rules.</param>
    /// <returns>The calling object.</returns>
    public INextWordGuesserStrategy WithPreviousGuess(WordGuessAndResult guessAndResult);

    /// <summary>
    /// Applies the guesser strategy's score metric to every candidate word and returns the maximum word with score.
    /// </summary>
    /// <returns>The highest-scoring word and its score.</returns>
    public WordGuessAndScore GuessNextWordAndScore();

    /// <summary>
    /// Get the set of already-guessed letters.
    /// </summary>
    /// <returns>The set of already guessed letters.</returns>
    public HashSet<char> GetAlreadyGuessedLetters();


    /// <summary>
    /// Determines if the guesser strategy has narrowed the candidate words to exactly one solution.
    /// </summary>
    /// <returns>True if the guessing is solved for this strategy, i.e. exactly one candidate word remains; false
    /// otherwise.</returns>
    public bool IsSolved();

    /// <summary>
    /// Determines if the guesser strategy still has some solution.
    /// </summary>
    /// <returns>True if this strategy still has a solution, i.e. at least one candidate word remains; false
    /// otherwise.</returns>
    public bool HasSolution();
}
