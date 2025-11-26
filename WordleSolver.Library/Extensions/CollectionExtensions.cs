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

public static class CollectionExtensions
{
    public static Dictionary<int, T> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Count()
            .Repeat((idx) => (idx, enumerable.ElementAt(idx)))
            .ToDictionary();
    }

    public static List<LetterWithPosition> Enumerate(this string stringToEnumerate)
    {
        return stringToEnumerate.Enumerate<char>()
            .Select(enumeratedLetter => new LetterWithPosition(enumeratedLetter.Value, enumeratedLetter.Key))
            .ToList();
    }

    public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Take(^1..).Concat(enumerable.Take(..^1));
    }
}