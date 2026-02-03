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

using System.Diagnostics;

using WordleSolver.Library.GuesserStrategies;

namespace WordleSolver.Library;

/// <summary>
/// The core Wordle-solving library, storing a list of guesser strategies and applying solving iterations to each
/// simultaneously.
/// </summary>
/// <param name="wordList">The word list from which to initialize the solver.</param>
/// <param name="guesserStrategyFactory">Factory for creating the list of guesser stratgies.</param>
public class WordleSolver(IWordList wordList, INextWordGuesserStrategyFactory guesserStrategyFactory) : IWordleSolver
{

    /// <summary>
    /// The list of guesser stratgies.
    /// </summary>
    public List<INextWordGuesserStrategy> GuesserStrategies { get; private set; } = guesserStrategyFactory.FromWordList(wordList);

    /// <summary>
    /// The list of potential words remaining.
    /// </summary>
    public IWordList CandidateWordList { get; private set; } = wordList;

    /// <summary>
    /// Applies the previous guess to each guessing strategy and returns the calling (modified) solver instance.
    /// </summary>
    /// <param name="previousGuessAndResult">The previous guessed word and its resulting letter-position rules.</param>
    /// <returns>The modified <see cref="WordleSolver"/> instance.</returns>
    public IWordleSolver WithPreviousGuess(WordGuessAndResult previousGuessAndResult)
    {
        GuesserStrategies = GuesserStrategies
            .AsParallel()
            .Select(guesserStrategy => guesserStrategy.WithPreviousGuess(previousGuessAndResult))
            .ToList();
        return this;
    }

    /// <summary>
    /// Scores every candidate word with every guesser strategy and finds the highest-scoring one.
    /// </summary>
    /// <returns>The name of the guesser strategy and the highest-scoring next word to guess.</returns>
    public (string GuesserStrategy, WordGuessAndScore GuessAndScore) GuessNextWord()
    {
        // TODO: add priority for immediate guesser strategy guess if it  IsSolved()
        var result = GuesserStrategies
            .Where(guesserStrategy => guesserStrategy.ShouldRun())
            .AsParallel()
            .Select(guesserStrategy =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var guesserStrategyAndScore = (
                    GuesserStrategy: guesserStrategy.GetType().Name,
                    GuessAndScore: guesserStrategy.GuessNextWordAndScore()
                );
                stopwatch.Stop();
                Console.WriteLine($"Potential guess: {guesserStrategyAndScore.GuesserStrategy} \t: {guesserStrategyAndScore.GuessAndScore.Word} = {guesserStrategyAndScore.GuessAndScore.Score,6:F3} ({stopwatch.Elapsed.TotalSeconds,7:F3} seconds)");
                return guesserStrategyAndScore;
            })
            .MaxBy(guesserStrategy => guesserStrategy.GuessAndScore.Score);
        return result;
    }


    /// <summary>
    /// Determines if the guesser strategies together are solved (i.e., at least one of them has exactly one candidate
    /// word remaining).
    /// </summary>
    /// <returns>True if at least one guesser strategy is solved; False otherwise.</returns>
    public bool IsSolved()
    {
        return GuesserStrategies.Any(guesserStrategy => guesserStrategy.IsSolved());
    }


    /// <summary>
    /// Determines if the guesser strategies together still have a solution (i.e., all them have at least one candidate
    /// word remaining).
    /// </summary>
    /// <returns>True if all guesser strategies have a solution; False otherwise.</returns>
    public bool HasSolution()
    {
        return GuesserStrategies.All(guesserStrategy => guesserStrategy.HasSolution());
    }

    /// <summary>
    /// Determines which positions are already solved (i.e., have an associated Mandatory letter rule).
    /// </summary>
    /// <returns>A list of (0-based) positions that are already solved.</returns>
    public List<LetterWithPosition> SolvedPositions()
    {
        return GuesserStrategies
            .SelectMany(guesserStrategy =>
                guesserStrategy.PreviousGuesses
                    .Where(guessAndResult => guessAndResult.WasValidGuess)
                    .SelectMany(previousGuesses => previousGuesses.Result)
            )
            .Where(rule => rule.Required == LetterAtPositionInWord.Mandatory)
            .Select(rule => new LetterWithPosition(rule.Letter, (int)rule.Position!))
            .ToList();
    }

    public List<string> PreviousGuesses()
    {
        return GuesserStrategies
            .SelectMany(guesserStrategy => guesserStrategy.PreviousGuesses)
            .Select(previousGuesses => previousGuesses.Word)
            .ToList();
    }
}
