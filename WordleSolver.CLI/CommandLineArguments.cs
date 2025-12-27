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
using System.CommandLine.Parsing;

using WordleSolver.CLI;
using WordleSolver.Library;

public struct CommandLineOptions
{
    public string DictionaryFilePath { get; set; }

    public string? ExcludedWordsFilePath { get; set; }
}

public class CommandLineArguments
{
    public static CommandLineOptions Parse(string[] arguments)
    {
        Option<string> dictionaryFilePathOption = new("--dictionary", "-d")
        {
            DefaultValueFactory = _ => "/usr/share/dict/words",
            Description = "File to use as the dictionary of words, one per line.",
            HelpName = "/path/to/dictionary/file"
        };
        Option<string> excludedWordsFilePathOption = new("--excluded-words", "-x")
        {
            DefaultValueFactory = _ => ".wordle-solver-excluded-words.txt",
            Description = "File with words to exclude from the guesser, one per line.",
            HelpName = "/path/to/excluded/words/file"
        };

        RootCommand rootCommand = new(LegalTexts.AppTitle) {
            dictionaryFilePathOption,
            excludedWordsFilePathOption
        };

        ParseResult parseResult = rootCommand.Parse(arguments);
        // Automagic handling of -h/--help and such.
        parseResult.Invoke(new()
        {

        });
        if (parseResult.Action?.Terminating ?? false)
        {
            Environment.Exit(0);
        }
        if (
            parseResult.Errors.Count == 0
            && parseResult.GetValue(dictionaryFilePathOption) is string dictionaryFilePath
            && parseResult.GetValue(excludedWordsFilePathOption) is string excludedWordsFilePath
        )
        {
            CommandLineOptions parsedArguments = new()
            {
                DictionaryFilePath = dictionaryFilePath,
                ExcludedWordsFilePath = excludedWordsFilePath
            };
            return parsedArguments;
        }
        else
        {
            foreach (ParseError error in parseResult.Errors)
            {
                Console.Error.WriteLine(error);
            }
            throw new WordleSolverException("Invalid command-line arguments: ");
        }
    }
}