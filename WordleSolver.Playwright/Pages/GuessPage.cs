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
 * see <https://www.gnu.org/licenses/gpl-3.0     .html>.
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Playwright;

using WordleSolver.Library;
using WordleSolver.Library.Extensions;

namespace WordleSolver.Playwright;
/// <summary>
/// The main Wordle guess-and-loop page.
/// </summary>
public class GuessPage(IPage Page)
{
    public ILocator NotInWordListMessage => Page.GetByText("Not in word list");

    public ILocator SettingsButton => Page.GetByTestId("settings-button");

    public ILocator SettingsModal => Page.GetByLabel("settings Modal");
    public ILocator DarkModeToggle => SettingsModal.GetByLabel("Dark Mode");
    public ILocator HardModeToggle => SettingsModal.GetByLabel("Hard Mode");
    public ILocator CloseSettingsModalButton => SettingsModal.GetByLabel("Close");


    public ILocator OnScreenKeyboard => Page.GetByLabel("Keyboard");
    public ILocator OnScreenKeyboardBackspace => OnScreenKeyboard.GetByLabel("backspace");
    public ILocator OnScreenKeyboardEnter => OnScreenKeyboard.GetByLabel("enter");


    public ILocator LetterKey(char letter) => OnScreenKeyboard.GetByLabel($"add {char.ToLower(letter)}");


    public async Task OpenSettingsModalAsync()
    {
        if (!await SettingsModal.IsVisibleAsync())
        {
            await SettingsButton.ClickAsync();
        }
    }

    public async Task CloseSettingsModalAsync()
    {
        if (await SettingsModal.IsVisibleAsync())
        {
            await CloseSettingsModalButton.ClickAsync();
        }
    }

    public async Task<bool> GetSettingToggleAsync(ILocator settingsModalToggle)
    {
        await OpenSettingsModalAsync();
        bool toggleValue = await settingsModalToggle.IsCheckedAsync();
        await CloseSettingsModalAsync();
        return toggleValue;
    }

    public async Task SetSettingToggleAsync(ILocator settingsModalToggle, bool isToggledOn)
    {
        await OpenSettingsModalAsync();
        await settingsModalToggle.SetCheckedAsync(isToggledOn);
        await CloseSettingsModalAsync();
    }

    public async Task WaitForIdleAnimationAsync(ILocator tileLocator)
    {
        async Task<bool> IsAnimationIdleAsync()
        {
            string? animationStatus = await tileLocator.GetAttributeAsync("data-animation");
            return string.IsNullOrEmpty(animationStatus)
                ? false
                : animationStatus == "idle";
        }

        do { } while (!await IsAnimationIdleAsync());
    }

    public async Task<LetterAtPositionInWordRule> ParseGuessedLetterAsync(ILocator guessedLetter, int position)
    {
        Dictionary<string, LetterAtPositionInWord> LetterDataStateRequiredMap = new()
        {
            { "absent", LetterAtPositionInWord.Impossible },
            { "correct", LetterAtPositionInWord.Mandatory },
            { "present", LetterAtPositionInWord.Misplaced }
        };

        await WaitForIdleAnimationAsync(guessedLetter);
        char letter = (await guessedLetter.InnerTextAsync()).ToUpper().First();
        string? letterDataState = await guessedLetter.GetAttributeAsync("data-state");
        bool validLetterState = LetterDataStateRequiredMap.TryGetValue(
            letterDataState ?? "",
            out LetterAtPositionInWord requiredFromDataState
        );
        if (!validLetterState)
        {
            throw new WordleSolverException(
                $"Guessed letter {letter} at position {position} has invalid data state {letterDataState}"
            );
        }
        return new LetterAtPositionInWordRule(position, letter, requiredFromDataState);

    }

    public async Task SubmitGuessAsync(string guess)
    {
        foreach (char letter in guess)
        {
            await LetterKey(letter).ClickAsync();
        }
        await OnScreenKeyboardEnter.ClickAsync();
    }

    public async Task<WordGuessAndResult?> ParseGuessRowAsync(int rowNum)
    {
        // TODO: Assume guess is valid; will be checked upon submission before calling this.

        ILocator guessRow = Page.GetByLabel($"Row {rowNum}");
        if (!await guessRow.IsVisibleAsync())
        {
            return null;
        }
        IReadOnlyList<ILocator> guessLetterItems = await guessRow.GetByTestId("tile").AllAsync();

        IEnumerable<Task<LetterAtPositionInWordRule>> letterAtPositionInWordRulesTasks = guessLetterItems
            .Select((ILocator guessedLetter, int position) => ParseGuessedLetterAsync(guessedLetter, position));

        // The letter-flip animation upon guess submission happens sequentially, so waiting for those must also
        // be sequential; otherwise the other letters will already have idle animation status but "tbd" state
        // attributes, making the ParseGuessedLetterAsync call erroneously throw for being invalid.
        foreach (Task<LetterAtPositionInWordRule> parseTask in letterAtPositionInWordRulesTasks)
        {
            await parseTask;
        }

        List<LetterAtPositionInWordRule> resultRules = letterAtPositionInWordRulesTasks
            .Select(finishedTask => finishedTask.Result)
            .ToList();

        string guessedWord = string.Join(string.Empty, resultRules.Select(rule => rule.Letter));

        // FIXME
        return new WordGuessAndResult(guessedWord, resultRules, true);
    }
}