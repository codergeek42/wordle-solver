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

using System.IO.Abstractions.TestingHelpers;
using System.Text;

using AwesomeAssertions;

using WordleSolver.Library;
using WordleSolver.Library.Extensions;

// FIXME: Use TheoryDataRow & related to clean up this test case naming & output.
using DoesWordMatchAllRulesTestCase = (
    string CaseName,
    string Alphabet,
    System.Collections.Generic.List<WordleSolver.Library.LetterAtPositionInWordRule> PositionLetterRules,
    System.Collections.Generic.List<(string Word, bool IsMatch)> ExpectedWordMatches
);

namespace WordleSolver.Tests;


/// <summary>
/// Unit tests for <see cref="WordList"/>
/// </summary>
[Trait("Category", "Unit")]
public class WordListTest
{
    public static DoesWordMatchAllRulesTestCase ExcludeImpossibleAAtEveryPosition = (
        CaseName: "exclude A at every position (Impossible)",
        Alphabet: "AB",
        PositionLetterRules: [
            new LetterAtPositionInWordRule(null, 'A', LetterAtPositionInWord.Impossible)
        ],
        ExpectedWordMatches: [
            (Word: new string('A', Data.WordLength), IsMatch: false)
        ]
    );

    public static DoesWordMatchAllRulesTestCase RequireMandatoryBAtEveryPosition = (
        CaseName: "require B at every position (Mandatory)",
        Alphabet: "AB",
        PositionLetterRules: Data.WordLength.Repeat(idx => new LetterAtPositionInWordRule(idx, 'B', LetterAtPositionInWord.Mandatory)),
        ExpectedWordMatches: [
            (Word: new string('A', Data.WordLength), IsMatch: false),
            (Word: new string('B', Data.WordLength), IsMatch: true),
            ..Data.WordLength.Repeat(idx => (
                // A list of words with an 'A' in exactly each position and 'B's otherwise.
                Word: new StringBuilder(new string('B', Data.WordLength)) {[idx] = 'A'}.ToString(),
                IsMatch: false
            ))
        ]
    );

    public static DoesWordMatchAllRulesTestCase RequireMisplacedCAtAnyPositionOtherThanZero = (
        CaseName: "require C at any position other than 0 (Misplaced)",
        Alphabet: "ABC",
        PositionLetterRules: [
            new LetterAtPositionInWordRule(0, 'C', LetterAtPositionInWord.Misplaced)
        ],
        ExpectedWordMatches: [
            // C cannot be at position 0...
            (Word: new string('C', Data.WordLength), IsMatch: false),
            // ...but the word must contain at least one C...
            (Word: new string('B', Data.WordLength), IsMatch: false),
            // ...and that C can be at any other position.
            (Word: 'B' + new string('C', Data.WordLength-1), IsMatch: true)
        ]
    );

    public static IEnumerable<object[]> DoesWordMatchAllRulesTestData()
    {
        List<DoesWordMatchAllRulesTestCase> testCases = [
            ExcludeImpossibleAAtEveryPosition,
            RequireMandatoryBAtEveryPosition,
            RequireMisplacedCAtAnyPositionOtherThanZero
        ];
        foreach (var (CaseName, Alphabet, PositionLetterRules, ExpectedWordMatches) in testCases)
        {
            yield return [CaseName, Alphabet, PositionLetterRules, ExpectedWordMatches];
        }
    }


    [Fact]
    public void WordList_Constructor_CanBeInstantiatedAsEmpty()
    {
        WordList emptyWordList = new();

        emptyWordList.Words.Should()
            .NotBeNull("should instantiate empty word list")
            .And.BeOfType<List<string>>("should be a string list")
            .And.BeEmpty("should have no words");
        emptyWordList.Alphabet.Should()
            .NotBeNull("should instantiate alphabet")
            .And.BeOfType<HashSet<char>>("should have alphabet be a set of chars")
            .And.BeEmpty("should have empty alphabet");
        emptyWordList.LetterRules.Should()
            .NotBeNull("should instantiate letter rules")
            .And.BeOfType<List<LetterAtPositionInWordRule>>("should have letter rules be a list of rules")
            .And.BeEmpty("should have empty letter rules");
        emptyWordList.PossibleLetters.Should()
            .NotBeNull("should instantiate possible letters")
            .And.BeOfType<List<HashSet<char>>>("should have possible letters be a list of sets of chars")
            .And.BeEquivalentTo(Data.WordLength.Repeat(_ => new HashSet<char>()),
                "should have an empty possible letters list of the correct length");
    }

    [Fact]
    public void WordList_Constructor_CanBeInstantiatedFromListOfWords()
    {
        char[] threeLetters = ['A', 'B', 'C'];
        List<string> threeWords = Data.GenerateAlphabetWords(string.Join("", threeLetters));
        WordList threeWordList = new(threeWords);

        threeWordList.Should()
            .BeOfType<WordList>($"should be a {nameof(WordList)}");
        threeWordList.Words.Should()
            .Equal(threeWords, "should store all words from the list");
        threeWordList.Alphabet.Should()
            .Equal(threeLetters.ToHashSet(), "should populate the alphabet from the words");
        threeWordList.PossibleLetters.Should()
            .AllSatisfy(pl => pl.Equals(threeLetters), "each position should have all possible letters");
        threeWordList.LetterRules.Should()
            .BeEmpty("should have no exclusion rules");
    }

    [Fact]
    public void WordList_Constructor_ShouldUpperTrimAndSortGivenList()
    {
        List<string> badWords = ["UNORD", "lower", " NOTRM "];
        List<string> goodWords = badWords.Select(word => word.Trim().ToUpper()).Order().ToList();

        WordList goodWordList = new(badWords);

        goodWordList.Should()
            .BeOfType<WordList>($"should be a {nameof(WordList)}");
        goodWordList.Words.Should()
            .Equal(goodWords, "words should be ordered, trimmed, and uppercase");
    }

    [Fact]
    public void WordList_Constructor_ShouldIgnoreWordsThatAreWrongLengthOrNotAlphabeticCharacters()
    {
        List<string> badWords = ["1_ONE", "2TWO2", "THREE", "FOUR"];
        List<string> goodWords = badWords
            .Where(word => word.Length == Data.WordLength && word.All(ch => Data.Alphabet.Contains(ch)))
            .ToList();

        WordList goodWordList = new(badWords);

        goodWordList.Should()
            .BeOfType<WordList>($"should be a {nameof(WordList)}");
        goodWordList.Words.Should()
            .Equal(goodWords, $"words should be exactly {Data.WordLength} alphabetic letters");
    }

    [Fact]
    public void WordList_Constructor_ShouldCreateAlphabetFromGivenWords()
    {
        List<string> alphabetBuilderWords = [
            "ABCDE",
            "FGHIJ", // Does it correctly use all of the words (i.e., not just one)?
            "ABCDK", // Does it correctly ignore duplicate letters across words (ABC)?
            "AABLL", // Does it correctly ignore duplicate letters within the same word (AA and LL)?
		];
        HashSet<char> expectedAlphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L'];

        WordList testWordList = new(alphabetBuilderWords);

        testWordList.Should()
            .BeOfType<WordList>($"should be a {nameof(WordList)}");
        testWordList.Words.Should()
            .Equal(alphabetBuilderWords.Order(), "should use only the given list of words");
        testWordList.Alphabet.Order().Should()
            .Equal(expectedAlphabet, "should build the alphabet from only the given words");

    }

    [Fact]
    public void WordList_Copy_ShouldMakeCopyOfOther()
    {
        WordList originalWordList = new(["TESTS", "FROMC", "OPYOF", "OTHER"]);
        WordList result = new(originalWordList);

        result.Should()
            .BeOfType<WordList>($"should be a {nameof(WordList)}")
            .And.BeEquivalentTo(originalWordList, "constructed copy should be identical")
            .And.NotBeSameAs(originalWordList, "constructed copy should be new distinct object");
    }

    [Fact]
    public async Task WordList_FromFile_CanInstantiateFromLinesOfFile()
    {
        List<string> wordsInFile = Data.GenerateAlphabetWords("ABC");
        string filename = "test.txt";

        MockFileSystem mockFileSystem = new();
        mockFileSystem.AddFile(filename, new MockFileData(string.Join("\n", wordsInFile)));

        WordList testWordList = await WordList.FromFileAsync(filename, mockFileSystem);

        testWordList.Words.Should()
            .Equal(wordsInFile.Order(), "should read orders words from file");
    }

    [Theory]
    [MemberData(nameof(DoesWordMatchAllRulesTestData))]
    public void WordList_DoesWordMatchAllRules_ShouldMatchAllRuleTypes(
        string because,
        string alphabet,
        List<LetterAtPositionInWordRule> positionLetterRules,
        List<(string Word, bool IsMatch)> expectedWordMatches
    )
    {
        WordList testWordList = new(Data.GenerateAlphabetWords(alphabet));
        testWordList.ProcessExclusionsFromRules(positionLetterRules);
        foreach (var (Word, IsMatch) in expectedWordMatches)
        {
            testWordList.DoesWordMatchAllRules(Word).Should()
                .Be(IsMatch, because);
        }

    }


    [Fact]
    public void WordList_CountLetters_ShouldThrowIfEmpty()
    {
        WordList emptyWordList = new([]);
        Action testCall = () => emptyWordList.CountLetters();

        testCall.Should()
            .Throw<NoMoreGuessesException>("word list is empty");
    }

    [Fact]
    public void WordList_CountLetters_ShouldReturnLettersCountIfNonEmpty()
    {
        WordList testWordList = new(["AAAAA", "BBABB", "ACACD"]);
        List<Dictionary<char, int>> expectedCounts = [
            new() { ['A'] = 2, ['B'] = 1 },
            new() { ['A'] = 1, ['B'] = 1, ['C'] = 1 },
            new() { ['A'] = 3 },
            new() { ['A'] = 1, ['B'] = 1, ['C'] = 1 },
            new() { ['A'] = 1, ['B'] = 1, ['D'] = 1 }
        ];

        List<Dictionary<char, int>> result = testWordList.CountLetters();

        result.Should()
            .BeEquivalentTo(expectedCounts, "should count letters at each position");
    }

    [Fact]
    public void WordList_WithPositionLetterRules_ReturnsCopyWithProcessedRules()
    {
        WordList originalWordList = new(Data.GenerateAlphabetWords("ABC"));
        WordList originalCopy = new(originalWordList);

        List<LetterAtPositionInWordRule> letterAtPositionInWordRules = [
            new LetterAtPositionInWordRule(null, 'A', LetterAtPositionInWord.Impossible),
            new LetterAtPositionInWordRule(0, 'B', LetterAtPositionInWord.Misplaced),
            new LetterAtPositionInWordRule(1, 'C', LetterAtPositionInWord.Mandatory)
        ];

        WordList result = (WordList)originalWordList.WithPositionLetterRules(letterAtPositionInWordRules);

        originalWordList.Should()
            .BeEquivalentTo(originalCopy, "calling object letter rules should not be processed");
        result.Should()
            .NotBeSameAs(originalWordList, "should return a modified copy, not the original");
        result.LetterRules.Should()
            .BeEquivalentTo(letterAtPositionInWordRules, "should process the given rules");
    }

    [Fact]
    public void WordList_WithExcludedWords_ShouldUpperAndTrimWords()
    {
        List<string> badWords = [" NOTRM ", "lower"];
        WordList badWordList = new(
            ["UPPER", .. badWords.ConvertAll(word => word.ToUpper().Trim())]
        );
        badWordList.Words.Should()
            .BeEquivalentTo(["LOWER", "NOTRM", "UPPER"], $"{nameof(WordList)} trims and uppercases words");

        IWordList wordListWithExcluded = badWordList.WithExcludedWords(badWords);

        wordListWithExcluded.Words.Should()
            .NotContain("NOTRM", "should exclude the trimmed word")
            .And.NotContain("LOWER", "should exclude the uppercased word")
            .And.Contain("UPPER", "should not exclude the word that was not in the parameter");
    }
}
