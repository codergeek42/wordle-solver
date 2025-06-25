import { times } from 'lodash';
import TextMenuBuilder, { TextMenu, TextMenuEntry, TextMenuNotInitializedError } from '../../../src/lib/cli/textMenu';
import * as readlinePromises from 'readline/promises';

jest.mock('readline/promises');

const title = 'test title';
const prompt = 'test prompt';
const entries: TextMenuEntry[] = [
    {
        text: 'synchronous',
        callback: () => {
            return;
        }
    },
    {
        text: 'asynchronous',
        callback: async () => await Promise.resolve()
    }
];

describe(TextMenu.name, () => {
    let testTextMenu: TextMenu;
    let consoleLogSpy: jest.SpiedFunction<typeof console.log>;
    let consoleWarnSpy: jest.SpiedFunction<typeof console.warn>;
    let createInterfaceSpy: jest.SpiedFunction<typeof readlinePromises.createInterface>;
    let questionMock: jest.Mock;

    beforeEach(() => {
        testTextMenu = new TextMenu();
        consoleLogSpy = jest.spyOn(console, 'log').mockName('console.log');
        consoleWarnSpy = jest.spyOn(console, 'warn').mockName('console.warn');
        questionMock = jest.fn().mockName('question');
        createInterfaceSpy = jest
            .spyOn(readlinePromises, 'createInterface')
            .mockName('createInterface')
            .mockReturnValue({ question: questionMock } as unknown as readlinePromises.Interface);
    });

    afterEach(() => {
        jest.restoreAllMocks();
    });

    describe(TextMenu.prototype.promptAndExecute.name, () => {
        it('should throw a TextMenuNotInitializedError if title is empty', async () => {
            testTextMenu = new TextMenu('', entries, prompt);

            const testCall = testTextMenu.promptAndExecute();

            await expect(testCall).rejects.toThrow(TextMenuNotInitializedError);
        });

        it('should throw a TextMenuNotInitializedError if entries is empty', async () => {
            testTextMenu = new TextMenu(title, [], prompt);

            const testCall = testTextMenu.promptAndExecute();

            await expect(testCall).rejects.toThrow(TextMenuNotInitializedError);
        });

        it('should throw a TextMenuNotInitializedError if prompt is empty', async () => {
            testTextMenu = new TextMenu(title, entries, '');

            const testCall = testTextMenu.promptAndExecute();

            await expect(testCall).rejects.toThrow(TextMenuNotInitializedError);
        });

        it('should output the title, each entry text, prompt for input, warn if invalid, and run callback', async () => {
            const entries: TextMenuEntry[] = times(2, (idx) => ({
                text: `test entry ${idx}`,
                callback: async () => {
                    const answer = await Promise.resolve(42);
                    console.log(`callback ${idx} answer is ${answer}`);
                }
            }));

            const testTextMenu = new TextMenu(title, entries, prompt);
            questionMock
                .mockImplementationOnce((_query: string) => {
                    expect(createInterfaceSpy).toHaveBeenCalledTimes(1);
                    expect(createInterfaceSpy).toHaveBeenCalledWith({
                        input: process.stdin,
                        output: process.stdout
                    });
                    return entries.length + 1;
                })
                .mockImplementationOnce((_query: string) => {
                    expect(consoleWarnSpy).toHaveBeenCalledTimes(1);
                    expect(consoleWarnSpy).toHaveBeenCalledWith('Oops, that was not a valid choice!');
                    return entries.length;
                });

            const testCall = testTextMenu.promptAndExecute();

            const numWidth = Math.ceil(Math.log10(entries.length));
            await expect(testCall).resolves.not.toThrow();
            // title + entries + prompt + callback
            expect(consoleLogSpy).toHaveBeenCalledTimes(1 + entries.length + 1 + 1);
            expect(consoleLogSpy).toHaveBeenNthCalledWith(1, title);
            entries.forEach(({ text }, idx) => {
                const entryNumber = (idx + 1).toString().padStart(numWidth, ' ');
                expect(consoleLogSpy).toHaveBeenNthCalledWith(idx + 2, `(${entryNumber}) ${text}\n`);
            });
            expect(consoleLogSpy).toHaveBeenNthCalledWith(entries.length + 2, prompt);
            expect(consoleLogSpy).toHaveBeenNthCalledWith(
                entries.length + 3,
                `callback ${entries.length - 1} answer is 42`
            );
        });
    });
});

describe(TextMenuBuilder.name, () => {
    let testTextMenuBuilder: TextMenuBuilder;

    beforeEach(() => {
        testTextMenuBuilder = new TextMenuBuilder();
    });

    describe(TextMenuBuilder.prototype.addEntry.name, () => {
        it.each(entries)('should append the given entry for $text callback and return this', (entry) => {
            expect(testTextMenuBuilder.entries).toHaveLength(0);
            const textMenuBuilderWithEntry = testTextMenuBuilder.addEntry(entry);
            expect(textMenuBuilderWithEntry).toBe(testTextMenuBuilder);
            expect(textMenuBuilderWithEntry.entries).toStrictEqual([entry]);
        });
    });

    describe(TextMenuBuilder.prototype.build.name, () => {
        it('should return a new TextMenu with the given title, entries, and prompt', () => {
            const testTextMenu = testTextMenuBuilder
                .withTitle(title)
                .addEntry(entries[0])
                .addEntry(entries[1])
                .withPrompt(prompt)
                .build();
            expect(testTextMenu).toStrictEqual(new TextMenu(title, entries, prompt));
        });
    });

    describe(TextMenuBuilder.prototype.withPrompt.name, () => {
        it('should store the given prompt and return this', () => {
            const prompt = 'test prompt';
            expect(testTextMenuBuilder.prompt).toStrictEqual('');
            const textMenuBuilderWithPrompt = testTextMenuBuilder.withPrompt(prompt);
            expect(textMenuBuilderWithPrompt).toBe(testTextMenuBuilder);
            expect(textMenuBuilderWithPrompt.prompt).toStrictEqual(prompt);
        });
    });

    describe(TextMenuBuilder.prototype.withTitle.name, () => {
        it('should store the given prompt and return this', () => {
            const title = 'test title';
            expect(testTextMenuBuilder.title).toStrictEqual('');
            const textMenuBuilderWithTitle = testTextMenuBuilder.withTitle(title);
            expect(textMenuBuilderWithTitle).toBe(testTextMenuBuilder);
            expect(textMenuBuilderWithTitle.title).toStrictEqual(title);
        });
    });
});
