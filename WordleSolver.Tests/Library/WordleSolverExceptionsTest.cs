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

using AwesomeAssertions;

using WordleSolver.Library;

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="WordleSolverException"/>
/// </summary>
public class ExceptionsTest
{
    [Fact]
    public void WordleSolverException_ExtendsException()
    {
        object baseException = new WordleSolverException();

        baseException.Should()
            .BeAssignableTo<Exception>($"{nameof(WordleSolverException)} should extend {nameof(Exception)}");
    }

    [Fact]
    public void WordleSolverException_TakesMessage()
    {
        var testMessage = "test message";
        var exceptionWithMessage = new WordleSolverException(testMessage);

        exceptionWithMessage.Should()
            .BeOfType<WordleSolverException>($"exception with message should be a {nameof(WordleSolverException)}");
        exceptionWithMessage.Message.Should()
            .Match(testMessage, "exception should populate message");
    }

    [Fact]
    public void WordleSolverException_TakesMessageAndInnerException()
    {
        var testMessage = "test message";
        var innerException = new Exception("inner exception");
        var exceptionWithMessageAndInnerException = new WordleSolverException(testMessage, innerException);

        exceptionWithMessageAndInnerException.Should()
            .BeOfType<WordleSolverException>($"exception with message should be a {nameof(WordleSolverException)}");
        exceptionWithMessageAndInnerException.Message.Should()
            .Match(testMessage, "exception should populate message");
        exceptionWithMessageAndInnerException.InnerException.Should()
            .Be(innerException, "exception should carry inner exception");
    }
}
