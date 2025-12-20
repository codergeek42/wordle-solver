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

using System.IO.Abstractions;

using WordleSolver.Library.Extensions;

namespace WordleSolver.Library;

/// <summary>
/// Tracks a list of possible guesses and processes their resulting exclusions. 
/// </summary>
public class WordList : IWordList
{
    /// <summary>
    /// The total set of possible letters across the words.
    /// </summary>
    public HashSet<char> Alphabet { get; private set; } = [];

    /// <summary>
    /// The list of processed letter-position rules. 
    /// </summary>
    public List<LetterAtPositionInWordRule> LetterRules { get; private set; } = [];

    /// <summary>
    /// The letters still possible at each position.
    /// </summary>
    public List<HashSet<char>> PossibleLetters { get; private set; } = [];

    /// <summary>
    /// The list of words still possible. 
    /// </summary>
    public List<string> Words { get; private set; } = [];


    /// <summary>
    /// Build a WordList from the given list of candidate words, initially assuming all are possible after filtering
    /// for the correct length, trimming whitespace, and uppercasing all of them.
    /// </summary>
    /// <param name="words">The initial list of candidate words.</param>
    public WordList(IList<string>? words = default(List<string>))
    {
        words ??= [];

        Words = words
            .Select(word => word.Trim().ToUpper())
            .Where(word => word.Length == Data.WordLength && word.All(letter => Data.Alphabet.Contains(letter)))
            .Order()
            .ToList();
        LetterRules = [];
        Alphabet = string.Join("", Words).ToHashSet();
        PossibleLetters = Data.WordLength.Repeat(_ => new HashSet<char>()).ToList();
        foreach (string word in Words)
        {
            foreach (int idx in Enumerable.Range(0, word.Length))
            {
                PossibleLetters[idx].Add(word[idx]);
            }
        }

    }

    /// <summary>
    /// Copy constructor: Creates a duplicate of the given WordList.
    /// </summary>
    /// <param name="original">The WordList to copy.</param>
    public WordList(IWordList original)
        : this(original.Words)
    { }

    /// <summary>
    /// Creates a WordList from the contents of the given file, one word per line.
    /// </summary>
    /// <param name="filename">The path to the words file.</param>
    /// <param name="fileSystem">The filesystem abstraction layer.</param>
    /// <returns>The created WordList object.</returns>
    // TODO: Error-handling cases for file access.
    // TODO: default parameter for filesystem?
    public static async Task<WordList> FromFileAsync(string filename, IFileSystem fileSystem)
    {
        string[] wordsFromFile = await fileSystem.File.ReadAllLinesAsync(filename);
        return new WordList(wordsFromFile);
    }

    /// <summary>
    /// Determines if the given word is possible within the current possible letters and letter-position rules.
    /// </summary>
    /// <param name="word">The candidate guess.</param>
    /// <returns>True if all all letters in the word are possible at their corresponding positions; false
    /// otherwise.</returns>
    public bool DoesWordMatchAllRules(string word)
    {
        // Is every letter in the word one of the possible letters remaining at that position?
        bool allLettersInWordArePossible = PossibleLetters.Index()
            .All(t => t.Item.Contains(word[t.Index]));
        // Are any letters in the word excluded by rules?
        bool noLettersInWordAreExcluded = LetterRules
            .All(rule => rule.Required != LetterAtPositionInWord.Misplaced || word.Contains(rule.Letter));
        return allLettersInWordArePossible && noLettersInWordAreExcluded;
    }

    /// <summary>
    /// Processes the given letter-position rules and recalculates the calling WordList to exclude all
    /// words that the rule makes no longer possible.
    /// </summary>
    /// <param name="lettersAtPositionInWordRules">The letter-position rules to process.</param>
    /// <exception cref="MissingLetterRulePositionException">If one of the rules is invalid because it is
    /// not an Impossible rule but yet has a null Position.</exception>
    public void ProcessExclusionsFromRules(List<LetterAtPositionInWordRule> lettersAtPositionInWordRules)
    {
        foreach (LetterAtPositionInWordRule rule in lettersAtPositionInWordRules.OrderBy(rule => rule.Required))
        {
            if (rule.Required == LetterAtPositionInWord.Impossible)
            {
                PossibleLetters.ForEach(possibleLettersAtPos => possibleLettersAtPos.ExceptWith([rule.Letter]));
            }
            else
            {
                switch (rule.Required)
                {
                    // NB: C# doesn't auto-detect that these cannot be null here for some reason; and
                    // without the explicit int assertions gives a CS1503 error, even though the thrown
                    // MissingLetterRulePositionException from LetterAtPositionInWordRule should prevent that.
                    case LetterAtPositionInWord.Mandatory:
                        PossibleLetters[(int)rule.Position!] = [rule.Letter];
                        break;
                    case LetterAtPositionInWord.Misplaced:
                        PossibleLetters[(int)rule.Position!].ExceptWith([rule.Letter]);
                        break;
                }
            }
        }
        LetterRules.AddRange(lettersAtPositionInWordRules);
        List<string> excludingWords = Words.Where(word => !DoesWordMatchAllRules(word)).ToList();
        Words.RemoveAll(word => !DoesWordMatchAllRules(word));
        Alphabet = string.Join("", Words).ToHashSet();
    }

    /// <summary>
    /// Calculates the letter frequency at each position.
    /// </summary>
    /// <returns>A list of counts of each letter, where the keys are the letters and values are their corresponding
    /// counts at those positions.</returns>
    /// <exception cref="NoMoreGuessesException">If the list of words is empty.</exception>
    public List<Dictionary<char, int>> CountLetters()
    {
        if (Words.Count <= 0)
        {
            throw new NoMoreGuessesException();
        }

        return Data.WordLength
            .Repeat(position => Words
                .CountBy(word => word[position])
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            );
    }

    /// <summary>
    /// Creates a copy of the caller, and applies the given letter-position rules to that copy. Does not
    /// modify the caller.
    /// </summary>
    /// <param name="lettersAtPositionRules">The letter-position rules that apply to the copy.</param>
    /// <returns>The created duplicate, with the rules processed.</returns>
    public IWordList WithPositionLetterRules(List<LetterAtPositionInWordRule> lettersAtPositionRules)
    {
        IWordList newWordList = new WordList(this);
        newWordList.ProcessExclusionsFromRules(lettersAtPositionRules);
        return newWordList;
    }

    /// <summary>
    /// Creates a copy of the caller, and removes the given words from the copy's list. Does not modify
    /// the caller.
    /// </summary>
    /// <param name="excludedWords">The list of words to exclude from the copy.</param>
    /// <returns>The created duplicate, with the given words excluded.</returns>
    public IWordList WithExcludedWords(List<string> excludedWords)
    {
        IEnumerable<string> wordsToRemove = excludedWords
            .ConvertAll(word => word.Trim().ToUpper());
        IWordList newWordList = new WordList(
            Words.Except(wordsToRemove).ToList()
        );
        return newWordList;

    }
}
