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



using Moq;

namespace WordleSolver.Library.GuesserStrategies;


/// <summary>
/// Test fixture which provides exactly one mock guesser strategy.
/// </summary>
public class MockSingleNextWordGuesserStrategyFactory : INextWordGuesserStrategyFactory
{
    /// <summary>
    /// The list of mock guesser strategies.
    /// </summary>
    public Mock<INextWordGuesserStrategy> NextWordGuesserStrategy { get; private set; } = new();

    /// <summary>
    /// Factory method to "instantiate" the mock guesser strategy.
    /// </summary>
    /// <param name="wordList">The mock WordList.</param>
    /// <returns>The singleton list of mock guesser strategy objects.</returns>
    public List<INextWordGuesserStrategy> FromWordList(IWordList wordList)
    {
        return [NextWordGuesserStrategy.Object];
    }
}