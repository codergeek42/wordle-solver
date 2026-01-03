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
using WordleSolver.Library;

namespace WordleSolver.CLI;

/// <summary>
/// A base exception for errors that TextMenu might throw.
/// </summary>
/// <param name="message">The exception message.</param>
public class TextMenuException(string message) : WordleSolverException(message);

/// <summary>
/// Thrown when a menu is attempted to be used when it is still empty.
/// </summary>
/// <param name="message">(Optional) The exception message.</param>
public class TextMenuEmptyException(string message = "Menu is empty.") : TextMenuException(message);

/// <summary>
/// How to map the user choice to the item index.
/// </summary>
public enum TextMenuItemSelector
{
    /// <summary>
    /// Automatically numbered: the first item added is index 0 (choice 1), the next is index 1 (choice 2), and so on.
    /// </summary>
    AutoNumbered,

    /// <summary>
    /// Use the uppercase first alphabetic letter of the item.
    /// </summary>
    // TODO: Add check if multiple items have same first letter.
    FirstLetter
}


/// <summary>
/// Runtime options for TextMenu usage.
/// </summary>
public struct TextMenuOptions
{
    /// <summary>
    /// Whether to use single- or multi-line menus.
    /// </summary>
    public bool? IsMultiline { get; set; }

    /// <summary>
    /// How to present the choices to the user and map their choice to the item index.
    /// </summary>
    public TextMenuItemSelector? ItemSelector { get; set; }
}


/// <summary>
/// A rudimentary interactive text-based menu system for CLI usage.
/// </summary>
public class TextMenu(string title)
{
    /// <summary>
    /// List of async functions to call for each item.
    /// </summary>
    public List<Func<Task>> Callbacks { get; private set; } = [];

    /// <summary>
    /// List of menu entries (items presented to the user).
    /// </summary>
    public List<string> Items { get; private set; } = [];

    /// <summary>
    /// Runtime options to change how the menu items are displayed and prompted for.
    /// </summary>
    public TextMenuOptions Options { get; set; } = new()
    {
        IsMultiline = true,
        ItemSelector = TextMenuItemSelector.AutoNumbered
    };

    /// <summary>
    /// The prompt to display to the user when requesting their choice.
    /// </summary>
    public string Prompt { get; private set; } = "?";

    /// <summary>
    /// The title for the menn.
    /// </summary>
    public string Title { get; private set; } = title;

    /// <summary>
    /// Adds the given item to the menu, so that the associated callback is run when that item is chosen; then
    /// returns the calling object for fluent chaining.
    /// </summary>
    /// <param name="displayedText">The text to display for the user.</param>
    /// <param name="callback">The code to run when the user chooses this item.</param>
    /// <returns>The caller.</returns>
    public TextMenu WithItem(string displayedText, Action callback)
    {
        Items.Add(displayedText);
        Callbacks.Add(async () => await Task.Run(callback));
        return this;
    }

    /// <summary>
    /// Adds the given item to the menu, so that the associated async callback is run when that item is chosen; then
    /// returns the calling object for fluent chaining.
    /// </summary>
    /// <param name="displayedText">The text to display for the user.</param>
    /// <param name="callback">The async code to run when the user chooses this item.</param>
    /// <returns>The caller.</returns>
    public TextMenu WithAsyncItem(string displayedText, Func<Task> asyncCallback)
    {
        Items.Add(displayedText);
        Callbacks.Add(asyncCallback);
        return this;
    }

    /// <summary>
    /// Sets the options to those given, then returns the calling object for fluent chaining.
    /// </summary>
    /// <param name="options">The runtime options to set. </param>
    /// <returns>The caller.</returns>
    public TextMenu WithOptions(TextMenuOptions options)
    {
        Options = new()
        {
            IsMultiline = options.IsMultiline ?? Options.IsMultiline,
            ItemSelector = options.ItemSelector ?? Options.ItemSelector
        };
        return this;
    }

    /// <summary>
    /// Sets the prompt, and returns the calling object for fluent chaining.
    /// </summary>
    /// <param name="prompt">The prompt to display.</param>
    /// <returns>The caller.</returns>
    public TextMenu WithPrompt(string prompt)
    {
        Prompt = prompt;
        return this;
    }


    /// <summary>
    /// Returns the item at the given index, where the choice value is 
    /// displayed based on the current ItemSelector option: either numbered from 1, or
    /// with the first letter of each, with the choice value parenthesized.
    /// </summary>
    /// <param name="index">The index of the item.</param>
    /// <returns>The string to display for the given item index.</returns>
    /// <exception cref="IndexOutOfRangeException">If the index is out of range.</exception>
    /// <exception cref="TextMenuException">If the item selector option is invalid.</exception>
    public string GetItemAtIndex(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            throw new IndexOutOfRangeException($"{index} must be in [0, {Items.Count})");
        }
        string itemAtIndex = Items[index];
        switch (Options.ItemSelector)
        {
            case TextMenuItemSelector.AutoNumbered:
                int numberWidth = (int)Math.Ceiling(Math.Log10(Items.Count)) + 1;
                string displayedIndex = (index + 1).ToString().PadLeft(numberWidth);
                return $"({displayedIndex}) {itemAtIndex}";
            case TextMenuItemSelector.FirstLetter:
                // FIXME: Splitting this .ToUpper().FirstOrDefault(...) expression chain here is needed
                // for Coverlet to properly detect the full code coverage.
                string uppercasedItem = itemAtIndex.ToUpper();
                char displayedLetter = uppercasedItem.FirstOrDefault(char.IsLetterOrDigit);
                if (displayedLetter == default(char))
                {
                    throw new TextMenuException(
                        $"FirstLetter selector enabled; but item '{itemAtIndex}' at index {index} is not alphanumeric."
                    );
                }
                int indexOfLetter = itemAtIndex.ToUpper().IndexOf(displayedLetter);
                return $"{itemAtIndex[..indexOfLetter]}({displayedLetter}){itemAtIndex[(indexOfLetter + 1)..]}";
            default:
                throw new TextMenuException("Unhandled item selector.");
        }
    }

    /// <summary>
    /// Displayes the menu items, one per line.
    /// </summary>
    public void DisplayMenuItemsMultiLine()
    {
        foreach (var line in Items.Index())
        {
            Console.WriteLine(GetItemAtIndex(line.Index));
        }
    }

    /// <summary>
    /// Displays the menu items in one line, separated by semicolons.
    /// </summary>
    public void DisplayMenuItemsSingleLine()
    {
        foreach (var line in Items.Index())
        {
            Console.Write(GetItemAtIndex(line.Index));
            if (line.Index + 1 < Items.Count)
            {
                Console.Write("; ");
            }
        }
    }

    /// <summary>
    /// Converts the user choice to the index of the item that was chosen.
    /// </summary>
    /// <param name="choice">The choice made by the user.</param>
    /// <returns>The (0-based) item index of the chosen item.</returns>
    /// <exception cref="TextMenuException">If the item selector is invalid.</exception>
    public int ParseChoiceToEntriesIndex(string? choice)
    {
        if (string.IsNullOrEmpty(choice))
        {
            return -1;
        }
        switch (Options.ItemSelector)
        {
            case TextMenuItemSelector.AutoNumbered:
                // Let the range-checking logic in RunPrompt handle the actual numerical value.
                bool isValidInt = int.TryParse(choice, out int result);
                return isValidInt
                    ? result - 1
                    : -1;
            case TextMenuItemSelector.FirstLetter:
                char choiceLetter = choice.Trim().ToUpper()[0];
                return Items
                    .ConvertAll(item => item.ToUpper())
                    .FindIndex(item => item.First(char.IsLetterOrDigit) == choiceLetter);
            default:
                throw new TextMenuException("Unhandled selector parse option.");
        }
    }

    /// <summary>
    /// Displays the formatted menu, prompts the user for their choice, and then runs the associated callback
    /// of the given choice. If the user's choice was invalid, a brief error is printed and the user is re-prompted
    /// for a choice, until a valid choice is made.
    /// </summary>
    /// <exception cref="TextMenuException">If there is a mismatch between the items and callbacks counts.</exception>
    /// <exception cref="TextMenuEmptyException">If the menu is empty (no items).</exception>
    public async Task RunPrompt()
    {
        if (Items.Count != Callbacks.Count)
        {
            throw new TextMenuException("Mismatched items & callbacks counts -- this should not happen!");
        }
        if (Items.Count < 1)
        {
            throw new TextMenuEmptyException();
        }
        Console.WriteLine(Title);
        if (Options.IsMultiline ?? true)
        {
            DisplayMenuItemsMultiLine();
        }
        else
        {
            DisplayMenuItemsSingleLine();
        }
        while (true)
        {
            Console.Write($"{Prompt} ");
            int choice = ParseChoiceToEntriesIndex(Console.ReadLine());
            if (0 <= choice && choice < Items.Count)
            {
                await Callbacks[choice]();
                return;
            }
            else
            {
                Console.WriteLine("Invalid choice!");
            }
        }
    }
}

