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

namespace WordleSolver.Tests;

using AwesomeAssertions;
using Moq;
using WordleSolver.Library;
using WordleSolver.Library.Extensions;
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;

public class WordleSolverTest : MockWordListFixture, IDisposable
{
    [Fact]
    public void WordleSolver_Constructor_ShouldStoreWordList()
    {
        MockSingleNextWordGuesserStrategyFactory mockGuesserStrategyFactory = new();
        WordleSolver wordleSolver = new(MockWordList.Object, mockGuesserStrategyFactory);


        wordleSolver.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<WordleSolver>("should be its correct type");
        wordleSolver.GuesserStrategies.Should()
            .BeOfType<List<INextWordGuesserStrategy>>("should have guesser strategies populated")
            .And.BeEquivalentTo([mockGuesserStrategyFactory.NextWordGuesserStrategy.Object], "should populate guesser strategies from factory");
        wordleSolver.CandidateWordList.Should()
            .BeSameAs(MockWordList.Object, "should store original word list in solver");
        wordleSolver.GuesserStrategies.First().Should()
            .BeEquivalentTo(mockGuesserStrategyFactory.NextWordGuesserStrategy.Object, "should store word list into guesser strategy");
    }

    [Fact]
    public void WordleSolver_WithPreviousGuess_AppliesPreviousGuessToGuesserStrategiesAndReturnsCaller()
    {
        MockSingleNextWordGuesserStrategyFactory mockGuesserStrategyFactory = new();
        WordleSolver wordleSolver = new(MockWordList.Object, mockGuesserStrategyFactory);
        WordGuessAndResult previousGuess = new("GUESS", []);

        IWordleSolver result = wordleSolver.WithPreviousGuess(previousGuess);

        mockGuesserStrategyFactory.NextWordGuesserStrategy.Verify(
            guesserStrategy => guesserStrategy.WithPreviousGuess(previousGuess),
            "should apply previous guess and result to guesser strategy"
        );

        result.Should()
            .BeSameAs(wordleSolver, "should return calling object for chaining");
    }

    [Fact]
    public void WordleSolver_GuessNextWord_CallsGuesserStrategyScoresAndReturnsMax()
    {
        MockMultiNextWordGuesserStrategyFactory mockGuesserStrategyFactory = new();
        WordleSolver wordleSolver = new(MockWordList.Object, mockGuesserStrategyFactory);
        WordGuessAndScore lowScore = new("GUESS", 4.9);
        WordGuessAndScore highScore = new("SCORE", 5.0);
        mockGuesserStrategyFactory.NextWordGuesserStrategies.SetupWordGuessAndScoreReturnValues([lowScore, highScore]);

        (string GuesserStrategy, WordGuessAndScore GuessAndScore) result = wordleSolver.GuessNextWord();

        mockGuesserStrategyFactory.NextWordGuesserStrategies.Should()
            .AllSatisfy(
                guesserStrategy => guesserStrategy.Verify(guesserStrategy => guesserStrategy.GuessNextWordAndScore()),
                "should call each guesser strategy's GuessNextWordAndScore"
            );

        result.Should()
            .NotBeNull("should return a result");
        result.GuessAndScore.Should()
            .Be(highScore, "should return the highest-scoring guess");
    }

    [Theory]
    [InlineData(2, 2, false)]
    [InlineData(1, 2, true)]
    [InlineData(0, 1, true)]
    public void WordleSolver_IsSolved(int countGuesserStrategiesUnsolved, int countExpectedIsSolvedCalls, bool expectedSolved)
    {
        MockMultiNextWordGuesserStrategyFactory mockGuesserStrategyFactory = new();
        WordleSolver wordleSolver = new(MockWordList.Object, mockGuesserStrategyFactory);
        int countTestGuesserStrategies = mockGuesserStrategyFactory.NextWordGuesserStrategies.Count;
        mockGuesserStrategyFactory.NextWordGuesserStrategies.SetupIsSolvedMockReturns([
           ..countGuesserStrategiesUnsolved.Repeat(_ => false),
           ..(countTestGuesserStrategies - countGuesserStrategiesUnsolved).Repeat(_ => true)
        ]);

        bool result = wordleSolver.IsSolved();

        int expectedGuesserStrategiesIsSolvedCount = Math.Min(countGuesserStrategiesUnsolved, 1);
        mockGuesserStrategyFactory.NextWordGuesserStrategies
            .Take(countExpectedIsSolvedCalls).Should()
            .AllSatisfy(
                guesserStrategy => guesserStrategy.Verify(guesserStrategy => guesserStrategy.IsSolved(), Times.Once()),
                "should call each guesser strategy's IsSolved with short-circuiting"
            );
        mockGuesserStrategyFactory.NextWordGuesserStrategies
            .TakeLast(countTestGuesserStrategies - countExpectedIsSolvedCalls).Should()
            .AllSatisfy(
                guesserStrategy => guesserStrategy.Verify(guesserStrategy => guesserStrategy.IsSolved(), Times.Never()),
                "should not call each remaining guesser strategy's IsSolved due when one is already true"
            );

        result.Should()
            .Be(expectedSolved, $"should be {expectedSolved} because {countGuesserStrategiesUnsolved}/2 are false");
    }

    [Theory]
    [InlineData(2, 1, false)]
    [InlineData(1, 1, false)]
    [InlineData(0, 2, true)]
    public void WordleSolver_HasSolution(int countGuesserStrategiesUnsolved, int countExpectedIsSolvedCalls, bool expectedSolved)
    {
        MockMultiNextWordGuesserStrategyFactory mockGuesserStrategyFactory = new();
        WordleSolver wordleSolver = new(MockWordList.Object, mockGuesserStrategyFactory);
        int countTestGuesserStrategies = mockGuesserStrategyFactory.NextWordGuesserStrategies.Count;
        mockGuesserStrategyFactory.NextWordGuesserStrategies.SetupHasSolutionMockReturns([
           ..countGuesserStrategiesUnsolved.Repeat(_ => false),
           ..(countTestGuesserStrategies - countGuesserStrategiesUnsolved).Repeat(_ => true)
        ]);

        bool result = wordleSolver.HasSolution();

        int expectedGuesserStrategiesIsSolvedCount = Math.Min(countGuesserStrategiesUnsolved, 1);
        mockGuesserStrategyFactory.NextWordGuesserStrategies
            .Take(countExpectedIsSolvedCalls).Should()
            .AllSatisfy(
                guesserStrategy => guesserStrategy.Verify(guesserStrategy => guesserStrategy.HasSolution(), Times.Once()),
                "should call each guesser strategy's IsSolved with short-circuiting"
            );
        mockGuesserStrategyFactory.NextWordGuesserStrategies
            .TakeLast(countTestGuesserStrategies - countExpectedIsSolvedCalls).Should()
            .AllSatisfy(
                guesserStrategy => guesserStrategy.Verify(guesserStrategy => guesserStrategy.HasSolution(), Times.Never()),
                "should not call each remaining guesser strategy's IsSolved due when one is already true"
            );

        result.Should()
            .Be(expectedSolved, $"should be {expectedSolved} because {countGuesserStrategiesUnsolved}/2 are false");
    }
}