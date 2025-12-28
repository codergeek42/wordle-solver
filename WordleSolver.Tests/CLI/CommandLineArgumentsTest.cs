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

using CommandLineArgumentsParseTestCase = (
    string CaseName,
    string DictionaryArgument,
    string ExcludedWordsArgument,
    CommandLineOptions ExpectedCommandLineOptions
);

using Moq;

using AwesomeAssertions;

using WordleSolver.CLI;

namespace WordleSolver.Tests.CLI;


/// <summary>
/// Unit tests for <see cref="CommandLineArguments"/>
/// </summary>
public class CommandLineArgumentsTest : IDisposable
{
    public Mock<Action<int>> MockExitCallback { get; set; } = new();

    private StringWriter MockConsoleOutput { get; set; } = new();

    public CommandLineArgumentsTest()
    {
        MockExitCallback.Setup(call => call(It.IsAny<int>()));
        Console.SetOut(MockConsoleOutput);
    }

    public void Dispose()
    {
        MockExitCallback.Reset();
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }

    public static CommandLineArgumentsParseTestCase ParseDefaultOptionsTestCase = (
        CaseName: "Default (empty) arguments",
        DictionaryArgument: "",
        ExcludedWordsArgument: "",
        ExpectedCommandLineOptions: new CommandLineOptions(
            dictionaryFilePath: new FileInfo("/usr/share/dict/words"),
            excludedWordsFilePath: null
        )
    );

    public static CommandLineArgumentsParseTestCase ParseSpecifiedOptionsTestCase = (
        CaseName: "Specific arguments",
        DictionaryArgument: ".",
        ExcludedWordsArgument: "..",
        ExpectedCommandLineOptions: new CommandLineOptions(
            dictionaryFilePath: new FileInfo("."),
            excludedWordsFilePath: new FileInfo("..")
        )
    );

    public static IEnumerable<object[]> ParseTestData()
    {
        IEnumerable<CommandLineArgumentsParseTestCase> testCases = [
            ParseDefaultOptionsTestCase,
            ParseSpecifiedOptionsTestCase
        ];

        foreach (var (CaseName, DictionaryArgument, ExcludedWordsFilePath, ExpectedCommandLineOptions) in testCases)
        {
            yield return [CaseName, DictionaryArgument, ExcludedWordsFilePath, ExpectedCommandLineOptions];
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
        CommandLineOptions expectedCommandLineOptions
    )
    {
        string[] arguments = [
            ..UseArgumentIfNotEmpty(dictionaryArgument, "--dictionary"),
            ..UseArgumentIfNotEmpty(excludedWordsArgument, "--excluded-words")
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
            $"exit with {ExitCode.InvalidCommandLineArguments}"
        );
    }
}