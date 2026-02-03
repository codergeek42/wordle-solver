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

// TODO: Consolidate this with the CLI GuessLoop?
namespace WordleSolver.Playwright;

using System.IO.Abstractions;

using WordleSolver.CLI;
using WordleSolver.Library;
using WordleSolver.Library.GuesserStrategies;


/// <summary>
/// The main guesser loop of the CLI app.
/// </summary>
public class BrowserGuessLoop
{
    public WordList CandidateWordList { get; private set; }
    public WordleSolver Solver { get; private set; }
    public GuessPage GuessPage { get; private set; }
    public string? StartingGuess { get; set; }


    /// <summary>
    /// Constructor: initializes with empty WordList and Solver instances.
    /// </summary>
    public BrowserGuessLoop(GuessPage guessPage)
    {
        CandidateWordList = new WordList([]);
        Solver = new(CandidateWordList, new NextWordGuesserStrategyFactory());
        GuessPage = guessPage;
    }

    /// <summary>
    /// Initializes a GuessLoop with the given CLI options. 
    /// </summary>
    /// <param name="cliOpts">Command-line options to use, including dictionary and excluded words files.</param>
    /// <returns>A GuessLoop to run the main solver loop logic.</returns>
    public static async Task<BrowserGuessLoop> InitializeAsync(CommandLineOptions cliOpts, GuessPage guessPage)
    {
        BrowserGuessLoop guesserLoop = new(guessPage);
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
        guesserLoop.StartingGuess = cliOpts.StartingWord;
        return guesserLoop;
    }

    /// <summary>
    /// Main guess-and-check loop: Calculates the next optimal guess, prompts the user for its result,
    /// calls the solver to processes the resulting letter-position rules, and repeats until either
    /// the word is solved or the solution can no longer be determined (for instance, if the result was
    /// mistakenly entered and caused all remaining words to be excluded).
    /// </summary>
    public async Task RunGuessLoopAsync()
    {
        Console.WriteLine($"starting guess: {StartingGuess}");
        int guessCount = 0;
        Queue<(string nextGuessStrategy, WordGuessAndScore wordAndScore)> initialManualGuesses = [];
        if (!string.IsNullOrEmpty(StartingGuess))
        {
            initialManualGuesses.Enqueue(("invoked argument", new WordGuessAndScore(StartingGuess, 0.0)));
        }

        new List<string>([
           /* Add word sequence here for testing as needed... */
        ]).ConvertAll(
            (string word) => ("(test)", new WordGuessAndScore(word, 0.0))
        ).ForEach(
            initialManualGuesses.Enqueue
        );

        while (Solver.HasSolution())
        {
            Console.WriteLine("Calculating next word. This may take some time! ...");
            (string _, WordGuessAndScore wordAndScore) = initialManualGuesses.Count > 0
                ? initialManualGuesses.Dequeue()
                : Solver.GuessNextWord();
            Console.WriteLine($"Guessing {wordAndScore.Word} ...");
            await GuessPage.SubmitGuessAsync(wordAndScore.Word);
            bool wasValidGuess = !await GuessPage.NotInWordListMessage.IsVisibleAsync();
            if (wasValidGuess)
            {
                guessCount += 1;
                Console.WriteLine("...was a valid guess. Parsing result row...");
                WordGuessAndResult wordGuessAndResult = await GuessPage.ParseGuessRowAsync(guessCount);
                Solver.WithPreviousGuess(wordGuessAndResult);
                foreach (LetterAtPositionInWordRule resultRule in wordGuessAndResult.Result)
                {
                    Console.WriteLine($"{resultRule.Letter} @ {resultRule.Position} is {resultRule.Required}.");
                }
                if (Solver.IsSolved())
                {
                    var solutionGuess = Solver.GuessNextWord();
                    Console.WriteLine($"Candidate solution found by {solutionGuess.GuesserStrategy}: {solutionGuess.GuessAndScore.Word}; checking...");
                    if (solutionGuess.GuessAndScore.Word == wordGuessAndResult.Word)
                    {
                        Console.WriteLine("Solution found: matched most recent guess!");
                    }
                    else
                    {
                        Console.WriteLine("...guessing as final!");
                        await GuessPage.SubmitGuessAsync(solutionGuess.GuessAndScore.Word);
                    }
                    return;
                }
            }
            else
            {
                Console.WriteLine("...was not a valid guess.");
                Solver.WithPreviousGuess(new WordGuessAndResult(wordAndScore.Word, [], false));
                foreach (var _ in wordAndScore.Word)
                {
                    await GuessPage.OnScreenKeyboardBackspace.ClickAsync();
                }
            }
        }
        if (!Solver.HasSolution())
        {
            Console.WriteLine($"All possible words exhausted after {guessCount} guesses; no solution found!");
        }
    }
}
