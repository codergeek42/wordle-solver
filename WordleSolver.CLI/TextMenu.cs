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
using System.Collections.Immutable;
using System.Security.Principal;
using System.Threading.Tasks;

using WordleSolver.Library;

namespace WordleSolver.CLI;

public class TextMenuException(string message) : WordleSolverException(message);

public class TextMenuEmptyException(string message = "Menu is empty.") : TextMenuException(message);

public enum TextMenuItemSelector
{
    AutoNumbered,
    FirstLetter
}

public struct TextMenuOptions
{
    public bool IsMultiline { get; set; }
    public TextMenuItemSelector ItemSelector { get; set; }
}


/// <summary>
/// A rudimentary interactive text-based menu system for CLI usage.
/// </summary>
public class TextMenu(string title)
{

    public List<Func<Task>> Callbacks { get; private set; } = [];

    public List<string> Items { get; private set; } = [];

    public TextMenuOptions Options { get; set; } = new()
    {
        IsMultiline = true,
        ItemSelector = TextMenuItemSelector.AutoNumbered
    };

    public string Prompt { get; private set; } = "?";
    public string Title { get; private set; } = title;

    public TextMenu WithItem(string displayedText, Action callback)
    {
        Items.Add(displayedText);
        Callbacks.Add(async () => await Task.Run(callback));
        return this;
    }

    public TextMenu WithAsyncItem(string displayedText, Func<Task> asyncCallback)
    {
        Items.Add(displayedText);
        Callbacks.Add(asyncCallback);
        return this;
    }


    public TextMenu WithOptions(TextMenuOptions options)
    {
        Options = options;
        return this;
    }

    public TextMenu WithPrompt(string prompt)
    {
        Prompt = prompt;
        return this;
    }

    private string GetItemAtIndex(int index)
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
                char displayedLetter = itemAtIndex.ToUpper().First(char.IsLetterOrDigit);
                int indexOfLetter = itemAtIndex.IndexOf(displayedLetter);
                return $"{itemAtIndex[..indexOfLetter]}({displayedLetter}){itemAtIndex[(indexOfLetter + 1)..]}";
            default:
                throw new TextMenuException("Unhandled item selector.");
        }
    }

    private void DisplayMenuItemsMultiLine()
    {

        foreach (var line in Items.Index())
        {
            Console.WriteLine(GetItemAtIndex(line.Index));
        }
    }

    private void DisplayMenuItemsSingleLine()
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

    private int ParseChoiceToEntriesIndex(string choice)
    {
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
        if (Options.IsMultiline)
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
            int choice = ParseChoiceToEntriesIndex(Console.ReadLine() ?? "");
            if (choice < 0 || choice > Items.Count)
            {
                Console.WriteLine("Invalid choice!");
            }
            else
            {
                await Callbacks[choice]();
                return;
            }
        }
    }
}

