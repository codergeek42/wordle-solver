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

using AwesomeAssertions;

using WordleSolver.Library;
using WordleSolver.Library.Extensions;
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;

using DistinctLettersScoreForGuessTestCase = (
    string CaseName,
    int NumGuessedLetters,
    int NumDistinctLetters
);

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="DistinctLettersGuesserStrategy"/>
/// </summary>
[Trait("Category", "Unit")]
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

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    public void DistinctLettersStrategy_ShouldRun_ShouldRunIfHasPreviousGuesses(int previousGuessCount, bool expectedShouldRun)
    {
        DistinctLettersGuesserStrategy distinctLettersStrategy = new(MockWordList.Object);
        distinctLettersStrategy.PreviousGuesses.AddRange(
            previousGuessCount.Repeat(_ => new WordGuessAndResult("", []))
        );

        bool result = distinctLettersStrategy.ShouldRun();

        result.Should()
            .Be(expectedShouldRun, "should run only if has any previous guesses");
    }

    [Fact]
    public void DistinctLettersStrategy_WithPreviousGuess_CallsBaseWithEmptyResult()
    {
        DistinctLettersGuesserStrategy distinctLettersStrategy = new(MockWordList.Object);
        distinctLettersStrategy.PreviousGuesses.Should()
            .BeEmpty("should be empty at start of test");

        WordGuessAndResult previousGuessAndResult = new("ABCDE", [
            new(null, 'A', LetterAtPositionInWord.Impossible),
            new(0, 'B', LetterAtPositionInWord.Mandatory),
            new(0, 'C', LetterAtPositionInWord.Misplaced)
        ]);

        DistinctLettersGuesserStrategy result = (DistinctLettersGuesserStrategy)distinctLettersStrategy.WithPreviousGuess(previousGuessAndResult);
        result.PreviousGuesses.Should()
            .NotBeNullOrEmpty("should have that previous guess value")
            .And.BeEquivalentTo(
                [previousGuessAndResult with { Result = [] }],
                "should contain the guess without its result"
            );
    }
}
