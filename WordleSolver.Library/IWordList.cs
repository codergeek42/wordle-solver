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

namespace WordleSolver.Library;

/// <summary>
/// The interface prototype for the WordList type.
/// </summary>
public interface IWordList
{
    /// <summary>
    /// The remaining alphabet for the list of words.
    /// </summary>
    public HashSet<char> Alphabet { get; }

    /// <summary>
    /// The ongoing list of letter-position rules.
    /// </summary>
    public List<LetterAtPositionInWordRule> LetterRules { get; }

    /// <summary>
    /// The list of possible letters remaining at each position.
    /// </summary>
    public List<HashSet<char>> PossibleLetters { get; }

    /// <summary>
    /// The list of remaining words still possible.
    /// </summary>
    public List<string> Words { get; }

    /// <summary>
    /// Determines if the given word is still a possible solution with all of the letter-position rules.
    /// </summary>
    /// <param name="word">The candidate word.</param>
    /// <returns>False if any of the letter-positions rule exclude the given word; true otherwise.</returns>
    public bool DoesWordMatchAllRules(string word);

    /// <summary>
    /// Processes the given letter-position rules and excludes every word from the candidate list that are
    /// no longer possible, then re-calculates the alphabet and possible letters at each position accordingly.
    /// </summary>
    /// <param name="lettersAtPositionInWordRules">The list of letter-position rules.</param>
    public void ProcessExclusionsFromRules(List<LetterAtPositionInWordRule> lettersAtPositionInWordRules);

    /// <summary>
    /// Calculate a list of letter counts at each position from the dictionary of remaining possible words.
    /// </summary>
    /// <returns>The list of letter counts at each position of the remaining possible words.</returns>
    public List<Dictionary<char, int>> CountLetters();

    /// <summary>
    /// Creates a copy of the calling WordList with the given rules processed, without modifying the caller.
    /// </summary>
    /// <param name="lettersAtPositionRules">The list of letter-position rules.</param>
    /// <returns>The created copy.</returns>
    public IWordList WithPositionLetterRules(List<LetterAtPositionInWordRule> lettersAtPositionRules);


    /// <summary>
    /// Creates a copy of the calling WordList  with the given words excluded, without modifying the caller.
    /// </summary>
    /// <param name="excludedWords">The list of words to exclude.</param>
    /// <returns>The created copy.</returns>
    public IWordList WithExcludedWords(List<string> excludedWords);
}
