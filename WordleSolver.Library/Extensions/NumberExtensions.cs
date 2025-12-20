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
/// Utility methods that extend numbers.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Repeat the given callback for the given count of times and returns a list of the results,
    /// like the "times" function from JavaScript's lodash.
    /// </summary>
    /// <typeparam name="T">the return type of the callback</typeparam>
    /// <param name="count">Count of times to call the given callback</param>
    /// <param name="callback">A function that maps the integer (0-based index) to some single value</param>
    /// <returns>A list of the callback results, called for each index from 0 to (count-1).
    public static List<T> Repeat<T>(this int count, Func<int, T> callback)
    {
        return Enumerable.Range(0, count).Select(callback).ToList();
    }

    /// <summary>
    /// Repeat the given callback for the given count of times and returns a list of the results, flattened one level,
    /// like chaining the "times" function from JavaScript's lodash with flatMap.
    /// </summary>
    /// <typeparam name="T">the type of the callback's returned enumerables</typeparam>
    /// <param name="count">Count of times to call the given callback</param>
    /// <param name="callback">A function that maps the integer (0-based index) to some enumerable value</param>
    /// <returns>A list of the callback results, called for each index from 0 to (count-1).
    public static List<T> RepeatMany<T>(this int count, Func<int, IEnumerable<T>> callback)
    {
        return Enumerable.Range(0, count).SelectMany(callback).ToList();
    }
}