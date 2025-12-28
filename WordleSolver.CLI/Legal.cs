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

public class LegalTexts
{
    /// <summary>
    /// The title and copyright line.
    /// </summary>
    public static string AppTitle = """
    Peter's Wordle Solver
    Copyright (C) 2025 Peter Gordon <codergeek42@gmail.com>
    """;

    /// <summary>
    /// The copyleft notice, per the GNU GPL v3+.
    /// </summary>
    public static string CopyleftInformation = """
    This program is free software: you can redistribute it and / or modify it under
    the terms of the GNU General Public License as published by the Free Software
    Foundation, either version 3 of the License, or (at your option) any later
    version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
    PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with
    this program (the included LICENSE text file). If not, see
    <https: // www.gnu.org/licenses/> .
    """;

    /// <summary>
    /// The disclaimer of warranty, per the GNU GPL v3+.
    /// </summary>
    public static string DisclaimerOfWarranty = """
    THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.
    EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER
    PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER
    EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO
    THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE
    DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
    """;

    /// <summary>
    ///  The welcome banner printed at app startup, per the GNU GPL v3+.
    /// </summary>
    public static string WelcomeBanner = """
    This program comes with ABSOLUTELY NO WARRANTY; for details, select `(No)
    Warranty Information` from the main menu. This is free software, and you are
    welcome to redistribute it under certain conditions; select `Copyleft
    Information` from the main menu for details.
    """;
}
