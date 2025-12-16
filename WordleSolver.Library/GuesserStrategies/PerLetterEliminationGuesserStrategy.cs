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

public class PerLetterEliminationGuesserStrategy(IWordList wordList) : NextWordGuesserStrategyBase(wordList)
{
    public override double ScoreForGuess(string guess)
    {
        int previousScore = GetTotalPossibleLettersInWordList(CandidateWordList);
        int simulatedGuessAllLetterScore = GetTotalPossibleLettersInWordList(
            CandidateWordList.WithPositionLetterRules(GenerateGuessPositionLetterRules(guess))
        );
        return previousScore - simulatedGuessAllLetterScore;
    }

    public static int GetTotalPossibleLettersInWordList(IWordList wordList)
    {
        return wordList.PossibleLetters.Sum(possibleLetters => possibleLetters.Count);
    }

    public static List<LetterAtPositionInWordRule> GenerateGuessPositionLetterRules(string guess)
    {
        return guess.Enumerate().ConvertAll(lwp => new LetterAtPositionInWordRule(lwp.Position, lwp.Letter, LetterAtPositionInWord.Misplaced));
    }
}