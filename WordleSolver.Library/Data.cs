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

public class Data
{
    public const int WordLength = 5;

    public static List<char> Alphabet = [.. "ABCDEFGHIJKLMNOPQRSTUVWXYZ"];

    public static List<string> GenerateAlphabetWords(string alphabet, int wordLength = WordLength)
    {
        if (wordLength <= 0)
        {
            throw new WordleSolverTestException($"{nameof(GenerateAlphabetWords)}: invalid word length");
        }
        return wordLength == 1
            ? [.. alphabet.ToCharArray().Select(ch => $"{ch}")]
            : [.. GenerateAlphabetWords(alphabet, wordLength - 1)
                .SelectMany(word => alphabet.Select(letter => $"{word}{letter}"))];
    }
};