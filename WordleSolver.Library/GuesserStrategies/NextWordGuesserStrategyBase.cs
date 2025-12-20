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

    public HashSet<char> GetAlreadyGuessedLetters()
    {
        return PreviousGuesses.SelectMany(guess => guess.Word).ToHashSet();
    }

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


    public bool IsSolved()
    {
        return CandidateWordList.Words.Count == 1;
    }

    public bool HasSolution()
    {
        return CandidateWordList.Words.Count >= 1;
    }

    public INextWordGuesserStrategy WithPreviousGuess(WordGuessAndResult guessAndResult)
    {
        CandidateWordList.ProcessExclusionsFromRules(guessAndResult.Result);
        PreviousGuesses.Add(guessAndResult);
        return this;

    }
}