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

namespace WordleSolver.CLI;

/// <summary>
/// The main CLI application class.
/// </summary>
public class WordleSolverCommandLineApp
{
    /// <summary>
    /// The main CLI application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static async Task Main(string[] args)
    {
        CommandLineOptions cliOptions = CommandLineArguments.Parse(args, Environment.Exit);

        Console.WriteLine(LegalTexts.AppTitle);
        Console.WriteLine(LegalTexts.WelcomeBanner);

        TextMenu mainMenu = new TextMenu("===== Main Menu =====")
            .WithAsyncItem("Begin guessing!", async () =>
            {
                GuessLoop mainGuesserLoop = await GuessLoop.InitializeAsync(cliOptions);
                await mainGuesserLoop.RunGuessLoopAsync();
            })
            .WithItem("(No) Warranty Information", () => Console.WriteLine(LegalTexts.DisclaimerOfWarranty))
            .WithItem("Copyleft Information", () => Console.WriteLine(LegalTexts.CopyleftInformation))
            .WithItem("Exit", () => Environment.Exit(0))
            .WithPrompt("?")
            .WithOptions(new()
            {
                IsMultiline = true,
                ItemSelector = TextMenuItemSelector.AutoNumbered
            });
        while (true)
        {
            await mainMenu.RunPromptAsync();
        }
    }
}
