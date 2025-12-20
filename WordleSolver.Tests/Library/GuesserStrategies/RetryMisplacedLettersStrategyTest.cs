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

using RetryMisplacedLettersTestCase = (
   string CaseName,
   int NumMisplacedLetters
);

using AwesomeAssertions;
using WordleSolver.Library;
using WordleSolver.Library.GuesserStrategies;
using WordleSolver.Tests.Extensions;
using WordleSolver.Library.Extensions;
using Moq;

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="RetryMisplacedLettersGuesserStrategy"/>
/// </summary>
public class RetryMisplacedLettersStrategyTest : MockWordListFixture
{
    public static IEnumerable<object[]> RetryMisplacedLettersScoreForGuessTestData()
    {
        IEnumerable<RetryMisplacedLettersTestCase> TestCases = (Data.WordLength + 1).Repeat((numMisplacedLetters) => (
            CaseName: $"scores the guess based on the number of previously misplaced letters ({numMisplacedLetters})",
            NumMisplacedLetters: numMisplacedLetters
        ));

        foreach (var (CaseName, NumMisplacedLetters) in TestCases)
        {
            yield return [CaseName, NumMisplacedLetters];
        }
    }


    [Fact]
    public void RetryMisplacedLettersGuesserStrategy_Constructor_CanBeInstantiated()
    {
        RetryMisplacedLettersGuesserStrategy retryMisplacedLettersStrategy = new(MockWordList.Object);

        retryMisplacedLettersStrategy.SetupGetAlreadyGuessedLettersMockReturn(['A']);

        retryMisplacedLettersStrategy.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<RetryMisplacedLettersGuesserStrategy>($"should be a {nameof(RetryMisplacedLettersGuesserStrategy)}")
            .And.BeAssignableTo<NextWordGuesserStrategyBase>($"should subclass ${nameof(NextWordGuesserStrategyBase)}");
    }

    [Theory]
    [MemberData(nameof(RetryMisplacedLettersScoreForGuessTestData))]
    public void RetryMisplacedLettersGuesserStrategy_ScoreForGuess_ScoresGuessBasedOnDistinctUngessedLetters(string because, int numMisplacedLetters)
    {
        int TestAlphabetLength = numMisplacedLetters + 3;

        TestAlphabetLength.Should()
            .BeLessThanOrEqualTo(Data.Alphabet.Count, "test data should contain valid misplaced letter quantities");

        IEnumerable<char> Alphabet = Data.Alphabet.Take(TestAlphabetLength);
        IEnumerable<char> MisplacedAlphabet = Alphabet.RotateRight();

        LetterAtPositionInWordRule ImpossibleLetterRule = new(null, Alphabet.ElementAt(^3), LetterAtPositionInWord.Impossible);
        LetterAtPositionInWordRule NonMatchingMisplacedLetterRule = new(numMisplacedLetters + 1, Alphabet.ElementAt(^2), LetterAtPositionInWord.Misplaced);
        List<LetterAtPositionInWordRule> PreviouslyMisplacedLetterRules = numMisplacedLetters.Repeat((position) => new LetterAtPositionInWordRule(position, Alphabet.ElementAt(position), LetterAtPositionInWord.Misplaced));
        MockWordList.Setup(wordList => wordList.LetterRules).Returns([
            ImpossibleLetterRule,
            NonMatchingMisplacedLetterRule,
            ..PreviouslyMisplacedLetterRules
        ]);

        RetryMisplacedLettersGuesserStrategy retryMisplacedLettersStrategy = new(MockWordList.Object);

        string Guess = string.Join(string.Empty, MisplacedAlphabet.Take(1 + numMisplacedLetters));

        double Result = retryMisplacedLettersStrategy.ScoreForGuess(Guess);

        MockWordList.Verify(wordList => wordList.LetterRules, Times.Once,
            "LetterRules should be invoked to determine previously misplaced letters");

        Result.Should()
            .Be(numMisplacedLetters, because);
    }
}