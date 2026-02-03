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
namespace WordleSolver.Playwright;

using Microsoft.Playwright;

using WordleSolver.CLI;

/// <summary>
/// The main Playwright solver application class.
/// </summary>
public class WordleSolverPlaywrightApp
{
    public static async Task Main(string[] args)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 200
        });

        CommandLineOptions cliOpts = CommandLineArguments.Parse(args, Environment.Exit);
        IBrowserContext browserContext = await browser.NewContextAsync();
        // FIXME: Only needed for some browsers; adjust for parameterized browser usage at some point.
        await browserContext.GrantPermissionsAsync([
            "clipboard-read", "clipboard-write"
        ]);
        IPage browserPage = await browserContext.NewPageAsync();
        OverviewPage overviewPage = new(browserPage);
        GuessPage guessPage = new(browserPage);

        BrowserGuessLoop browserGuessLoop = await BrowserGuessLoop.InitializeAsync(cliOpts, guessPage);

        await overviewPage.NavigateToOverviewAndClickThroughToPuzzleAsync();
        await guessPage.SetSettingToggleAsync(guessPage.DarkModeToggle, cliOpts.BrowserDarkMode);
        await browserGuessLoop.RunGuessLoopAsync();
        string results = await guessPage.CopyAndScreenshotResultsAsync();
        Console.WriteLine($"*** Results: \n{results}");
    }
}
