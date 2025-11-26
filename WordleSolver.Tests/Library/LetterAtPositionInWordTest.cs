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

// using System.Data;
// using AwesomeAssertions;
// using WordleSolver.Library;

// namespace WordleSolver.Tests;

// public class LetterAtPositionInWordTest
// {
//     public static LetterAtPositionInWordRule ImpossibleRuleA = new(null, 'A', LetterAtPositionInWord.Impossible);
//     public static LetterAtPositionInWordRule ImpossibleRuleB = new(null, 'B', LetterAtPositionInWord.Impossible);
//     public static LetterAtPositionInWordRule MisplacedRuleAAt0 = new(0, 'A', LetterAtPositionInWord.Misplaced);
//     public static LetterAtPositionInWordRule MisplacedRuleAAt1 = new(1, 'A', LetterAtPositionInWord.Misplaced);
//     public static LetterAtPositionInWordRule MandatoryBAt0 = new(0, 'B', LetterAtPositionInWord.Mandatory);
//     public static LetterAtPositionInWordRule MandatoryBAt1 = new(1, 'B', LetterAtPositionInWord.Mandatory);



//     public static IEnumerable<object[]> ComparatorTestData_Mandatory()
//     {
//         yield return [ImpossibleRuleA];
//         yield return [MisplacedRuleAAt0];
//     }

//     public static IEnumerable<object[]> ComparatorTestData_NonMandatory()
//     {
//         yield return [MandatoryBAt0, MandatoryBAt1, "two Mandatory rules"];
//         yield return [ImpossibleRuleA, ImpossibleRuleB, "two Impossible rules"];
//         yield return [MisplacedRuleAAt0, MisplacedRuleAAt1, "two Misplaced rules"];
//         yield return [MisplacedRuleAAt0, ImpossibleRuleA, "a Misplaced and an Impossible rule"];
//     }


//     [Theory]
//     [MemberData(nameof(ComparatorTestData_Mandatory))]
//     public void LetterAtPositionInWordRule_Compare_ComparesOneMandatory(LetterAtPositionInWordRule nonMandatoryRule)
//     {
//         int sortResultLeftAssoc = [MandatoryBAt0, nonMandatoryRule].OrderBy(Rule => Rule.Required);
//         int sortResultRightAssoc = nonMandatoryRule.CompareTo(MandatoryBAt0);

//         sortResultLeftAssoc.Should()
//             .BePositive("Mandatory rule should sort greater than non-Mandatory rule");
//         sortResultRightAssoc.Should()
//             .BeNegative("non-Mandatory rule should sort less than Mandatory rule");
//     }

//     [Theory]
//     [MemberData(nameof(ComparatorTestData_NonMandatory))]
//     public void LetterAtPositionInWordRule_CompareTo_ComparesAsEqual(LetterAtPositionInWordRule ruleA, LetterAtPositionInWordRule ruleB, string rulesDescription)
//     {
//         int sortResultLeftAssoc = ruleA.CompareTo(ruleB);
//         int sortResultRightAssoc = ruleB.CompareTo(ruleA);

//         sortResultLeftAssoc.Should()
//             .Be(0, $"{rulesDescription} should sort with equal precedence (left associative)");
//         sortResultRightAssoc.Should()
//             .Be(0, $"{rulesDescription} should sort with equal precedence (right associative)");
//     }

//     [Fact]
//     public void LetterAtPositionInWordRule_CompareTo_ThrowsIfNull()
//     {
//         Action testCall = () => ImpossibleRuleA.CompareTo(null);

//         testCall.Should()
//             .Throw<NullComparisonException>();
//     }
// }