import path from 'path';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';

export type WordleSolverCommandLineArguments = {
    dictionary: string;
    previousWords: string;
};

export function parseCommandLineArgs(): WordleSolverCommandLineArguments {
    // TODO: Support for Windows words file?
    return yargs(hideBin(process.argv))
        .default('dictionary', path.join(path.sep, 'usr', 'share', 'dict', 'words'))
        .default('previousWords', path.join(process.env.HOME ?? '~', '.wordle-solver-previous-words.txt'))
        .parseSync();
}
