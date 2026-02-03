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
namespace WordleSolver.CLI;

using System.IO.Abstractions;

using WordleSolver.Library;
using WordleSolver.Library.Extensions;
using WordleSolver.Library.GuesserStrategies;


/// <summary>
/// The main guesser loop of the CLI app.
/// </summary>
public class GuessLoop
{
    public WordList CandidateWordList { get; private set; }
    public WordleSolver Solver { get; private set; }


    /// <summary>
    /// Constructor: initializes with empty WordList and Solver instances.
    /// </summary>
    public GuessLoop()
    {
        CandidateWordList = new WordList([]);
        Solver = new(CandidateWordList, new NextWordGuesserStrategyFactory());
    }

    /// <summary>
    /// Initializes a GuessLoop with the given CLI options. 
    /// </summary>
    /// <param name="cliOpts">Command-line options to use, including dictionary and excluded words files.</param>
    /// <returns>A GuessLoop to run the main solver loop logic.</returns>
    public static async Task<GuessLoop> InitializeAsync(CommandLineOptions cliOpts)
    {
        GuessLoop guesserLoop = new();
        FileSystem fileSystem = new();
        Console.WriteLine("Populating word list...");

        guesserLoop.CandidateWordList = await WordList.FromFileAsync(
            cliOpts.DictionaryFilePath.FullName, fileSystem
        );

        if (cliOpts.ExcludedWordsFilePath?.Exists ?? false)
        {
            guesserLoop.CandidateWordList = (WordList)guesserLoop.CandidateWordList.WithExcludedWords(
                [.. await fileSystem.File.ReadAllLinesAsync(cliOpts.ExcludedWordsFilePath.FullName)]
            );
        }
        guesserLoop.Solver = new(guesserLoop.CandidateWordList, new NextWordGuesserStrategyFactory());
        return guesserLoop;
    }


    /// <summary>
    /// Prompts the user for the result of each letter of the guess at its corresponding position, and
    /// creates a list of letter-position rules matching their input.
    /// </summary>
    /// <param name="guess">The previously-guessed word.</param>
    /// <returns>The created list of letter-position rules.</returns>
    public async Task<List<LetterAtPositionInWordRule>> GetResultingRulesFromGuessAsync(string guess)
    {
        List<LetterAtPositionInWordRule> letterRules = [];
        List<LetterWithPosition> solvedPositions = Solver.SolvedPositions();
        foreach (LetterWithPosition lwp in guess.Enumerate())
        {
            if (solvedPositions.Contains(lwp))
            {
                Console.WriteLine($"{lwp.Letter} @ index {lwp.Position} is... Required by previous guess!");
                continue;
            }
            TextMenu letterMenu = new TextMenu($"{lwp.Letter} @ index {lwp.Position} is...")
                .WithItem("...Required here", () =>
                {
                    letterRules.Add(new LetterAtPositionInWordRule(
                        lwp.Position,
                        lwp.Letter,
                        LetterAtPositionInWord.Mandatory
                    ));
                })
                .WithItem("Misplaced here", () =>
                {
                    letterRules.Add(new LetterAtPositionInWordRule(
                        lwp.Position,
                        lwp.Letter,
                        LetterAtPositionInWord.Misplaced
                    ));
                })
                .WithItem("Impossible anywhere in the word", () =>
                {
                    letterRules.Add(new LetterAtPositionInWordRule(
                        null,
                        lwp.Letter,
                        LetterAtPositionInWord.Impossible
                    ));
                })
                .WithPrompt("?")
                .WithOptions(new()
                {
                    IsMultiline = false,
                    ItemSelector = TextMenuItemSelector.FirstLetter
                });
            await letterMenu.RunPromptAsync();
        }
        return letterRules;
    }

    /// <summary>
    /// Main guess-and-check loop: Calculates the next optimal guess, prompts the user for its result,
    /// calls the solver to processes the resulting letter-position rules, and repeats until either
    /// the word is solved or the solution can no longer be determined (for instance, if the result was
    /// mistakenly entered and caused all remaining words to be excluded).
    /// </summary>
    public async Task RunGuessLoopAsync()
    {
        int guessCount = 0;
        while (Solver.HasSolution())
        {
            Console.WriteLine("Calculating next word. This may take some time! ...");
            (string nextGuessStrategy, WordGuessAndScore wordAndScore) = Solver.GuessNextWord();
            Console.WriteLine($"{wordAndScore.Word} ({wordAndScore.Score} by {nextGuessStrategy}):");
            if (Solver.IsSolved())
            {
                Console.WriteLine("...is the solution!");
                break;
            }
            TextMenu isWordPossibleMenu = new TextMenu($"Is {wordAndScore.Word} a possible solution?")
                .WithItem("Yes", async () =>
                {
                    guessCount += 1;
                    List<LetterAtPositionInWordRule> letterRules = await GetResultingRulesFromGuessAsync(wordAndScore.Word);
                    Solver.WithPreviousGuess(new WordGuessAndResult(wordAndScore.Word, letterRules));
                })
                .WithItem("No", () =>
                {
                    Solver.WithPreviousGuess(new WordGuessAndResult(wordAndScore.Word, [], false));
                })
                .WithItem("Solved!", () =>
                {
                    Solver.WithPreviousGuess(new WordGuessAndResult(
                        wordAndScore.Word, wordAndScore.Word.Enumerate().ConvertAll(
                            lwp => new LetterAtPositionInWordRule(
                                lwp.Position, lwp.Letter, LetterAtPositionInWord.Mandatory
                            )
                        )
                    ));
                })
                .WithPrompt("?")
                .WithOptions(new()
                {
                    IsMultiline = false,
                    ItemSelector = TextMenuItemSelector.FirstLetter
                });
            await isWordPossibleMenu.RunPromptAsync();
        }
        if (!Solver.HasSolution())
        {
            Console.WriteLine($"All possible words exhausted after {guessCount} guesses; no solution found!");
            Console.WriteLine("(Did you typo the guess results somewhere, perhaps?)");
        }
        else // IsSolved
        {
            guessCount += 1;
            Console.WriteLine($"Wordle solved in {guessCount} guesses!");
        }
    }

}
