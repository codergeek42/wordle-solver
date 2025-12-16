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

using DistinctLettersScoreForGuessTestCase = (
    string CaseName,
    int NumGuessedLetters,
    int NumDistinctLetters
);

using AwesomeAssertions;
using WordleSolver.Library;
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;
using WordleSolver.Library.Extensions;

namespace WordleSolver.Tests;

public class DistinctLettersStrategyTest : MockWordListFixture
{
    public static IEnumerable<object[]> DistinctLettersScoreForGuessTestData()
    {
        IEnumerable<DistinctLettersScoreForGuessTestCase> TestCases = Data.WordLength.RepeatMany(
            numGuessedLetters => Data.WordLength.Repeat(numDistinctLetters => (
                CaseName: $"{numGuessedLetters} guessed already, {numDistinctLetters} distinct unguessed",
                NumGuessedLetters: numGuessedLetters,
                NumDistinctLetters: numDistinctLetters
            )));

        foreach (var (CaseName, NumGuessedLetters, NumDistinctLetters) in TestCases)
        {
            yield return [CaseName, NumGuessedLetters, NumDistinctLetters];
        }
    }

    [Fact]
    public void DistinctLettersStrategy_Constructor_CanBeInstantiated()
    {
        DistinctLettersGuesserStrategy distinctLettersStrategy = new(MockWordList.Object);

        distinctLettersStrategy.SetupGetAlreadyGuessedLettersMockReturn(['A']);

        distinctLettersStrategy.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<DistinctLettersGuesserStrategy>($"should be a {nameof(DistinctLettersGuesserStrategy)}")
            .And.BeAssignableTo<NextWordGuesserStrategyBase>($"should subclass ${nameof(NextWordGuesserStrategyBase)}");
    }

    [Theory]
    [MemberData(nameof(DistinctLettersScoreForGuessTestData))]
    public void DistinctLettersStrategy_ScoreForGuess_ScoresGuessBasedOnDistinctUngessedLetters(string because, int numGuessedLetters, int numDistinctLetters)
    {
        HashSet<char> previousGuessLetters = Data.Alphabet.Take(numGuessedLetters).ToHashSet();

        int AlphabetSize = numGuessedLetters + numDistinctLetters;

        AlphabetSize.Should()
            .BeInRange(0, Data.Alphabet.Count, "test case data should contain valid guessed + distinct letter quantities");

        // Not null by construction of Data.Alphabet, and above assertion.
        string distinctGuessLetters = string.Join(string.Empty, Data.Alphabet.Take(AlphabetSize));

        DistinctLettersGuesserStrategy distinctLettersStrategy = new(MockWordList.Object);

        distinctLettersStrategy.SetupGetAlreadyGuessedLettersMockReturn(previousGuessLetters);

        double result = distinctLettersStrategy.ScoreForGuess(distinctGuessLetters);

        result.Should()
            .Be(numDistinctLetters, because);
    }
}