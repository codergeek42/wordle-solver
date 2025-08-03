import { open } from 'fs/promises';
import TextMenuBuilder from '../lib/cli/textMenu';
import WordList from '../lib/wordList';
import { parseCommandLineArgs } from './arguments';
import { runGuessLoop } from './guessLoop';
import { displayCopyleftInformation, displayNoWarrantyInformation, displayWelcomeBanner } from './legal';

async function initializeWordList(dictionaryFile: string, previousWordsFile: string): Promise<WordList> {
    // Create the file if it does not yet exist...
    const previousWordsFileHandle = await open(previousWordsFile, 'a+', 0o664);
    try {
        const previousWords = (await previousWordsFileHandle.readFile({ encoding: 'utf8' })).split('\n');
        return (await WordList.fromFile(dictionaryFile)).withExcludedWords(previousWords);
    } finally {
        previousWordsFileHandle.close();
    }
}

async function main(): Promise<void> {
    const cliArgs = parseCommandLineArgs();
    const wordList = await initializeWordList(cliArgs.dictionary, cliArgs.previousWords);

    const MainMenu = new TextMenuBuilder()
        .withTitle('===== Main Menu =====')
        .addEntry('Start guessing!', () => runGuessLoop(wordList))
        .addEntry('(No) Warranty Information', displayNoWarrantyInformation)
        .addEntry('Copyleft Information', displayCopyleftInformation)
        .addEntry('Exit', () => process.exit(0))
        .withPrompt('? ')
        .build();

    displayWelcomeBanner();
    while (true) {
        await MainMenu.promptAndExecute();
    }
}

if (require.main === module) {
    (async () => await main())();
}
