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
/// Base guesser strategy implementation, of common methods & logic. Guesser strategies should subclass this instead
/// of implementing INextWordGuesserStrategy directly.
/// </summary>
/// <param name="wordList">The word list from which to initialize the guesser strategy.</param>
public abstract class NextWordGuesserStrategyBase(IWordList wordList) : INextWordGuesserStrategy
{
    public List<WordGuessAndResult> PreviousGuesses { get; private set; } = new([]);
    public IWordList CandidateWordList { get; private set; } = wordList;

    /// <summary>
    /// Calculates the score for the candidate guess based on the guesser strategy's metric.
    /// Guesser strategies must each implement this method. 
    /// </summary>
    /// <param name="guess">The candidate word.</param>
    /// <returns>The score for the guess based on the guessing strategy in use.</returns>
    public abstract double ScoreForGuess(string guess);

    /// <summary>
    /// Override this with a strategy-specific definition to determines whether the guesser strategy should be used
    /// for scoring the current iteration.
    /// </summary>
    /// <returns>True if it should be used; False otherwise.</returns>
    public virtual bool ShouldRun()
    {
        return true;
    }

    /// <summary>
    /// Returns the set of all guessed letters.
    /// </summary>
    /// <returns>The set of all guessed letters.</returns>
    public HashSet<char> GetAlreadyGuessedLetters()
    {
        return PreviousGuesses.SelectMany(guess => guess.Word).ToHashSet();
    }

    /// <summary>
    /// Applies the guesser strategy's score metric to every word and returns the highest-scoring one.
    /// </summary>
    /// <returns>The highest-scoring word and guess.</returns>
    /// <exception cref="NoMoreGuessesException">If the guesser strategy does not have a solution.</exception>
    public WordGuessAndScore GuessNextWordAndScore()
    {
        if (!HasSolution())
        {
            throw new NoMoreGuessesException();
        }
        return CandidateWordList.Words
            .AsParallel()
            .Select(word => new WordGuessAndScore(word, ScoreForGuess(word)))
            // NB: Not null because the word list is certainly non-empty here, as
            // a NoMoreGuessesException would be thrown earlier otherwise.
            .MaxBy(wordWithScore => wordWithScore.Score)!;
    }

    /// <summary>
    /// Determines if the guesser strategy is solved (i.e., has exactly one candidate word remaining).
    /// </summary>
    /// <returns>True if the guesser strategy has exactly one word remaining; False otherwise.</returns>
    public bool IsSolved()
    {
        return CandidateWordList.Words.Count == 1;
    }

    /// <summary>
    /// Determines if the guesser strategy has a solution (i.e., has at least one candidate word remaining).
    /// </summary>
    /// <returns>True if the guesser strategy has at least one word remaining; False otherwise.</returns>
    public bool HasSolution()
    {
        return CandidateWordList.Words.Count >= 1;
    }

    /// <summary>
    /// Processes the resulting letter-position rules from the previous guess and stores the guess, then
    /// returns the calling guesser strategy.
    /// </summary>
    /// <param name="guessAndResult">The guess and its associated result.</param>
    /// <returns>The caller.</returns>
    public virtual INextWordGuesserStrategy WithPreviousGuess(WordGuessAndResult guessAndResult)
    {
        if (guessAndResult.WasValidGuess)
        {
            CandidateWordList.ProcessExclusionsFromRules(guessAndResult.Result);
            PreviousGuesses.Add(guessAndResult);
        }
        else
        {
            CandidateWordList = CandidateWordList.WithExcludedWords([guessAndResult.Word]);
        }
        return this;
    }


}
