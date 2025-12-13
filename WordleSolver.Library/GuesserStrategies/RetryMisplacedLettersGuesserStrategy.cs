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

public class RetryMisplacedLettersGuesserStrategy(IWordList wordList) : NextWordGuesserStrategyBase(wordList)
{
	public override double ScoreForGuess(string guess)
	{
		IEnumerable<LetterWithPosition> previouslyMisplacedLetters = CandidateWordList.LetterRules
			.Where((rule) => rule.Required == LetterAtPositionInWord.Misplaced)
			// Position is not null because non-Impossible rules must have a Position, validated by
			// WordList.ProcessExclusionsFromRules as the guess is added.
			.Select((rule) => new LetterWithPosition(rule.Letter, rule.Position.GetValueOrDefault()));

		return guess.Enumerate()
			.Where((guessedLetterWithPosition) =>
				previouslyMisplacedLetters.Any((previouslyMisplaced) =>
					previouslyMisplaced.Letter == guessedLetterWithPosition.Letter &&
					previouslyMisplaced.Position != guessedLetterWithPosition.Position
			))
			.Count();
	}
}