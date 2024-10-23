/*
 * wordle-solver: A clever algorithm and automated tool to solve the
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
 * along with this program, namely the "LICENSE" text file.  If not,
 * see <https://www.gnu.org/licenses/gpl-3.0.html>.
 */

import mockConsole from 'jest-mock-console';
import { helloWorld, goodByeWorld } from '../src/index';
import 'jest-extended';

describe('index', () => {
    let restoreConsole: ReturnType<typeof mockConsole>;

    beforeEach(() => {
        restoreConsole = mockConsole();
    });

    afterEach(() => {
        restoreConsole();
    });

    describe('index', () => {
        it.each([
            {
                caseName: 'helloWorld',
                method: helloWorld,
                expectedOutput: 'Hello, World!'
            },
            {
                caseName: 'goodByeWorld',
                method: goodByeWorld,
                expectedOutput: 'Goodbye, World! ...'
            }
        ])('$caseName', async ({ method, expectedOutput }) => {
            const methodCall = method();

            expect(methodCall).resolves.not.toThrow();
            expect(console.log).toHaveBeenCalledTimes(1);
            expect(console.log).toHaveBeenCalledWith(expectedOutput);
        });
    });
});
