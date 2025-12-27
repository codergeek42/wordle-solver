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

using System.IO.Abstractions;
using System.Threading.Tasks;

using WordleSolver.CLI;
using WordleSolver.Library;
using WordleSolver.Library.Extensions;
using WordleSolver.Library.GuesserStrategies;

public class GuessLoop
{
    public WordList CandidateWordList { get; private set; }
    public WordleSolver.Library.WordleSolver Solver { get; private set; }

    public GuessLoop()
    {
        CandidateWordList = new WordList([]);
        Solver = new(CandidateWordList, new NextWordGuesserStrategyFactory());
    }

    public static async Task<GuessLoop> Initialize(CommandLineOptions cliOpts)
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

    public async Task<List<LetterAtPositionInWordRule>> GetResultingRulesFromGuess(string guess)
    {
        List<LetterAtPositionInWordRule> letterRules = [];
        foreach (LetterWithPosition lwp in guess.Enumerate())
        {
            if (Solver.SolvedPositions().Contains(lwp.Position))
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
            await letterMenu.RunPrompt();
        }
        return letterRules;
    }

    public async Task RunGuessLoop()
    {
        int guessCount = 0;
        while (Solver.HasSolution())
        {
            guessCount += 1;
            Console.WriteLine("Calculating next word. This may take some time! ...");
            (string nextGuessStrategy, WordGuessAndScore wordAndScore) = Solver.GuessNextWord();
            Console.WriteLine($"{wordAndScore.Word} ({wordAndScore.Score} by {nextGuessStrategy}):");
            if (Solver.IsSolved())
            {
                Console.WriteLine("...is the solution!");
                break;
            }
            TextMenu isWordPossibleMenu = new TextMenu($"Is {wordAndScore.Word} a possible solution?")
                .WithAsyncItem("Yes", async () =>
                {
                    List<LetterAtPositionInWordRule> letterRules = await GetResultingRulesFromGuess(wordAndScore.Word);
                    Solver.WithPreviousGuess(new WordGuessAndResult(wordAndScore.Word, letterRules));
                })
                .WithItem("No", () =>
                {
                    Solver.WithPreviousGuess(new WordGuessAndResult(wordAndScore.Word, [], false));
                    guessCount -= 1;
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
            await isWordPossibleMenu.RunPrompt();
        }
        if (!Solver.HasSolution())
        {
            Console.WriteLine($"All possible words exhausted after {guessCount} guesses; no solution found!");
            Console.WriteLine("(Did you typo the guess results somewhere, perhaps?)");
        }
        else
        {
            Console.WriteLine($"Wordle solved in {guessCount} guesses!");
        }
    }

}