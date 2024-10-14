import { readFile } from 'node:fs/promises';
import { LetterAtPositionInWordRule, letterAtPositionInWordRuleComparator } from './letterAtPosition';
import { cloneDeep, difference, every, times, union, uniq } from 'lodash';
import { WordLength } from '../../__data__/alphabet';
import { LetterAtPositionInWord } from './letterAtPosition';
import { MissingPositionError } from './wordleSolverError';

export default class WordList {
    private myWords: string[];
    private myPossibleLetters: string[][];
    private myLetterRules: LetterAtPositionInWordRule[];
    private myAlphabet: string[];

    constructor(private readonly _words: string[] = []) {
        this.myWords = _words.map((word) => word.trim().toUpperCase()).sort();
        this.myLetterRules = [];
        this.myAlphabet = uniq(this.myWords.join('')).sort();

        this.myPossibleLetters = times(WordLength, () => []);
        this.myWords.forEach((word) => {
            Array.from(word).forEach((letter, position) => {
                this.myPossibleLetters[position] = union([letter], this.myPossibleLetters[position]);
            });
        });
        this.myPossibleLetters.forEach((possibleLetters) => possibleLetters.sort());
    }

    get alphabet(): WordList['myAlphabet'] {
        return this.myAlphabet;
    }

    get words(): WordList['myWords'] {
        return this.myWords;
    }

    get letterRules(): WordList['myLetterRules'] {
        return this.myLetterRules;
    }

    get possibleLetters(): WordList['myPossibleLetters'] {
        return this.myPossibleLetters;
    }

    static async fromFile(fileName: string): Promise<WordList> {
        // TODO: I will probably need to make newline configurable, i.e. for Windows CR/LF, etc.
        // For now, assume Linux/Unix-like \n only, for simplicity.
        const newline = '\n';
        const fileData = await readFile(fileName, { encoding: 'utf8' });
        return new WordList(fileData.split(newline));
    }

    static fromCopyOf(wordListObj: WordList): WordList {
        return cloneDeep(wordListObj);
    }

    withPositionLetterRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): WordList {
        const newWordList = WordList.fromCopyOf(this);
        // console.log('@@ withPositionLetterRules: '.concat(JSON.stringify({ newWordList }, null, 2)));
        newWordList.processExclusionsFromRules(lettersAtPositionsRules);

        return newWordList;
    }

    doesWordMatchAllRules(word: string): boolean {
        const doesMatch =
            every(this.possibleLetters, (possibleLettersAtPos, position) => {
                const matchesPossibleLetters = possibleLettersAtPos.includes(word[position]);
                // console.log(
                //     '@@ doesMatch possibleLetters callback: '.concat(
                //         JSON.stringify({ word, possibleLettersAtPos, position, matchesPossibleLetters }, null, 2)
                //     )
                // );
                return matchesPossibleLetters;
            }) &&
            every(this.myLetterRules, (rule) => {
                const matchesLetterRules =
                    rule.required !== LetterAtPositionInWord.Misplaced || word.includes(rule.letter);
                // console.log(
                //     '@@ doesMatch letterRules callback: '.concat(
                //         JSON.stringify({ rule, word, matchesLetterRules }, null, 2)
                //     )
                // );
                return matchesLetterRules;
            });
        // console.log(
        //     '@@ doesWordMatchAllRules: '.concat(
        //         JSON.stringify(
        //             { doesMatch, word, rules: this.letterRules, possibleLetters: this.myPossibleLetters },
        //             null,
        //             2
        //         )
        //     )
        // );
        return doesMatch;
    }

    processExclusionsFromRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): void {
        // NB: Ensure that the Mandatory letters are processed after the others,
        // to handle the case when a Misplaced letter is found at another position,
        // and so marked as Mandatory in the correct position and Impossible in the
        // position where it was previously Misplaced.
        lettersAtPositionsRules.sort(letterAtPositionInWordRuleComparator).forEach((rule) => {
            // console.log(
            //     '@@ processExclusionsFromRules forEach callback ',
            //     JSON.stringify({ words: this.words, rule }, null, 2)
            // );

            if (rule.required === LetterAtPositionInWord.Impossible) {
                this.myPossibleLetters = this.myPossibleLetters.map((letters) => difference(letters, [rule.letter]));
            } else if (typeof rule.position === 'undefined') {
                throw new MissingPositionError(JSON.stringify(rule));
            } else
                switch (rule.required) {
                    case LetterAtPositionInWord.Mandatory:
                        this.myPossibleLetters[rule.position] = [rule.letter];
                        break;
                    case LetterAtPositionInWord.Misplaced:
                        this.myPossibleLetters[rule.position] = difference(this.myPossibleLetters[rule.position], [
                            rule.letter
                        ]);
                        break;
                }
        }, this);

        this.myLetterRules.push(...lettersAtPositionsRules);
        this.myWords = this.words.filter((word) => this.doesWordMatchAllRules(word), this);
        this.myAlphabet = uniq(this.words.join(''));
        // console.log(
        //     '@@ processExclusionsFromRules after callback: ',
        //     JSON.stringify(
        //         { words: this.words, possibleLetters: this.possibleLetters, alphabet: this.alphabet },
        //         null,
        //         2
        //     )
        // );
    }
}
