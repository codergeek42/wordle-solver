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
/// A guessing strategy that scores each guess by the number of distinct unguessed letters, ignoring repetitionss.
/// </summary>
/// <param name="wordList">The word list from which to initialize the guesser strategy.</param>
public class DistinctLettersGuesserStrategy(IWordList wordList) : NextWordGuesserStrategyBase(wordList)
{
    /// <summary>
    /// Only run this guesser strategy if there are any previous guesses; otherwise, it's at best the same
    /// score as PerLetterEliminationStrategy.
    /// </summary>
    /// <returns>True if there is at least one previous guess; False otherwise.</returns>
    public override bool ShouldRun()
    {
        return PreviousGuesses.Count(previousGuess => previousGuess.WasValidGuess) > 0;
    }

    /// <summary>
    /// Scores the guess based on the number of distinct unguessed letters, ignoring repetitions.
    /// For example, if there are no guesses yet, `BREAD` would yield 5 and `BOOKS` would yield 4;
    /// but if `BAKER` had already been guessed before those, then `BREAD` would yield only 1 (D), while
    /// `BOOKS` would yield 2 (O and S).
    /// 
    /// Note that the letters shown in these example results are for demonstration only; and the actual
    /// returned score does not include the information about those letters, only their total count.
    /// </summary>
    /// <param name="guess">The candidate word.</param>
    /// <returns>The number of letters in the given guess that have not previously been guessed.</returns>
    public override double ScoreForGuess(string guess)
    {
        HashSet<char> alreadyGuessedLetters = GetAlreadyGuessedLetters();
        return guess.ToHashSet().Except(alreadyGuessedLetters).Count();
    }

    /// <summary>
    /// Stores the previous guess and resulting rules in order to prioritize guessing more letters
    /// over trying to match already-known ones; then returns the calling guesser strategy.
    /// </summary>
    /// <param name="guessAndResult">The guess and its associated result.</param>
    /// <returns>The caller.</returns>
    public override INextWordGuesserStrategy WithPreviousGuess(WordGuessAndResult guessAndResult)
    {
        return base.WithPreviousGuess(guessAndResult with
        {
            Result = []
        });
    }
}
