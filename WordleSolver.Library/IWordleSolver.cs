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

using WordleSolver.Library.GuesserStrategies;

namespace WordleSolver.Library;

/// <summary>
/// The interface prototype for the WordleSolver methods.
/// </summary>
public interface IWordleSolver
{

    /// <summary>
    /// List of instantiated guesser strategies.
    /// </summary>
    public List<INextWordGuesserStrategy> GuesserStrategies { get; }

    /// <summary>
    /// The word list from which the guesser strategies are initiated.
    /// </summary>
    public IWordList CandidateWordList { get; }

    /// <summary>
    /// Applies the previous guess to each guesser strategy and appends the resulting rules to the ongoing solver.
    /// </summary>
    /// <param name="previousGuessAndResult">The previous guess and its resulting list of letter rules.</param>
    /// <returns>The calling object after modification.</returns>
    public IWordleSolver WithPreviousGuess(WordGuessAndResult previousGuessAndResult);

    /// <summary>
    /// From all of the guesser strategies, returns the highest-scoring word along with its corresponding score and
    /// the name of the guesser strategy which gave that score.
    /// </summary>
    /// <returns>The highest-scoring guess and its score with the name of the used guesser strategy.</returns>
    public (string GuesserStrategy, WordGuessAndScore GuessAndScore) GuessNextWord();

    /// <summary>
    /// Determines if the ongoing solver is solved by at least one guessing strategy (i.e., there is exactly one
    /// possible word remaining).
    /// </summary>
    /// <returns>True if at least one of the guesser strategies is solved; false otherwise.</returns>
    public bool IsSolved();

    /// <summary>
    /// Determines if the ongoing solver has a solution by every guessing strategy (i.e., there is at least one
    /// possible word remaining).
    /// </summary>
    /// <returns>True if all of the guesser strategies have a solution; false otherwise.</returns>
    public bool HasSolution();

    /// <summary>
    /// Determines which positions are already solved (i.e., have an associated Mandatory letter rule).
    /// </summary>
    /// <returns>A list of (0-based) positions that are already solved.</returns>
    public List<LetterWithPosition> SolvedPositions();

    public List<string> PreviousGuesses();
}
