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

using WordleSolver.Library.GuesserStrategies;

namespace WordleSolver.Library;

public class WordleSolver(IWordList wordList, INextWordGuesserStrategyFactory guesserStrategyFactory) : IWordleSolver
{

    public List<INextWordGuesserStrategy> GuesserStrategies { get; private set; } = guesserStrategyFactory.FromWordList(wordList);

    public IWordList CandidateWordList { get; private set; } = wordList;

    public IWordleSolver WithPreviousGuess(WordGuessAndResult previousGuessAndResult)
    {

        GuesserStrategies = GuesserStrategies
            .AsParallel()
            .Select(guesserStrategy => guesserStrategy.WithPreviousGuess(previousGuessAndResult))
            .ToList();
        return this;
    }

    public (string GuesserStrategy, WordGuessAndScore GuessAndScore) GuessNextWord()
    {
        var result = GuesserStrategies
            .AsParallel()
            .Select(guesserStrategy => (
                GuesserStrategy: guesserStrategy.GetType().Name,
                GuessAndScore: guesserStrategy.GuessNextWordAndScore()
            ))
            .MaxBy(guesserStrategy => guesserStrategy.GuessAndScore.Score);
        return result;
    }
    public bool IsSolved()
    {
        return GuesserStrategies.Any(guesserStrategy => guesserStrategy.IsSolved());
    }

    public bool HasSolution()
    {
        return GuesserStrategies.All(guesserStrategy => guesserStrategy.HasSolution());
    }
}