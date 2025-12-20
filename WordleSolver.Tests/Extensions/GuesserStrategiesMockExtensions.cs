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

using Moq;

using WordleSolver.Library;
using WordleSolver.Library.GuesserStrategies;

namespace WordleSolver.Tests.Extensions;

/// <summary>
/// Extensions for mocking INextWordGuesserStrategy to centralize method & property usage, and allow fluent chaining
/// for slightly nicer setup calls.
/// </summary>
public static class GuesserStrategiesMockExtensions
{
    /// <summary>
    /// Sets up the AlreadyGuessedLetters to return the specified set.
    /// </summary>
    /// <param name="guesserStrategyUnderTest">The mock gueser strategy</param>
    /// <param name="mockReturn">The desired return value.</param>
    /// <returns>The calling mock.</returns>
    // TODO make this go on the Mock directly instead of the INextWordGuesserStrategy.
    public static INextWordGuesserStrategy SetupGetAlreadyGuessedLettersMockReturn(this INextWordGuesserStrategy guesserStrategyUnderTest, HashSet<char> mockReturn)
    {
        guesserStrategyUnderTest.PreviousGuesses.Clear();
        guesserStrategyUnderTest.PreviousGuesses.Add(new(string.Join(string.Empty, mockReturn), []));
        return guesserStrategyUnderTest;
    }

    /// <summary>
    /// Sets up each guesser strategy mock so that their GuessNextWordAndScore() in order return the specified score of the same index.
    /// </summary>
    /// <param name="guesserStrategyMocks">The mock guesser strategy.</param>
    /// <param name="expectedScores">The desired return values.</param>
    /// <returns>The calling mock.</returns>
    public static List<Mock<INextWordGuesserStrategy>> SetupWordGuessAndScoreReturnValues(this List<Mock<INextWordGuesserStrategy>> guesserStrategyMocks, List<WordGuessAndScore> expectedScores)
    {
        guesserStrategyMocks.Should()
            .HaveCount(expectedScores.Count, "mock expected scores data should have the same length as calls to be made");

        foreach (int idx in Enumerable.Range(0, guesserStrategyMocks.Count))
        {
            guesserStrategyMocks[idx]
                .Setup(guesserStrategy => guesserStrategy.GuessNextWordAndScore())
                .Returns(expectedScores[idx]);
        }
        return guesserStrategyMocks;
    }

    /// <summary>
    /// Sets up each guesser strategy mock so that their IsSolved() return the specified value of the same index.
    /// </summary>
    /// <param name="guesserStrategyMocks">The mock guesser strategy</param>
    /// <param name="expectedIsSolveds">The desired return values.</param>
    /// <returns>The calling mock.</returns>
    public static List<Mock<INextWordGuesserStrategy>> SetupIsSolvedMockReturns(this List<Mock<INextWordGuesserStrategy>> guesserStrategyMocks, List<bool> expectedIsSolveds)
    {
        guesserStrategyMocks.Should()
            .HaveCount(expectedIsSolveds.Count, "mock expected solved values should have the same length as calls to be made");

        foreach (int idx in Enumerable.Range(0, guesserStrategyMocks.Count))
        {
            guesserStrategyMocks[idx]
                .Setup(guesserStrategy => guesserStrategy.IsSolved())
                .Returns(expectedIsSolveds[idx]);
        }
        return guesserStrategyMocks;
    }

    /// <summary>
    /// Sets up each guesser strategy mock so that their HasSolution() return the specified value of the same index.
    /// </summary>
    /// <param name="guesserStrategyMocks">The mock guesser strategy</param>
    /// <param name="expectedIsSolveds">The desired return values.</param>
    /// <returns>The calling mock.</returns>
    public static List<Mock<INextWordGuesserStrategy>> SetupHasSolutionMockReturns(this List<Mock<INextWordGuesserStrategy>> guesserStrategyMocks, List<bool> expectedHasSolutions)
    {
        guesserStrategyMocks.Should()
            .HaveCount(expectedHasSolutions.Count, "mock expected solved values should have the same length as calls to be made");

        foreach (int idx in Enumerable.Range(0, guesserStrategyMocks.Count))
        {
            guesserStrategyMocks[idx]
                .Setup(guesserStrategy => guesserStrategy.HasSolution())
                .Returns(expectedHasSolutions[idx]);
        }
        return guesserStrategyMocks;
    }
}
