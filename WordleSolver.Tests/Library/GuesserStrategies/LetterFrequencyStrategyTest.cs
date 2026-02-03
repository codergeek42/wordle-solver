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
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="LetterFrequencyGuesserStrategy"/>
/// </summary>
[Trait("Category", "Unit")]
public class LetterFrequencyStrategyTest : MockWordListFixture
{
    [Fact]
    public void LetterFrequencyGuesserStrategy_Constructor_CanBeInstantiated()
    {
        LetterFrequencyGuesserStrategy letterFrequencyStrategy = new(MockWordList.Object);

        letterFrequencyStrategy.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<LetterFrequencyGuesserStrategy>($"should be a {nameof(LetterFrequencyGuesserStrategy)}")
            .And.BeAssignableTo<NextWordGuesserStrategyBase>($"should subclass ${nameof(NextWordGuesserStrategyBase)}");
    }

    [Fact]
    public void LetterFrequencyGuesserStrategy_ScoreForGuess_ScoresGuessBasedOnLetterFrequency()
    {
        List<string> testWords = ["BREAD", "BROOD", "BLOOD", "CROOK"];
        MockWordList
            .SetupCountLettersMockReturnValue([
                new() { ['B'] = 3, ['C'] = 1 },
                new() { ['L'] = 1, ['R'] = 3 },
                new() { ['E'] = 1, ['O'] = 3 },
                new() { ['A'] = 1, ['O'] = 3 },
                new() { ['D'] = 3, ['K'] = 1 }
            ])
            .SetupWordsMockReturnValue(testWords);

        LetterFrequencyGuesserStrategy letterFrequencyStrategy = new(MockWordList.Object);

        List<WordGuessAndScore> results = testWords
            .ConvertAll(word => new WordGuessAndScore(word, letterFrequencyStrategy.ScoreForGuess(word)));

        results.Should()
            .BeEquivalentTo([
                new WordGuessAndScore("BLOOD", 3.25),
                new WordGuessAndScore("BREAD", 2.75),
                new WordGuessAndScore("BROOD", 3.75),
                new WordGuessAndScore("CROOK", 2.75)
            ], "word scores should be calculated by letter frequency");
    }

}
