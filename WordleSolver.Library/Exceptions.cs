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
/// Generic WordleSolver exception base class.
/// </summary>
public class WordleSolverException : Exception
{
    public WordleSolverException() { }

    public WordleSolverException(string message) : base(message) { }
    public WordleSolverException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// An error occured in one the test setup helpers, such as `GenerateAlphabetWords`.
/// </summary>
public class WordleSolverTestException : WordleSolverException
{
    public WordleSolverTestException(string message) : base(message) { }
}

/// <summary>
/// A `LetterAtPositionInWordRule` or `LetterWithPosition` was attempted to be processed that required a `Position`
/// property but did not have it.
/// </summary>
public class MissingLetterRulePositionException : WordleSolverException { }

/// <summary>
/// There are no more possible guesses (i.e., the set of possible words has become empty).
/// </summary>
public class NoMoreGuessesException : WordleSolverException { }


