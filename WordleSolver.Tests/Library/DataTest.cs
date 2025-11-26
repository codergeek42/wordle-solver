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

namespace WordleSolver.Tests;

public class DataTest
{
    [Fact]
    public void Alphabet_HasAll26Chars()
    {
        Data.Alphabet.Should()
            .HaveCount(26, "alphabet should contain all 26 letters")
            .And.OnlyHaveUniqueItems("alphabet should only contain each letter once")
            .And.ContainInOrder([.. Enumerable.Range('A', 26).Select(ch => (char)ch)],
                "alphabet should contain every letter");
    }

    [Theory]
    [InlineData(-1, "negative")]
    [InlineData(0, "zero")]
    public void GenerateAlphabetWords_ShouldThrowIfNonPositiveLength(int wordLength, string condition)
    {
        string alphabet = "ABC";
        Action testCall = () => Data.GenerateAlphabetWords(alphabet, wordLength);

        testCall.Should()
            .Throw<WordleSolverTestException>($"the word length is {condition}");
    }

    [Fact]
    public void GenerateAlphabetWords_ShouldGenerateAllPossibleWords()
    {
        string alphabet = "ABC";
        List<string> result = Data.GenerateAlphabetWords(alphabet, 2);
        List<string> expectedAlphabetWords = ["AA", "AB", "AC", "BA", "BB", "BC", "CA", "CB", "CC"];

        // TODO: Make this a custom fluent assertion if unordered?
        result.Should()
            .Equal(expectedAlphabetWords, "the given alphabet should be permuted for the given word length");
    }
}