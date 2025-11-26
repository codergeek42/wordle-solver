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

using WordleSolver.Library.Extensions;

namespace WordleSolver.Library.GuesserStrategies;

public class LetterFrequencyGuesserStrategy : NextWordGuesserStrategyBase
{
	public LetterFrequencyGuesserStrategy(IWordList wordList) : base(wordList)
	{ }

	public override double ScoreForGuess(string guess)
	{
		List<Dictionary<char, int>> LettersCount = CandidateWordList.CountLetters();
		var enumerated = guess.Enumerate();
		// Needs double cast to force floating-point division.
		var summed = enumerated.Sum(lwp => (double)LettersCount[lwp.Position][lwp.Letter] / CandidateWordList.Words.Count);
		return summed;
	}
}