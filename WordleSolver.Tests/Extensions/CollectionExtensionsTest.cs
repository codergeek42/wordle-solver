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

using WordleSolver.Library;
using WordleSolver.Library.Extensions;


/// <summary>
/// Unit tests for <see cref="CollectionExtensions"/> .
/// </summary>
[Trait("Category", "Unit")]
public class CollectionExtensionsTest
{
    [Fact]
    public void CollectionExtensions_Enumerate_EnumeratesStringWithLetterPositions()
    {
        string text = "ABC";
        List<LetterWithPosition> expected = [
            new LetterWithPosition('A', 0),
            new LetterWithPosition('B', 1),
            new LetterWithPosition('C', 2)
        ];

        List<LetterWithPosition> result = text.Enumerate();

        result.Should()
            .BeOfType<List<LetterWithPosition>>("should enumerate to Wordle Solver type")
            .And.BeEquivalentTo(expected, "should pair letters with their corresponding 0-based indices");
    }


    [Fact]
    public void CollectionExtensions_RotateRight_RotatesRight()
    {
        List<int> original = [1, 2, 3];
        List<int> expected = [3, 1, 2];

        List<int> result = original.RotateRight().ToList();

        result.Should()
            .BeOfType<List<int>>("should maintain type of elements")
            .And.Equal(expected, "should be rotated right");
    }

    [Fact]
    public void CollectionExtensions_RotateRight_KeepsSingleton()
    {
        List<char> original = ['A'];
        List<char> expected = [.. original];
        List<char> result = original.RotateRight().ToList();

        result.Should()
            .BeOfType<List<char>>("should maintain type of element")
            .And.Equal(expected, "should maintain singleton array as-is");
    }

    [Fact]
    public void CollectionExtensions_RotateRight_KeepsEmpty()
    {
        List<string> original = [];
        List<string> expected = [.. original];
        List<string> result = original.RotateRight().ToList();

        result.Should()
                .BeOfType<List<string>>("should maintain type of element")
                .And.BeEmpty("should maintain empty array as empty");
    }
}
