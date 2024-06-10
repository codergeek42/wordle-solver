import { readFile } from 'node:fs/promises';
import {
    LetterAtPositionRule,
    doesWordMatchLetterAtPosition
} from './letterAtPosition';
import { every } from 'lodash';

export class WordList {
    private myWords: string[];

    constructor(private readonly _words: string[] = []) {
        this.myWords = _words.map((word) => word.trim().toUpperCase()).sort();
    }

    public get words() {
        return this.myWords;
    }

    public static async fromFile(fileName: string): Promise<WordList> {
        // TODO: I will probably need to make newline configurable, i.e. for Windows CR/LF, etc.
        // For now, assume Linux/Unix-like \n only, for simplicity.
        const newline = '\n';
        const fileData = await readFile(fileName, { encoding: 'utf8' });
        return new WordList(fileData.split(newline));
    }

    public withPositionLetterRules(
        lettersAtPositionsRules: LetterAtPositionRule[]
    ) {
        return new WordList(
            this.myWords.filter((candidateWord) =>
                every(lettersAtPositionsRules, (letterAtPositionRule) =>
                    doesWordMatchLetterAtPosition(
                        candidateWord,
                        letterAtPositionRule
                    )
                )
            )
        );
    }
}
