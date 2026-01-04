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

using WordleSolver.Library.Extensions;
using WordleSolver.Tests;

namespace WordleSolver.CLI;

/// <summary>
/// Integration tests for <see cref="TextMenu"/>.
/// </summary>
[Trait("Category", "Integration")]
public class TextMenuIntegrationTest : MockConsoleFixture
{
    [Fact]
    public void TextMenu_DisplayMenuItemsMultiLine_DisplaysItemsInMultipleLines()
    {
        Action noopCallback = () => { };
        TextMenu testMenu = new TextMenu("test-title")
            .WithItem("One", noopCallback)
            .WithItem("Two", noopCallback)
            .WithItem("Three", noopCallback);

        testMenu.DisplayMenuItemsMultiLine();

        string result = MockConsoleOutput.ToString();
        string expected = string.Join(
            Environment.NewLine,
            [
                .. testMenu.Items.Select((_, idx) => testMenu.GetItemAtIndex(idx)),
                "" // As the Console.WriteLine will append a newline to all lines
            ]
        );
        result.Should()
            .Be(expected, "should display Items one per line");
    }

    [Fact]
    public void TextMenu_DisplayMenuItemsSingleLine_DisplaysItemsInSingleLine()
    {
        Action noopCallback = () => { };
        TextMenu testMenu = new TextMenu("test-title")
            .WithItem("One", noopCallback)
            .WithItem("Two", noopCallback)
            .WithItem("Three", noopCallback);

        testMenu.DisplayMenuItemsSingleLine();

        string result = MockConsoleOutput.ToString();
        string expected = string.Join(
            "; ",
            testMenu.Items.Select((_, idx) => testMenu.GetItemAtIndex(idx))
        );
        result.Should()
            .Be(expected, "should display Items in one line");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TextMenu_DisplaysTitleAndItemsAndPrompt_HandlesInvalidGuesses_ThenRunsAssociatedCallback(
        bool isMultiline
    )
    {
        string title = "test-menu-title";
        List<string> items = ["first", "second", "third"];
        string prompt = "test-menu-prompt";

        string whichCallbackCalled = string.Empty;

        TextMenu testMenu = new TextMenu(title)
            .WithPrompt(prompt)
            .WithOptions(new()
            {
                IsMultiline = isMultiline,
                ItemSelector = TextMenuItemSelector.AutoNumbered
            });
        // Todo: need to check multiline output...
        testMenu = items.Aggregate(testMenu, (menu, item) =>
            menu.WithItem(item, () =>
            {
                whichCallbackCalled = item;
            }
        ));

        List<string> testInputs = [
            // Invalid because not FirstLetter selector.
            "F",
            // Invalid because out of range.
            (items.Count + 1).ToString(),
            // Valid on third attempt
            "2"
        ];

        string expectOutput =
            title + Environment.NewLine
            + string.Join(
                isMultiline ? Environment.NewLine : "; ",
                items.Select((_, itemIdx) => testMenu.GetItemAtIndex(itemIdx))
            )
            + (isMultiline ? Environment.NewLine : "")
            + $"{prompt} "
            + string.Join(
                string.Empty,
                (testInputs.Count - 1).Repeat(_ =>
                    $"Invalid choice!{Environment.NewLine}{prompt} "
                )
            );


        MockConsoleInput.SetInputText(string.Join(Environment.NewLine, testInputs));

        await testMenu.RunPromptAsync();

        string resultOutput = MockConsoleOutput.ToString();
        resultOutput.Should()
            .NotBeNullOrEmpty("should show output to console")
            .And.Be(expectOutput, "should match expected output");
        whichCallbackCalled.Should()
            .Be(items[1], "should call the given function when selected");
    }
}
