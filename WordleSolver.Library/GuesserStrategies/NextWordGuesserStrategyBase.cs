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

public abstract class NextWordGuesserStrategyBase(IWordList wordList) : INextWordGuesserStrategy
{
	public List<WordGuessAndResult> PreviousGuesses { get; private set; } = new([]);
	public IWordList CandidateWordList { get; private set; } = wordList;

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
		List<WordGuessAndScore> wordsWithScores = CandidateWordList.Words
			.ConvertAll(word => new WordGuessAndScore(word, ScoreForGuess(word)));
		// NB: Not null because the word list is certainly non-empty here, as
		// a NoMoreGuessesException would be thrown earlier otherwise.
		return wordsWithScores.MaxBy(wordWithScore => wordWithScore.Score)!;
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