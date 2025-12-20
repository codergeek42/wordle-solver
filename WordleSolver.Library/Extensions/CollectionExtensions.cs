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

namespace WordleSolver.Library.Extensions;

/// <summary>
/// Utility methods that are callable on collections of items.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Converts the given string to a list of position-character pairs,
    /// similar to Python's "enumerate(...)".
    /// </summary>
    /// <param name="stringToEnumerate">the string to enumerate</param>
    /// <returns>A list of letters from the string with their corresponding positions.</returns>
    public static List<LetterWithPosition> Enumerate(this string stringToEnumerate)
    {
        return stringToEnumerate
            .Select((letter, position) => new LetterWithPosition(letter, position))
            .ToList();
    }

    /// <summary>
    /// Rotates the given enumerable one element to the right, shifting the rightmost element to be the leftmost.
    /// More formally, returns a new enumerable whose 0th index element is that of index (L-1) from the original, and
    /// whose nth index element otherwise is that of index (n-1) from the original, where L is the count of the
    /// given enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Take(^1..).Concat(enumerable.Take(..^1));
    }
}