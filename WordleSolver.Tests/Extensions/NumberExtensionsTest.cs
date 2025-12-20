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

using WordleSolver.Library.Extensions;


/// <summary>
/// Unit tests for <see cref="NumberExtensions"/>.
/// </summary>
public class NumberExtensionsTest
{

    [Fact]
    public void NumberExtensions_Repeat_ShouldRepeatCallback()
    {
        int numRepetitions = 5;
        Func<int, int> functionThatReturnsIntDoubled = (int num) => 2 * num;
        List<int> expectedDoubles = Enumerable.Range(0, numRepetitions).Select(functionThatReturnsIntDoubled).ToList();

        List<int> result = numRepetitions.Repeat(functionThatReturnsIntDoubled);

        result.Should()
            .NotBeNullOrEmpty("should create the non-empty list")
            .And.BeOfType<List<int>>("should return list of the correct type")
            .And.Equal(expectedDoubles);
    }

    [Fact]
    public void NumberExtensions_RepeatMany_ShouldRepeatCallbackAndFlattenList()
    {
        int numRepetitions = 5;
        Func<int, List<int>> functionThatReturnsIntDoubledAndIndex = (int num) => [num * 2, num];
        List<int> expectedDoublesAndInts = Enumerable.Range(0, numRepetitions).SelectMany(functionThatReturnsIntDoubledAndIndex).ToList();

        List<int> result = numRepetitions.RepeatMany(functionThatReturnsIntDoubledAndIndex);

        result.Should()
            .NotBeNullOrEmpty("should create the non-empty list")
            .And.BeOfType<List<int>>("should return list of the correct type, flattened")
            .And.Equal(expectedDoublesAndInts);
    }
}
