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
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;

/// <summary>
/// A fully-implemented guesser strategy from NextWordGuesserStrategyBase, in order to be able to instantiate it
/// for testing.
/// </summary>
public class NextWordBaseTestGuesserStrategy : NextWordGuesserStrategyBase
{
    public NextWordBaseTestGuesserStrategy(IWordList wordList) : base(wordList)
    { }

    /// <summary>
    /// Scores the guess based on the sum of the ASCII values of its letters.
    /// </summary>
    /// <param name="guess">The candidate guess.</param>
    /// <returns>The calculated sum.</returns>
    public override double ScoreForGuess(string guess)
    {
        return guess.Sum(ch => (int)ch);
    }
}

/// <summary>
/// Unit tests for <see cref="NextWordGuesserStrategyBase"/>
/// </summary>
[Trait("Category", "Unit")]
public class NextWordGuesserStrategyBaseTest : MockWordListFixture
{
    [Fact]
    public void NextWordGuesserStrategyBase_Constructor_CanBeInstantiatedFromEmpty()
    {
        WordList emptyWordList = new([]);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(emptyWordList);

        nextWordGuesserStrategy.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<NextWordBaseTestGuesserStrategy>($"should be a {nameof(NextWordBaseTestGuesserStrategy)}");
        nextWordGuesserStrategy.CandidateWordList.Should()
            .BeOfType<WordList>("guesser word list should be instantiated");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_GetAlreadyGuessedLetters_ReturnsSetOfAlreadyGuessedLetters()
    {
        WordGuessAndResult wordGuessAndResult = new("GUESS", [new(0, 'G', LetterAtPositionInWord.Mandatory)]);

        MockWordList.StubProcessExclusionsFromRules();

        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        nextWordGuesserStrategy.PreviousGuesses.Should()
            .BeEmpty("previous guesses should be empty on newly-created guesser");

        NextWordBaseTestGuesserStrategy guesserWithPreviousGuess = (NextWordBaseTestGuesserStrategy)nextWordGuesserStrategy.WithPreviousGuess(wordGuessAndResult);

        guesserWithPreviousGuess.PreviousGuesses.Should()
            .Equal([wordGuessAndResult], "previous guess should be stored");

        HashSet<char> alreadyGuessedLetters = guesserWithPreviousGuess.GetAlreadyGuessedLetters();

        alreadyGuessedLetters.Should()
            .Equal(['G', 'U', 'E', 'S'], "set of guessed letter should be returned");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_GuessNextWordAndScore_ThrowsIfNoMoreGuesses()
    {
        MockWordList.SetupWordsMockReturnValue([]);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        Action testCall = () => nextWordGuesserStrategy.GuessNextWordAndScore();

        testCall.Should()
            .Throw<NoMoreGuessesException>("candidate word list is empty");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_GuessNextWordAndScore_ReturnsNextHighestScoreGuessIfAtLeasOneGuessRemains()
    {
        List<string> testWords = ["AAAAA", "BBBBB", "CCCCC"];

        testWords.Should()
            .BeInAscendingOrder("the highest-scoring word should be last");

        MockWordList.SetupWordsMockReturnValue(testWords);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);
        List<WordGuessAndScore> wordsWithScores = testWords.ConvertAll(word => new WordGuessAndScore(word, nextWordGuesserStrategy.ScoreForGuess(word)));
        WordGuessAndScore expectedGuessAndScore = wordsWithScores.Last();

        WordGuessAndScore result = nextWordGuesserStrategy.GuessNextWordAndScore();

        result.Should()
            .BeEquivalentTo(expectedGuessAndScore, "the last word in alphabetic order should score highest by sum of ASCII values");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_IsSolved_ReturnsTrueIfExactlyOneGuessRemains()
    {
        MockWordList.SetupWordsMockReturnCount(1);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        bool result = nextWordGuesserStrategy.IsSolved();

        result.Should()
            .BeTrue("exactly one guess remains");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_IsSolved_ReturnsFalseIfNoGuessesRemain()
    {
        MockWordList.SetupWordsMockReturnCount(0);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        bool result = nextWordGuesserStrategy.IsSolved();

        result.Should()
            .BeFalse("no guesses remain");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_IsSolved_ReturnsFalseIfMoreThanOneGuessRemains()
    {
        MockWordList.SetupWordsMockReturnCount(2);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        bool result = nextWordGuesserStrategy.IsSolved();

        result.Should()
            .BeFalse("more than one guess remains");
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void NextWordGuesserStrategyBase_HasSolution_ReturnsTrueIfAtLeastOneGuessRemains(int wordListLength)
    {
        MockWordList.SetupWordsMockReturnCount(wordListLength);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        bool result = nextWordGuesserStrategy.HasSolution();

        result.Should()
            .BeTrue("at least one guess remains");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_HasSolution_ReturnsFalseIfNoGuessesRemain()
    {
        MockWordList.SetupWordsMockReturnCount(0);
        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        bool result = nextWordGuesserStrategy.HasSolution();

        result.Should()
            .BeFalse("no guesses remain");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_WithPreviousGuess_StoresThePreviousGuessIfItWasValid()
    {
        WordGuessAndResult validGuessResult = new(
            "GUESS",
            [new(0, 'G', LetterAtPositionInWord.Mandatory)],
            true
        );
        MockWordList.StubProcessExclusionsFromRules();

        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        nextWordGuesserStrategy.PreviousGuesses.Should()
            .BeEmpty("previous guesses should be empty on newly-created guesser strategy");

        NextWordBaseTestGuesserStrategy result = (NextWordBaseTestGuesserStrategy)nextWordGuesserStrategy.WithPreviousGuess(validGuessResult);

        result.Should()
            .BeSameAs(nextWordGuesserStrategy, "should return the calling object for fluent chaining");
        MockWordList.Verify(wordList => wordList.ProcessExclusionsFromRules(validGuessResult.Result),
            Times.Once(), "should add exlcusion rule from the guess result");
        nextWordGuesserStrategy.PreviousGuesses.Should()
            .Equal([validGuessResult], "guesser should append previous guess to stored list");
    }

    [Fact]
    public void NextWordGuesserStrategyBase_WithPreviousGuess_ExcludesThePreviousGuessIfItWasInvalid()
    {
        WordGuessAndResult invalidGuessResult = new(
            "GUESS",
            [],
            false
        );
        MockWordList.StubWithExcludedWords();

        NextWordBaseTestGuesserStrategy nextWordGuesserStrategy = new(MockWordList.Object);

        nextWordGuesserStrategy.PreviousGuesses.Should()
            .BeEmpty("previous guesses should be empty on newly-created guesser strategy");

        NextWordBaseTestGuesserStrategy result = (NextWordBaseTestGuesserStrategy)nextWordGuesserStrategy.WithPreviousGuess(invalidGuessResult);

        result.Should()
            .BeSameAs(nextWordGuesserStrategy, "should return the calling object for fluent chaining");
        MockWordList.Verify(wordList => wordList.WithExcludedWords(new List<string> { invalidGuessResult.Word }),
            Times.Once(), "should add the word to the exclusion list");
        MockWordList.Verify(wordList => wordList.ProcessExclusionsFromRules(It.IsAny<List<LetterAtPositionInWordRule>>()),
            Times.Never(), "Should not call process exclusions when guess is invalid");
    }
}
