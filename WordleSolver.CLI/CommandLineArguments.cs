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

using System.CommandLine;

namespace WordleSolver.CLI;

/// <summary>
/// Command-line arguments parsed from how the app was invoked.
/// </summary>
/// <param name="DictionaryFilePath">The dictionary (file of all possible words, one per line).</param>
/// <param name="ExcludedWordsFilePath">The list of words to exclude (one per line).</param>
public record CommandLineOptions(FileInfo DictionaryFilePath, FileInfo? ExcludedWordsFilePath);

/// <summary>
/// Command-line argument parser.
/// </summary>
public class CommandLineArguments
{
    /// <summary>
    /// Given the command-line arguments, parses them and returns the parsed object. Calls the given
    /// exitCallback with an appropriate exit status if that results in a flow that should exit (such
    /// as showing the help or version) or a parse error (such as in )
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="exitCallback"></param>
    /// <returns>The parsed command-line options object.</returns>
    /// <exception cref="WordleSolverUnterminatedExitException">If the given exitCallback returned without exiting.</exception>
    public static CommandLineOptions Parse(string[] arguments, Action<int> exitCallback)
    {
        Option<FileInfo> dictionaryFilePathOption = new("--dictionary", "-d")
        {
            DefaultValueFactory = _ => new FileInfo("/usr/share/dict/words"),
            Description = "File to use as the dictionary of words, one per line.",
            HelpName = "/path/to/dictionary/file",
        };
        Option<FileInfo> excludedWordsFilePathOption = new("--excluded-words", "-x")
        {
            Description = "File with words to exclude from the guesser, one per line.",
            HelpName = "/path/to/excluded/words/file"
        };

        RootCommand rootCommand = new(LegalTexts.AppTitle) {
            dictionaryFilePathOption.AcceptExistingOnly(),
            excludedWordsFilePathOption.AcceptExistingOnly()
        };

        ParseResult parseResult = rootCommand.Parse(arguments);
        // Automagic handling of -h/--help and such.
        parseResult.Invoke(new() { });
        if (parseResult.Action?.Terminating ?? false)
        {
            // The rootCommand.Parse call will validate arguments.
            exitCallback(
                parseResult.Errors.Count == 0
                ? ExitCode.OK
                : ExitCode.InvalidCommandLineArguments
            );
            throw new WordleSolverUnterminatedExitException();
        }
        return new CommandLineOptions(
            DictionaryFilePath: parseResult.GetRequiredValue(dictionaryFilePathOption),
            ExcludedWordsFilePath: parseResult.GetValue(excludedWordsFilePathOption)
        );
    }
}
