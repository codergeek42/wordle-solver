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

using Moq;

using WordleSolver.CLI;

using CommandLineArgumentsParseTestCase = (
    string CaseName,
    string DictionaryArgument,
    string ExcludedWordsArgument,
    string StartingWordArgument,
    string BrowserDarkModeArgument,
    WordleSolver.CLI.CommandLineOptions ExpectedCommandLineOptions
);

namespace WordleSolver.Tests.CLI;


/// <summary>
/// Unit tests for <see cref="CommandLineArguments"/>
/// </summary>
[Trait("Category", "Unit")]
public class CommandLineArgumentsTest : MockConsoleFixture
{
    public Mock<Action<int>> MockExitCallback { get; set; } = new();

    public CommandLineArgumentsTest()
        : base()
    {
        MockExitCallback.Setup(call => call(It.IsAny<int>()));
    }

    public override void Dispose()
    {
        base.Dispose();
        MockExitCallback.Reset();
    }

    public static CommandLineArgumentsParseTestCase ParseDefaultOptionsTestCase = (
        CaseName: "Default (empty) arguments",
        DictionaryArgument: "",
        ExcludedWordsArgument: "",
        StartingWordArgument: "",
        BrowserDarkModeArgument: "",
        ExpectedCommandLineOptions: new CommandLineOptions(
            DictionaryFilePath: new FileInfo("/usr/share/dict/words"),
            ExcludedWordsFilePath: null,
            StartingWord: null,
            BrowserDarkMode: false
        )
    );

    // NB: The AcceptExistingOnly settings in the root command for the argument parsing
    // necessitates that the file paths exist; but for the purposes of our unit tests, we
    // only need to validate that those specific paths are stored (that is, instead of the defaults),
    // not any functionality around opening/reading them.
    public static CommandLineArgumentsParseTestCase ParseSpecifiedOptionsTestCase = (
        CaseName: "Specific arguments",
        DictionaryArgument: ".",
        ExcludedWordsArgument: "..",
        StartingWordArgument: "START",
        BrowserDarkModeArgument: "--browser-dark-mode",
        ExpectedCommandLineOptions: new CommandLineOptions(
            DictionaryFilePath: new FileInfo("."),
            ExcludedWordsFilePath: new FileInfo(".."),
            StartingWord: "START",
            BrowserDarkMode: true
        )
    );

    public static IEnumerable<object[]> ParseTestData()
    {
        IEnumerable<CommandLineArgumentsParseTestCase> testCases = [
            ParseDefaultOptionsTestCase,
            ParseSpecifiedOptionsTestCase
        ];

        foreach (var (CaseName, DictionaryArgument, ExcludedWordsFilePath, StartingWordArgument, BrowserDarkMode, ExpectedCommandLineOptions) in testCases)
        {
            yield return [CaseName, DictionaryArgument, ExcludedWordsFilePath, StartingWordArgument, BrowserDarkMode, ExpectedCommandLineOptions];
        }
    }

    public static string[] UseArgumentIfNotEmpty(string? argument, string prefix)
    {
        return string.IsNullOrEmpty(argument) ? [] : [prefix, argument];
    }

    [Theory]
    [MemberData(nameof(ParseTestData))]
    public void CommandLineArguments_Parse_ShouldAssignValues(
        string caseName,
        string dictionaryArgument,
        string excludedWordsArgument,
        string? startingWordArgument,
        string browserDarkModeArgument,
        CommandLineOptions expectedCommandLineOptions
    )
    {
        string[] arguments = [
            ..UseArgumentIfNotEmpty(dictionaryArgument, "--dictionary"),
            ..UseArgumentIfNotEmpty(excludedWordsArgument, "--excluded-words"),
            ..UseArgumentIfNotEmpty(startingWordArgument, "--starting-word"),
            ..UseArgumentIfNotEmpty(browserDarkModeArgument, "--browser-dark-mode")
        ];

        CommandLineOptions result = CommandLineArguments.Parse(arguments, MockExitCallback.Object);

        result.Should()
            .NotBeNull("should have a non-null result")
            .And.BeOfType<CommandLineOptions>("should be of the correct options type")
            .And.BeEquivalentTo(
                expectedCommandLineOptions,
                options => options.Using<FileInfo>(ctx =>
                {
                    if (ctx.Expectation is null)
                    {
                        ctx.Subject.Should()
                            .BeNull();
                    }
                    else
                    {
                        ctx.Subject.FullName
                            .Should().Be(ctx.Expectation.FullName);
                    }
                }).WhenTypeIs<FileInfo>(),
                $"should match the expected arguments: {caseName}"
            );
    }


    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    [InlineData("--version")]
    public void CommandLineArguments_HelpAndVersion_ShouldExit(string argument)
    {
        Action testCallParse = () => CommandLineArguments.Parse([argument], MockExitCallback.Object);
        testCallParse.Should()
            .Throw<WordleSolverUnterminatedExitException>("callback returns");
        MockExitCallback.Verify(
            callback => callback(ExitCode.OK),
            $"exit with {ExitCode.OK} exitCode"
        );
    }

    [Theory]
    [InlineData("/does/not/exist")]
    [InlineData("")]
    public void CommandLineArguments_Invalid(string invalidFilePath)
    {
        Action testCallParse = () => CommandLineArguments.Parse(["--dictionary", invalidFilePath], MockExitCallback.Object);
        testCallParse.Should()
            .Throw<WordleSolverUnterminatedExitException>("callback returns");
        MockExitCallback
            .Verify(callback => callback(ExitCode.InvalidCommandLineArguments),
            $"exit with {ExitCode.InvalidCommandLineArguments} exitCode"
        );
    }
}
