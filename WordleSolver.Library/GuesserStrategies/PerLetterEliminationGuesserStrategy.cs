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
/// A guesser strategy that scores each guess based on the relative letter frequency at each position, namely
/// how many letter-position pairings the guess could potentially eliminate.
/// </summary>
/// <param name="wordList">The word list from which to initialize the guesser strategy.</param>
public class PerLetterEliminationGuesserStrategy(IWordList wordList) : NextWordGuesserStrategyBase(wordList)
{
    /// <summary>
    /// Scores the candidate word by the relative letter frequency at each position, calculating the number of
    /// position-letter pairings that the current solver rules have not specified that the given guess could
    /// potentially eliminate.
    /// 
    /// For example, if only `GUESS` has previously been guessed and all of the letters had a `Misplaced` result, then
    /// a guess of `GUEST` would score very low because that could only eliminate one letter-position pair (T at
    /// position 5), as all of `G`, `U`, `E`, and `S` are already known to not be in the word at those positions;
    /// whereas a guess of `SEGUE` would score much higher because it could potentially eliminate each of the `G`,
    /// `U`, `E`, and `S` at positions different from `GUESS`.
    /// </summary>
    /// <param name="guess">The candidate word.</param>
    /// <returns>The total count of letter-position pair eliminations that could be made from the guess.</returns>
    public override double ScoreForGuess(string guess)
    {
        int previousScore = GetTotalPossibleLettersInWordList(CandidateWordList);
        int simulatedGuessAllLetterScore = GetTotalPossibleLettersInWordList(
            CandidateWordList.WithPositionLetterRules(GenerateGuessPositionLetterRules(guess))
        );
        return previousScore - simulatedGuessAllLetterScore;
    }

    /// <summary>
    /// Gets the total count of possible letters in the given word list.
    /// </summary>
    /// <param name="wordList">The word list.</param>
    /// <returns>The total count of possible letters in the given word list (sum of possible letter counts at
    /// each position).</returns>
    public static int GetTotalPossibleLettersInWordList(IWordList wordList)
    {
        return wordList.PossibleLetters.Sum(possibleLetters => possibleLetters.Count);
    }

    /// <summary>
    /// Generates a list of position ruls with each letter is marked as Misplaced at its corresponding position.
    /// </summary>
    /// <param name="guess"></param>
    /// <returns>A list of the generated letter exclusion rules.</returns>
    public static List<LetterAtPositionInWordRule> GenerateGuessPositionLetterRules(string guess)
    {
        return guess.Enumerate().ConvertAll(lwp => new LetterAtPositionInWordRule(lwp.Position, lwp.Letter, LetterAtPositionInWord.Misplaced));
    }

    /// <summary>
    /// To help increase reproducibility of results and improve initial performance, only run this guesser strategy if
    /// there is at least one guess already made. Otherwise, almost every word gives a score of 5 or more, leading to
    /// one of them essentially being chosen at random (through whichever of the same value `MaxBy` takes).
    /// </summary>
    /// <returns>True if there is at least one valid guess already made; False otherwise.</returns>
    public override bool ShouldRun()
    {
        return PreviousGuesses.Count(previousGuess => previousGuess.WasValidGuess) > 0;
    }

}
