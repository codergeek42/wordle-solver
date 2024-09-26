import { readFile } from 'node:fs/promises';
import { LetterAtPositionInWordRule, letterAtPositionInWordRuleComparator } from './letterAtPosition';
import { every, times, uniq } from 'lodash';
import { WordLength } from '../../__data__/alphabet';
import { LetterAtPositionInWord } from './letterAtPosition';
import { MissingPositionError } from './wordleSolverError';

export default class WordList {
    private myWords: string[];
    private myPossibleLetters: Set<string>[];
    private myLetterRules: LetterAtPositionInWordRule[];
    private myAlphabet: Set<string>;

    constructor(private readonly _words: string[] = []) {
        this.myWords = _words.map((word) => word.trim().toUpperCase()).sort();
        this.myLetterRules = [];
        this.myAlphabet = new Set(this.myWords.flatMap((word) => uniq(word)));
        this.myPossibleLetters = times(WordLength, () => new Set(this.myAlphabet));
    }

    get alphabet(): Set<string> {
        return this.myAlphabet;
    }

    get words(): string[] {
        return this.myWords;
    }

    get letterRules(): LetterAtPositionInWordRule[] {
        return this.myLetterRules;
    }

    get possibleLetters(): Set<string>[] {
        return this.myPossibleLetters;
    }

    static async fromFile(fileName: string): Promise<WordList> {
        // TODO: I will probably need to make newline configurable, i.e. for Windows CR/LF, etc.
        // For now, assume Linux/Unix-like \n only, for simplicity.
        const newline = '\n';
        const fileData = await readFile(fileName, { encoding: 'utf8' });
        return new WordList(fileData.split(newline));
    }

    withPositionLetterRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): WordList {
        const newWordList = new WordList(this.myWords);
        newWordList.processExclusionsFromRules(lettersAtPositionsRules);
        newWordList.myWords = this.myWords.filter((word) => newWordList.doesWordMatchAllRules(word), newWordList);
        return newWordList;
    }

    doesWordMatchAllRules(word: string): boolean {
        return (
            every(this.myPossibleLetters, (possibleLetters, position) => possibleLetters.has(word[position])) &&
            every(
                this.myLetterRules,
                (rule) => rule.required !== LetterAtPositionInWord.Misplaced || word.includes(rule.letter)
            )
        );
    }

    processExclusionsFromRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): void {
        // NB: Ensure that the Mandatory letters are processed after the others,
        // to handle the case when a Misplaced letter is found at another position,
        // and so marked as Mandatory in the correct position and Impossible in the
        // position where it was previously Misplaced.
        lettersAtPositionsRules.sort(letterAtPositionInWordRuleComparator).forEach((rule) => {
            if (rule.required === LetterAtPositionInWord.Impossible) {
                this.myPossibleLetters.forEach((letters) => letters.delete(rule.letter));
            } else if (typeof rule.position === 'undefined') {
                throw new MissingPositionError(JSON.stringify(rule));
            } else
                switch (rule.required) {
                    case LetterAtPositionInWord.Mandatory:
                        this.myPossibleLetters[rule.position] = new Set([rule.letter]);
                        break;
                    case LetterAtPositionInWord.Misplaced:
                        this.myPossibleLetters[rule.position].delete(rule.letter);
                        break;
                }
        }, this);
        this.myLetterRules.push(...lettersAtPositionsRules);
    }
}
