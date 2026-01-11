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
/// Determination of the letter requirement at some position in the word. 
/// </summary>
public enum LetterAtPositionInWord
{
    // Mandatory rules should be processed last.
    /// <summary>
    /// The letter at this position must be the given letter.
    /// </summary>
    Mandatory = 10,

    /// <summary>
    /// The given letter is certainly in the word, but not at this position.
    /// </summary>
    Misplaced = 1,

    /// <summary>
    /// The given letter is certainly not in the word, at any position.
    /// </summary>
    Impossible = 2,
};

/// <summary>
/// A pairing of a letter to its position rule.
/// </summary>
/// <param name="Letter">The letter (e.g. 'A', 'Z').</param>
/// <param name="Required">The letter-position requirement.</param>
public record LetterRule(char Letter, LetterAtPositionInWord Required);

/// <summary>
/// Pairing of a <see cref="LetterRule"/> with its associated position (optional).
/// </summary>
public record LetterAtPositionInWordRule : LetterRule
{
    /// <summary>
    /// The (0-based) position in the word.
    /// </summary>
    public int? Position { get; init; }

    /// <summary>
    /// Ensure that non-Impossible rules must have a Position set.
    /// </summary>
    /// <param name="position">The (0-based) position in the word.</param>
    /// <param name="Letter">The letter (e.g. 'A', 'Z').</param>
    /// <param name="Required">The letter-position requirement.</param>
    /// <exception cref="MissingLetterRulePositionException"></exception>
    public LetterAtPositionInWordRule(int? position, char Letter, LetterAtPositionInWord Required)
        : base(Letter, Required)
    {
        Position = position;
        if (Position is null && Required != LetterAtPositionInWord.Impossible)
        {
            throw new MissingLetterRulePositionException();
        }
    }
}

/// <summary>
/// A pairing of only a letter with its position, both required.
/// </summary>
/// <param name="Letter">The letter (e.g. 'A', 'Z').</param>
/// <param name="Position">The (0-bazed) position of the letter in the word.</param>
public record LetterWithPosition(char Letter, int Position);