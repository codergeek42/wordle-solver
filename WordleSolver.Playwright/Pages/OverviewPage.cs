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

using System.Text.RegularExpressions;

using Microsoft.Playwright;


/// <summary>
/// The main Wordle overview page.
/// </summary>
public class OverviewPage(IPage Page)
{
    public static string Url => "https://www.nytimes.com/games/wordle/index.html";

    public static Regex WordleChancesRegex => new("^Get (?<NumChances>[\\d]+) chances to guess a (?<WordLength>[\\d]+)-letter word\\.$");

    public ILocator TitleH1 => Page.GetByTestId("title");
    public ILocator WordleIcon => Page.GetByTestId("--wordle-icon");
    public ILocator WordleChances = Page.GetByText(WordleChancesRegex).First;


    public ILocator HowToPlayModal = Page.GetByLabel("help Modal");

    public ILocator HowToPlayModalCloseButton => HowToPlayModal.GetByLabel("Close");


    public ILocator PlayButton => Page.GetByTestId("Play");
    public ILocator SubscribeButton => Page.GetByTestId("Subscribe");



    public async Task NavigateToOverviewAndClickThroughToPuzzleAsync()
    {
        await Page.GotoAsync(Url);
        await WordleChances.FocusAsync();
        await PlayButton.ClickAsync();
        await HowToPlayModalCloseButton.ClickAsync();
    }
}


