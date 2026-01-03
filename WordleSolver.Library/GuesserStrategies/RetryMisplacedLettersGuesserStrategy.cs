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

/// <summary>
/// A guessing strategy that scores each guess by the number of previously-Misplaced letters that are retried
/// at different positions.
/// </summary>
/// <param name="wordList">The word list from which to initialize the guesser strategy.</param>
public class RetryMisplacedLettersGuesserStrategy(IWordList wordList) : NextWordGuesserStrategyBase(wordList)
{
    /// <summary>
    /// Only run this guesser strategy if there is at least one already-Misplaced guess.
    /// </summary>
    /// <returns>True if there is at least one Misplaced letter guessed; False otherwise.</returns>
    public override bool ShouldRun()
    {
        return PreviousGuesses
            .SelectMany(guessAndResult => guessAndResult.Result)
            .Any(letterPositionRule => letterPositionRule.Required == LetterAtPositionInWord.Misplaced);
    }


    /// <summary>
    /// Scores the guess by the number of retried misplaced letters, i.e. the number of letters in the guess
    /// that previous rules have determined are Misplaced at other position(s).
    /// 
    /// For example: if only `STONE` was guessed so far and all five letters were Misplaced, then a candidate guess
    /// of `NOTES` would score the highest (5), as all five letters are used in difference positions; but `ATONE`
    /// would score the lowest of 0 (because all of `T`, `O`, `N`, and `E` are not in other positions and `A` was not
    /// already guessed in any position. `STENO` would score between them at 2, because only its `E` and `O` are
    /// retried in different positions, but `S`, T`, and `N` are guessed at their same positions.
    /// </summary>
    /// <param name="guess"></param>
    /// <returns></returns>
    public override double ScoreForGuess(string guess)
    {
        IEnumerable<LetterWithPosition> previouslyMisplacedLetters = CandidateWordList.LetterRules
            .Where(rule => rule.Required == LetterAtPositionInWord.Misplaced)
            // Position is not null because non-Impossible rules must have a Position, validated by
            // WordList.ProcessExclusionsFromRules as the guess is added and by the
            // LetterAtPositionInWordRule constructor.
            .Select(rule => new LetterWithPosition(rule.Letter, rule.Position!.Value));

        return guess.Enumerate()
            .Count(guessedLetterWithPosition =>
                previouslyMisplacedLetters.Any(previouslyMisplaced =>
                    previouslyMisplaced.Letter == guessedLetterWithPosition.Letter &&
                    previouslyMisplaced.Position != guessedLetterWithPosition.Position
            ));
    }
}
