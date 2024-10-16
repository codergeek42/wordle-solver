/*****
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
 *****/

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
        newWordList.processExclusionsFromRules(lettersAtPositionsRules);

        return newWordList;
    }

    doesWordMatchAllRules(word: string): boolean {
        const doesMatch =
            every(this.possibleLetters, (possibleLettersAtPos, position) => {
                const matchesPossibleLetters = possibleLettersAtPos.includes(word[position]);
                return matchesPossibleLetters;
            }) &&
            every(this.myLetterRules, (rule) => {
                const matchesLetterRules =
                    rule.required !== LetterAtPositionInWord.Misplaced || word.includes(rule.letter);
                return matchesLetterRules;
            });
        return doesMatch;
    }

    processExclusionsFromRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): void {
        // NB: Ensure that the Mandatory letters are processed after the others,
        // to handle the case when a Misplaced letter is found at another position,
        // and so marked as Mandatory in the correct position and Impossible in the
        // position where it was previously Misplaced.
        lettersAtPositionsRules.sort(letterAtPositionInWordRuleComparator).forEach((rule) => {
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
    }
}
