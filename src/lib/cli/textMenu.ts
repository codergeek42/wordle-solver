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

import * as readlinePromises from 'readline/promises';

export type TextMenuEntry = {
    callback: () => void | Promise<void>;
    text: string;
};

/**
 * The menu was attempted while not fully initialized (i.e., one of the title, entries, or prompt
 * was empty).
 */
export class TextMenuNotInitializedError extends Error {}

/** A built menu, ready for prompting. */
export class TextMenu {
    constructor(
        protected title = '',
        protected entries: TextMenuEntry[] = [],
        protected prompt = ''
    ) {}

    async promptAndExecute(): Promise<void> {
        if (this.title.length <= 0 || this.entries.length <= 0 || this.prompt.length <= 0) {
            throw new TextMenuNotInitializedError('Menu has no entries.');
        }

        console.log(this.title);
        const numWidth = Math.ceil(Math.log10(this.entries.length));
        this.entries.forEach(({ text }, idx) => {
            const entryNumber = (idx + 1).toString().padStart(numWidth, ' ');
            console.log(`(${entryNumber}) ${text}\n`);
        });
        console.log(this.prompt);

        // TODO: Consider using parameterized I/O?
        const readlineInterface = readlinePromises.createInterface({
            input: process.stdin,
            output: process.stdout
        });

        let callback: undefined | (() => void) = undefined;
        while (!callback) {
            const response = await readlineInterface.question(this.prompt);
            try {
                ({ callback } = this.entries[parseInt(response) - 1]);
            } catch {
                console.warn('Oops, that was not a valid choice!');
            }
        }
        await callback();
    }
}

/**
 * A quick helper class to create, display, and repeat through a text-driven menu.
 * @example
 * ```
 *	const myMenu = new TextMenuBuilder()
 *		.withTitle('Welcome to MyApp v1.0!')
 *		.addEntry('Do the first cool thing', firstCoolFunction)
 *		.addEntry('Do the second cool thing', secondCoolFunction)
 *		.addEntry<ParamsType>('Do the third thing with parameters', thirdFunc, params)
 *		.addEntry('Exit', process.exit, 0)
 *		.withPrompt('Choose one:')
 *		.build();
 * await myMenu.promptAndExecute();
 * ```
 */
export default class TextMenuBuilder {
    title = '';
    entries: TextMenuEntry[] = [];
    prompt = '';

    addEntry({ text, callback }: TextMenuEntry): TextMenuBuilder {
        this.entries.push({
            text,
            callback
        });
        return this;
    }

    build(): TextMenu {
        return new TextMenu(this.title, this.entries, this.prompt);
    }

    withPrompt(prompt: string): TextMenuBuilder {
        this.prompt = prompt;
        return this;
    }

    withTitle(title: string): TextMenuBuilder {
        this.title = title;
        return this;
    }
}
