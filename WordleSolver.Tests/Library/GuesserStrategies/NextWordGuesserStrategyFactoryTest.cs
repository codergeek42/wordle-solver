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

public class NextWordGuesserStrategyFactoryTest : MockWordListFixture
{

    [Fact]
    public void NextWordGuesserStrategyFactory_GetAllGuesserStrategies_ReturnsAllGuesserStrategies()
    {
        MockWordList
            .SetupAlphabetMockReturnValue(['_'])
            .SetupLetterRulesMockReturnValue([
                new LetterAtPositionInWordRule(0, '_', LetterAtPositionInWord.Mandatory)
            ])
            .SetupPossibleLettersMockReturnValue(Data.WordLength.Repeat(_ => new HashSet<char> { '_' }))
            .SetupWordsMockReturnValue([
                new string('_', Data.WordLength)
            ]);
        List<INextWordGuesserStrategy> result = new NextWordGuesserStrategyFactory().FromWordList(MockWordList.Object);

        result.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<List<INextWordGuesserStrategy>>("should be a list of guesser strategies")
            .And.HaveCount(4, "should have all four guesser strategies")
            .And.ContainItemsAssignableTo<DistinctLettersGuesserStrategy>($"should have a {nameof(DistinctLettersGuesserStrategy)}")
            .And.ContainItemsAssignableTo<LetterFrequencyGuesserStrategy>($"should have a {nameof(LetterFrequencyGuesserStrategy)}")
            .And.ContainItemsAssignableTo<PerLetterEliminationGuesserStrategy>($"should have a {nameof(PerLetterEliminationGuesserStrategy)}")
            .And.ContainItemsAssignableTo<RetryMisplacedLettersGuesserStrategy>($"should have a {nameof(RetryMisplacedLettersGuesserStrategy)}");
        result.Should()
            .AllSatisfy(guesserStrategy =>
            {
                guesserStrategy.CandidateWordList.Should()
                    .BeEquivalentTo(new WordList(MockWordList.Object), $"should store word list into {guesserStrategy.GetType().Name}")
                    .And.NotBeSameAs(MockWordList.Object, $"should copy word list into {guesserStrategy.GetType().Name}");
            },
            "should copy word list into each guesser strategy, not store reference");
    }
}

