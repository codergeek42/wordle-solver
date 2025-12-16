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

using Moq;
using WordleSolver.Library;
using WordleSolver.Library.Extensions;

namespace WordleSolver.Tests.Extensions;

public static class WordListMockExtensions
{
    public static Mock<IWordList> SetupAlphabetMockReturnValue(this Mock<IWordList> mockWordList, HashSet<char> expectedAlphabet)
    {
        mockWordList
            .Setup(wordList => wordList.Alphabet)
            .Returns(expectedAlphabet);
        return mockWordList;
    }

    public static Mock<IWordList> SetupCountLettersMockReturnValue(this Mock<IWordList> mockWordList, List<Dictionary<char, int>> expectedPossibleLetters)
    {
        mockWordList
            .Setup(wordList => wordList.CountLetters())
            .Returns(expectedPossibleLetters);
        return mockWordList;
    }

    public static Mock<IWordList> SetupLetterRulesMockReturnValue(this Mock<IWordList> mockWordList, List<LetterAtPositionInWordRule> expectedLetterRules)
    {
        mockWordList
            .Setup(wordList => wordList.LetterRules)
            .Returns(expectedLetterRules);
        return mockWordList;
    }
    public static Mock<IWordList> SetupPossibleLettersMockReturnCount(this Mock<IWordList> mockWordList, int expectedCount)
    {
        return mockWordList.SetupPossibleLettersMockReturnValue(expectedCount.Repeat(_ => new HashSet<char>(['_'])));
    }


    public static Mock<IWordList> SetupPossibleLettersMockReturnValue(this Mock<IWordList> mockWordList, List<HashSet<char>> expectedPossibleLetters)
    {
        mockWordList
            .Setup(wordList => wordList.PossibleLetters)
            .Returns(expectedPossibleLetters);
        return mockWordList;
    }

    public static Mock<IWordList> StubProcessExclusionsFromRules(this Mock<IWordList> mockWordList)
    {
        mockWordList
            .Setup(wordList => wordList.ProcessExclusionsFromRules(It.IsAny<List<LetterAtPositionInWordRule>>()));
        return mockWordList;
    }

    public static Mock<IWordList> SetupWordsMockReturnCount(this Mock<IWordList> mockWordList, int expectedWordsCount)
    {
        return mockWordList.SetupWordsMockReturnValue(expectedWordsCount.Repeat(_ => ""));
    }


    public static Mock<IWordList> SetupWordsMockReturnValue(this Mock<IWordList> mockWordList, List<string> expectedWords)
    {
        mockWordList
            .Setup(mockWordList => mockWordList.Words)
            .Returns(expectedWords);
        return mockWordList;
    }

}