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

public class WordList : IWordList
{
    public HashSet<char> Alphabet { get; private set; }
    public List<LetterAtPositionInWordRule> LetterRules { get; private set; }

    public List<HashSet<char>> PossibleLetters { get; private set; }
    public List<string> Words { get; private set; }

    public WordList()
    {
        Alphabet = new HashSet<char>();
        LetterRules = [];
        PossibleLetters = [];
        Words = [];
    }

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
        PossibleLetters = Data.WordLength.Repeat(idx => new HashSet<char>()).ToList();
        foreach (string word in Words)
        {
            foreach (int idx in Enumerable.Range(0, word.Length))
            {
                PossibleLetters[idx].Add(word[idx]);
            }
        }

    }

    public WordList(IWordList original)
        : this(original.Words)
    { }

    public static async Task<WordList> FromFileAsync(string filename, IFileSystem fileSystem)
    {
        string[] wordsFromFile = await fileSystem.File.ReadAllLinesAsync(filename);
        return new WordList(wordsFromFile);
    }

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

    public void ProcessExclusionsFromRules(List<LetterAtPositionInWordRule> lettersAtPositionInWordRules)
    {
        foreach (LetterAtPositionInWordRule rule in lettersAtPositionInWordRules.OrderBy(rule => rule.Required))
        {
            if (rule.Required == LetterAtPositionInWord.Impossible)
            {
                PossibleLetters.ForEach(possibleLettersAtPos => possibleLettersAtPos.ExceptWith([rule.Letter]));
            }
            else if (rule.Position is null)
            {
                throw new MissingLetterRulePositionException();
            }
            else
            {
                switch (rule.Required)
                {
                    // NB: C# doesn't auto-detect that these cannot be null here for some reason; and
                    // without the explicit int assertions gives a CS1503 error, even though the thrown
                    // MissingLetterRulePositionException in the preceding condition should prevent that.
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
        var excludingWords = Words.Where(word => !DoesWordMatchAllRules(word)).ToList();
        // Console.WriteLine("REMOVING WORDS: {0}", JsonConvert.SerializeObject(excludingWords));
        Words.RemoveAll(word => !DoesWordMatchAllRules(word));
        Alphabet = string.Join("", Words).ToHashSet();
    }

    public List<Dictionary<char, int>> CountLetters()
    {
        if (Words.Count <= 0)
        {
            throw new NoMoreGuessesException();
        }

        return Data.WordLength
            .Repeat(position => Words
                .CountBy(word => word[position])
                .ToList()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ).ToList();
    }

    public IWordList WithPositionLetterRules(List<LetterAtPositionInWordRule> lettersAtPositionRules)
    {
        IWordList newWordList = new WordList(this);
        newWordList.ProcessExclusionsFromRules(lettersAtPositionRules);
        return newWordList;
    }

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