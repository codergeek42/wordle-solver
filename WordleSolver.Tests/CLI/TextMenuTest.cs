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

using GetItemAtIndexTestCase = (
    TextMenuItemSelector ItemSelector,
    string ExpectedLineItemNine
);

using ParseChoiceToEntriesIndexTestCase = (
    TextMenuItemSelector ItemSelector,
    string? Choice,
    int ExpectedIndex
);

/// <summary>
/// Unit tests for TextMenu.
/// </summary>
[Trait("Category", "Unit")]
public class TextMenuTest : MockConsoleFixture
{
    public static GetItemAtIndexTestCase GetItemAtIndexAutoNumberedTestCase = (
        ItemSelector: TextMenuItemSelector.AutoNumbered,
        ExpectedLineItemNine: "( 9) ninth"
    );

    public static GetItemAtIndexTestCase GetItemAtIndexFirstLetterTestCase = (
        ItemSelector: TextMenuItemSelector.FirstLetter,
        ExpectedLineItemNine: "(N)inth"
    );

    public static IEnumerable<object[]> GetItemAtIndexTestData()
    {
        IEnumerable<GetItemAtIndexTestCase> testCases = [
            GetItemAtIndexAutoNumberedTestCase,
            GetItemAtIndexFirstLetterTestCase
        ];

        foreach (var (ItemSelector, ExpectedLineItemNine) in testCases)
        {
            yield return [ItemSelector, ExpectedLineItemNine];
        }
    }
    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexAutoNumberedValidTestCase = (
        ItemSelector: TextMenuItemSelector.AutoNumbered,
        Choice: "2",
        ExpectedIndex: 1
    );

    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexFirstLetterValidTestCase = (
        ItemSelector: TextMenuItemSelector.FirstLetter,
        Choice: "s",
        ExpectedIndex: 1
    );

    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexAutoNumberedInvalidTestCase = (
        ItemSelector: TextMenuItemSelector.AutoNumbered,
        Choice: "0",
        ExpectedIndex: -1
    );

    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexAutoNumberedInvalidTestCaseUnderscore = (
        ItemSelector: TextMenuItemSelector.AutoNumbered,
        Choice: "_",
        ExpectedIndex: -1
    );


    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexFirstLetterInvalidTestCaseUnderscore = (
        ItemSelector: TextMenuItemSelector.FirstLetter,
        Choice: "_",
        ExpectedIndex: -1
    );

    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexFirstLetterInvalidTestCaseLetter = (
        ItemSelector: TextMenuItemSelector.FirstLetter,
        Choice: "n",
        ExpectedIndex: -1
    );

    public static ParseChoiceToEntriesIndexTestCase ParseChoiceToEntriesIndexInvalidTestCaseNull = (
        ItemSelector: TextMenuItemSelector.FirstLetter,
        Choice: null,
        ExpectedIndex: -1
    );



    public static IEnumerable<object[]> ParseChoiceToEntriesTestData()
    {
        IEnumerable<ParseChoiceToEntriesIndexTestCase> testCases = [
            ParseChoiceToEntriesIndexAutoNumberedValidTestCase,
            ParseChoiceToEntriesIndexFirstLetterValidTestCase,
            ParseChoiceToEntriesIndexAutoNumberedInvalidTestCase,
            ParseChoiceToEntriesIndexFirstLetterInvalidTestCaseLetter,
            ParseChoiceToEntriesIndexAutoNumberedInvalidTestCaseUnderscore,
            ParseChoiceToEntriesIndexFirstLetterInvalidTestCaseUnderscore,
            ParseChoiceToEntriesIndexInvalidTestCaseNull
        ];

        foreach (var (ItemSelector, Choice, ExpectedIndex) in testCases)
        {
            // NB: Having a null Choice in the last test item is intentional, to check that error
            // condition in the initial IsNullOrEmpty guard.
            yield return [ItemSelector, Choice!, ExpectedIndex];
        }
    }


    [Fact]
    public void TextMenu_Constructor_CanBeInstantiated()
    {
        string title = "test-title";
        TextMenu testMenu = new(title);

        testMenu.Should()
            .NotBeNull("should be instantiated")
            .And.BeOfType<TextMenu>("should be of the correct type");
        testMenu.Callbacks.Should()
            .NotBeNull("should have Callbacks instantiated")
            .And.BeOfType<List<Func<Task>>>("should have Callbacks of the correct type")
            .And.BeEmpty("should initialize with empty Callbacks list");
        testMenu.Items.Should()
            .NotBeNull("should have Items instantiated")
            .And.BeOfType<List<string>>("should have Items of the correct type")
            .And.BeEmpty("should initialize with empty Items list");
        testMenu.Options.Should()
           .NotBeNull("should have Options instantiated")
           .And.BeOfType<TextMenuOptions>("should have Options of the correct type");
        testMenu.Prompt.Should()
            .NotBeNull("should have Prompt instantiated")
            .And.BeOfType<string>("should have Prompt of the correct type")
            .And.Be("?", "should have Prompt be its default");
        testMenu.Title.Should()
            .NotBeNull("should have Title instantiated")
            .And.BeOfType<string>("should have Title of the correct type")
            .And.Be(title, "should have Title be set from the constructor parameter");

    }

    [Fact]
    public async Task TextMenu_WithItem_AddsItemAndReturnsCaller()
    {
        TextMenu testMenu = new TextMenu("test-title");

        testMenu.Items.Should()
            .BeEmpty("Items list should be empty at test start");
        testMenu.Callbacks.Should()
            .BeEmpty("Callbacks list should be empty at test start");

        string itemDescription = "test-item-description-sync";
        bool callbackCalled = false;
        Action syncCallbackToAdd = () => { callbackCalled = true; };

        TextMenu result = testMenu.WithItem(itemDescription, syncCallbackToAdd);

        result.Should()
            .BeSameAs(testMenu, "should return modified calling object");

        result.Items.Should()
            .ContainSingle()
            .Which.Should()
                .Be(itemDescription, "should store given item description");

        result.Callbacks.Should()
            .ContainSingle()
            .Which.Should()
                .BeOfType<Func<Task>>("should be an async wrapper around the given callback");

        await result.Callbacks[0]();
        callbackCalled.Should()
            .BeTrue("async-ified callback should be stored");
    }

    [Fact]
    public async Task TextMenu_WithAsyncItem_AddsItemAndReturnsCaller()
    {
        TextMenu testMenu = new TextMenu("test-title");

        testMenu.Items.Should()
            .BeEmpty("Items list should be empty at test start");
        testMenu.Callbacks.Should()
            .BeEmpty("Callbacks list should be empty at test start");

        string itemDescription = "test-item-description-async";
        bool callbackCalled = false;
        Func<Task> asyncCallbackToAdd = async () => await Task.Run(() => callbackCalled = true);

        TextMenu result = testMenu.WithAsyncItem(itemDescription, asyncCallbackToAdd);

        result.Should()
            .BeSameAs(testMenu, "should return modified calling object");

        result.Items.Should()
            .ContainSingle()
            .Which.Should()
                .Be(itemDescription, "should store given item description");

        result.Callbacks.Should()
            .ContainSingle()
            .Which.Should()
                .BeSameAs(asyncCallbackToAdd, "should store given async callback");

        await result.Callbacks[0]();

        callbackCalled.Should()
            .BeTrue("callback should be stored async as-is");
    }

    [Fact]
    public void TextMenu_WithOptions_LeavesUnspecifiedOptionsUnchangedAndReturnsCaller()
    {
        TextMenu testMenu = new("test-title");
        TextMenuOptions originalOptions = testMenu.Options;
        TextMenu result = testMenu.WithOptions(new() { });

        result.Should()
            .BeSameAs(testMenu, "should return calling object");
        result.Options.Should()
            .BeEquivalentTo(originalOptions, "should retain original options");
    }

    [Fact]
    public void TextMenu_WithOptions_ChangesGivenOptionsAndReturnsCaller()
    {
        TextMenuOptions originalOptions = new()
        {
            IsMultiline = true,
            ItemSelector = TextMenuItemSelector.AutoNumbered
        };
        TextMenuOptions modifiedOptions = new()
        {
            IsMultiline = false,
            ItemSelector = TextMenuItemSelector.FirstLetter
        };
        TextMenu testMenu = new("test-title");
        TextMenu menuWithOriginalOptions = testMenu.WithOptions(originalOptions);
        menuWithOriginalOptions.Should()
            .BeSameAs(testMenu, "should return modified caller");
        menuWithOriginalOptions.Options.Should()
            .BeEquivalentTo(originalOptions, "should store original options");

        TextMenu result = testMenu.WithOptions(modifiedOptions);

        result.Should()
            .BeSameAs(testMenu, "should return calling object");
        result.Options.Should()
            .BeEquivalentTo(modifiedOptions, "should retain original options");
    }

    [Fact]
    public void TextMenu_WithPrompt_StoresPromptAndReturnsCaller()
    {
        TextMenu testMenu = new("test-title");
        string expectedPrompt = "test prompt";
        TextMenu result = testMenu.WithPrompt(expectedPrompt);

        result.Should()
            .BeSameAs(testMenu, "should return modified caller");
        result.Prompt.Should()
            .BeOfType<string>("should be of correct prompt type")
            .And.NotBeNullOrEmpty("should be non-empty")
            .And.Be(expectedPrompt, "should store the given prompt");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void TextMenu_GetItemAtIndex_ThrowsIfInvalidIndex(int index)
    {
        // Empty Items
        TextMenu testMenu = new TextMenu("test-title");
        Action testCall = () => testMenu.GetItemAtIndex(index);
        testCall.Should()
            .Throw<IndexOutOfRangeException>("should throw exception for invalid index");
    }

    [Theory]
    [MemberData(nameof(GetItemAtIndexTestData))]
    public void TextMenu_GetItemAtIndex_GetsNinthItemOfTen(TextMenuItemSelector itemSelector, string expectedLine)
    {
        TextMenu testMenuWithTenItems = new TextMenu("ten items test");
        List<string> tenItems = [
            ..8.Repeat(_ => "up-to-eighth"),
            "ninth",
            "tenth"
        ];
        testMenuWithTenItems.Items.AddRange(tenItems);
        string result = testMenuWithTenItems.WithOptions(new()
        {
            ItemSelector = itemSelector
        }).GetItemAtIndex(8);

        result.Should()
            .NotBeNullOrEmpty("should be non-empty string")
            .And.Be(expectedLine, "should match expected line");
    }

    [Fact]
    public void TextMenu_GetItemAtIndex_ThrowsIfInvalidItemSelector()
    {
        TextMenu testMenu = new TextMenu("one item test");
        testMenu.Items.Add("single item");
        testMenu.Options = new()
        {
            ItemSelector = null
        };

        Action testCall = () => testMenu.GetItemAtIndex(0);
        testCall.Should()
            .Throw<TextMenuException>("should throw if the item selector is invalid");
    }

    [Fact]
    public void TextMenu_GetItemAtIndex_ThrowsIfFirstLetterSelectorAndItemNotAlphanumeric()
    {
        TextMenu testMenu = new TextMenu("one item test");
        testMenu.Items.Add("+++");
        testMenu.Options = new()
        {
            ItemSelector = TextMenuItemSelector.FirstLetter
        };

        Action testCall = () => testMenu.GetItemAtIndex(0);
        testCall.Should()
            .Throw<TextMenuException>(
                "should throw if the item selector is FirsLetter and item has no alphanumeric characters"
            );
    }

    [Theory]
    [MemberData(nameof(ParseChoiceToEntriesTestData))]
    public void TextMenu_ParseChoiceToEntriesIndex_ParsesSecondOrInvalidChoiceToIndex(
        TextMenuItemSelector itemSelector,
        string choice,
        int expectedIndex
    )
    {
        TextMenu testMenu = new TextMenu("test-title");
        testMenu.Items.AddRange([
            "first",
            "second",
            "third"
        ]);
        testMenu.Options = new()
        {
            ItemSelector = itemSelector
        };

        int result = testMenu.ParseChoiceToEntriesIndex(choice);

        result.Should()
            .Be(expectedIndex, "should match expected index");

    }

    [Fact]
    public void TextMenu_ParseChoiceToEntriesIndex_ThrowsIfInvalidSelector()
    {
        TextMenu testMenu = new TextMenu("test-title");
        testMenu.Items.AddRange([
            "first",
            "second",
            "third"
        ]);
        testMenu.Options = new()
        {
            ItemSelector = (TextMenuItemSelector)(-1)
        };

        Action testCall = () => testMenu.ParseChoiceToEntriesIndex("f");

        testCall.Should()
                .Throw<TextMenuException>("should throw if invalid selector");
    }

    [Fact]
    public async Task TextMenu_RunPrompt_ThrowsIfMismatchedItemsAndCallbacks()
    {
        TextMenu testMenu = new("test-title");
        testMenu.Items.AddRange(["first", "second"]);
        testMenu.Callbacks.Add(async () => await Task.Run(() => { }));

        testMenu.Items.Should()
            .NotHaveSameCount(testMenu.Callbacks, "Items and Callbacks in test menu should differ in count");

        Func<Task> testCall = async () => await testMenu.RunPrompt();

        await testCall.Should()
                .ThrowAsync<TextMenuException>("should throw when the Items and Callbacks have mismatched counts");
    }

    [Fact]
    public async Task TextMenu_RunPrompt_ThrowsIfEmpty()
    {
        TextMenu testMenu = new("test-title");
        testMenu.Items.Should()
            .NotBeNull("Items in test menu should be empty instance")
            .And.BeEmpty("Items in test menu should differ in count");

        Func<Task> testCall = async () => await testMenu.RunPrompt();

        await testCall.Should()
            .ThrowAsync<TextMenuEmptyException>("should throw when the Items is empty");
        MockConsoleOutput.ToString().Should()
            .BeEmpty("should not output anything");
        MockConsoleError.ToString().Should()
            .BeEmpty("should not output any errors");
    }
}