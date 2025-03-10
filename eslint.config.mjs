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

import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';
import { default as pluginTsdoc } from 'eslint-plugin-tsdoc';

export default [
    { languageOptions: { globals: globals.browser } },
    pluginJs.configs.recommended,
    ...tseslint.configs.recommended,
    ...tseslint.configs.stylistic,
    {
        plugins: {
            tsdoc: pluginTsdoc
        },
        rules: {
            // If I want the openness of an interface instead of a type for object definitions,
            // then I'll make the thing an interface. Otherwise this recommendation is silly IMHO.
            // Make it a warning, so it's still visible just in case; but that it doesn't block CI.
            '@typescript-eslint/consistent-type-definitions': 'warn',

            // Sometimes it's good practice to have placeholder variables that are then unused,
            // such as in mock functions, parameter ordering, etc. to ensure that the right type
            // and/or count of parameters are matched, even if they are not necessary for the
            // logic, such as iteration callbacks and mappings.
            '@typescript-eslint/no-unused-vars': [
                'error',
                {
                    argsIgnorePattern: '^_[^_].*$|^_$',
                    varsIgnorePattern: '^_[^_].*$|^_$',
                    caughtErrorsIgnorePattern: '^_[^_].*$|^_$'
                }
            ],
            'tsdoc/syntax': 'warn'
        }
    }
];
