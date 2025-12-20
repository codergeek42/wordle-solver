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

using WordleSolver.Library;

namespace WordleSolver.Tests;

/// <summary>
/// Unit tests for <see cref="LetterAtPositionInWordRule"/>
/// </summary>
public class LetterAtPositionInWordRuleTest
{
    [Theory]
    [InlineData(LetterAtPositionInWord.Mandatory)]
    [InlineData(LetterAtPositionInWord.Misplaced)]
    public void LetterAtPositionInWordRule_ShouldThrowIfNonImpossibleRuleHasNullPosition(LetterAtPositionInWord ruleRequired)
    {
        Action testCall = () => new LetterAtPositionInWordRule(null, 'A', ruleRequired);

        testCall.Should()
            .Throw<MissingLetterRulePositionException>("a non-Impossible rule should have an associated Position");
    }

    [Fact]
    public void LetterAtPositionInWordRule_ShouldAllowImpossibleRuleWithNullPosition()
    {
        LetterAtPositionInWordRule expected = new LetterAtPositionInWordRule(null, 'A', LetterAtPositionInWord.Impossible);
        LetterAtPositionInWordRule? result = null;
        Action testCall = () => result = new LetterAtPositionInWordRule(null, 'A', LetterAtPositionInWord.Impossible);


        testCall.Should()
            .NotThrow<MissingLetterRulePositionException>("an Impossible rule should be constructible with null position");
        result.Should()
            .NotBeNull("should return the non-null rule")
            .And.BeOfType<LetterAtPositionInWordRule>("should return the constructed rule type")
            .And.BeEquivalentTo(expected, "should have the rule letter and position set");


    }
}
