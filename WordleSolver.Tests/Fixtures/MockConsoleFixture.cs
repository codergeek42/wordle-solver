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

namespace WordleSolver.Tests;

/// <summary>
/// A basic test fixture which stores and resets the console I/O per test.
/// NB: The Collection attribute is required to ensure that all test classes
/// which use this mock console fixture are run by xUnit in the same collection
/// (i.e., sequentially rather than in parallel), in order to prevent the
/// global Console mocks from leaking between tests.
/// </summary>
[Collection("MockConsole")]
public class MockConsoleFixture : IDisposable
{
    public StringReader MockConsoleInput { get; set; } = new(string.Empty);
    public StringWriter MockConsoleOutput { get; set; } = new();

    public StringWriter MockConsoleError { get; set; } = new();

    public MockConsoleFixture()
    {
        MockConsoleInput = new(string.Empty);
        MockConsoleOutput = new();
        MockConsoleError = new();

        Console.SetIn(MockConsoleInput);
        Console.SetOut(MockConsoleOutput);
        Console.SetError(MockConsoleError);
    }

    public virtual void Dispose()
    {
        StreamReader stdIn = new(Console.OpenStandardInput());
        StreamWriter stdOut = new(Console.OpenStandardOutput());
        StreamWriter stdErr = new(Console.OpenStandardError());
        stdOut.AutoFlush = true;
        stdErr.AutoFlush = true;

        Console.SetIn(stdIn);
        Console.SetOut(stdOut);
        Console.SetError(stdErr);
    }
}
