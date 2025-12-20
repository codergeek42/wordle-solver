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
using WordleSolver.Library.Extensions;
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="PerLetterEliminationGuesserStrategy"/>
/// </summary>
public class PerLetterEliminationGuesserStrategyTest : MockWordListFixture
{
    [Fact]
    public void PerLetterEliminationGuesserStrategy_Constructor_CanBeInstantiated()
    {
        PerLetterEliminationGuesserStrategy perLetterEliminationStrategy = new(MockWordList.Object);

        perLetterEliminationStrategy.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<PerLetterEliminationGuesserStrategy>($"should be a {nameof(PerLetterEliminationGuesserStrategy)}")
            .And.BeAssignableTo<NextWordGuesserStrategyBase>($"should subclass ${nameof(NextWordGuesserStrategyBase)}");
    }

    [Fact]
    public void PerLetterEliminationGuesserStrategy_GetTotalPossibleLettersInWordList_SumsNumberOfLetters()
    {
        MockWordList.SetupPossibleLettersMockReturnValue(
            Data.WordLength.Repeat(maxPos => Data.Alphabet.Take(1 + maxPos).ToHashSet())
        );

        int result = PerLetterEliminationGuesserStrategy.GetTotalPossibleLettersInWordList(MockWordList.Object);

        MockWordList.Verify(wordList => wordList.PossibleLetters, Times.Once(), "should query the possible letters");
        result.Should()
            .Be(Enumerable.Range(1, Data.WordLength).Sum(), "should count the total possible letters");
    }

    [Fact]
    public void PerLetterEliminationGuesserStrategy_GenerateGuessPositionLetterRules_CreatesRuleForEachGuessedLetterAsMisplaced()
    {
        string guess = string.Join(string.Empty, Data.Alphabet.Take(Data.WordLength));
        List<LetterAtPositionInWordRule> result = PerLetterEliminationGuesserStrategy.GenerateGuessPositionLetterRules(guess);
        List<LetterAtPositionInWordRule> expected = guess.Enumerate().ConvertAll(lwp =>
            new LetterAtPositionInWordRule(lwp.Position, lwp.Letter, LetterAtPositionInWord.Misplaced)
        );

        result.Should()
            .BeEquivalentTo(result, "should map the guess to rules with each letter Misplaced");
    }

    [Fact]
    public void PerLetterEliminationGuesserStrategy_ScoreForGuess_ScoresGuessByNumberOfLetterPossibilitiesEliminated()
    {
        WordList wordList = new(["WORDS"]);
        WordList wordListOther = new(["OTHER"]);
        Mock<IWordList> mockWordListWithPositionLetterRules = new();
        string guess = "GUESS";
        int expectedScore = 42;

        MockWordList.SetupPossibleLettersMockReturnCount(1 + expectedScore);
        mockWordListWithPositionLetterRules.SetupPossibleLettersMockReturnCount(1);
        List<LetterAtPositionInWordRule> expectedGuessAsMisplacedRules = PerLetterEliminationGuesserStrategy.GenerateGuessPositionLetterRules(guess);
        MockWordList.SetupProcessExclusionsFromRulesMockReturnValue(mockWordListWithPositionLetterRules.Object);
        PerLetterEliminationGuesserStrategy perLetterEliminationStrategy = new(MockWordList.Object);

        double result = perLetterEliminationStrategy.ScoreForGuess(guess);


        MockWordList.Verify(wordList => wordList.PossibleLetters, Times.Once(),
            "should query possible letters from provided word list");
        mockWordListWithPositionLetterRules.Verify(wordList => wordList.PossibleLetters, Times.Once(),
            "should query possible letters from word list with guess applied as all Misplaced");
        result.Should()
            .Be(expectedScore, "should calculate the total score based on difference of possible letters");
    }
}
