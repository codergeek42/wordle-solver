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

import { readFile } from 'node:fs/promises';
import { LetterAtPositionInWordRule, letterAtPositionInWordRuleComparator } from './letterAtPosition';
import { cloneDeep, countBy, difference, every, maxBy, times, union, uniq } from 'lodash';
import { WordLength } from '../../__data__/alphabet';
import { LetterAtPositionInWord } from './letterAtPosition';
import { MissingPositionError, NoMoreGuessesError } from './wordleSolverError';

/**
 * Keeps track of a dictionary of possible words to be guessed, as well as exclusion/mandate rules for
 * already-guessed letters.
 */
export default class WordList {
    private myWords: string[];
    private myPossibleLetters: string[][];
    private myLetterRules: LetterAtPositionInWordRule[];
    private myAlphabet: string[];

    /**
     * @param _words - the list of words to use as a dictionary of possible guess candidates
     */
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

    /**
     * List of all possible letters.
     */
    get alphabet(): WordList['myAlphabet'] {
        return this.myAlphabet;
    }

    /**
     * List of possible words.
     */
    get words(): WordList['myWords'] {
        return this.myWords;
    }

    /**
     * List of mandate/exclusion rules.
     */
    get letterRules(): WordList['myLetterRules'] {
        return this.myLetterRules;
    }

    /**
     * List of possible letters at each position.
     */
    get possibleLetters(): WordList['myPossibleLetters'] {
        return this.myPossibleLetters;
    }

    /**
     * Factory method to read the given file and build a `WordList` from it, expecting one word per line.
     *
     * @param fileName - filename (relative)
     *
     * @returns a new WordList object with the words from the given file contents.
     *
     * @example To build a WordList having words ABC and DEF, suppose  'abc-def.txt' contains
     * ```plaintext
     * ABC
     * DEF
     * ```
     * then one can write
     * ```typescript
     * const sampleWordList = await WordList.fromFile('abc-def.txt');
     * expect(sampleWordList.words).toStrictEqual(['ABC', 'DEF']);
     * ```
     */
    static async fromFile(fileName: string, wordLength?: number): Promise<WordList> {
        // TODO: I will probably need to make newline configurable, i.e. for Windows CR/LF, etc.
        // For now, assume Linux/Unix-like \n only, for simplicity.
        const newline = '\n';
        const fileData = await readFile(fileName, { encoding: 'utf8' });
        let words = fileData.split(newline);
        if (wordLength) {
            words = words.filter((word) => word.length === wordLength);
        }
        return new WordList(words);
    }

    /**
     * Factory method to create a `WordList` that is a duplicate of another.
     *
     * @param wordListObj - the `WordList` object to copy
     *
     * @returns a deep copy of the given `WordList` object.
     *
     * @example
     * const originalWordList = new WordList(['ABC', 'DEF']);
     * const copyWordList = WordList.fromCopyOf(originalWordList);
     * expect(copyWordList).toStrictEqual(originalWordList);
     * expect(copyWordList).not.toBe(originalWordList);
     */
    static fromCopyOf(wordListObj: WordList): WordList {
        return cloneDeep(wordListObj);
    }

    /**
     * Create a new `WordList` as a copy of the calling object with the given additional rules, without modifying the
     * caller.
     *
     * @param lettersAtPositionsRules - the additional exclude and/or mandate rules
     * @see {@link LetterAtPositionInWordRule}
     *
     * @returns the created `WordList`
     *
     * @example to exclude DEF from a `WordList` that has words ABC and DEF
     * ```typescript
     * const abcDefWordList = new WordList(['ABC', 'DEF']);
     * const abcOnlyWordList = abcDefWordList.withPositionLetterRules([{
     *      letter: 'D',
     *      required: LetterAtPositionInWordRule.Impossible }
     * ]);
     * ```
     */
    withPositionLetterRules(lettersAtPositionsRules: LetterAtPositionInWordRule[]): WordList {
        const newWordList = WordList.fromCopyOf(this);
        newWordList.processExclusionsFromRules(lettersAtPositionsRules);

        return newWordList;
    }

    /**
     * Checks if the given word matches all of the current mandate and exclusion rules.
     *
     * @param word - the word to validate
     *
     * @returns true if the word matches all of the current rules, false otherwise.
     *
     * @example to check that ABC would be allowed but DEF would not
     * ```typescript
     * const abcDefWordList = new WordList(['ABC', 'DEF']);
     * const abcOnlyWordList = abcDefWordList.withPositionLetterRules([{
     *      letter: 'D',
     *      required: LetterAtPositionInWordRule.Impossible }
     * ]);
     * expect(abcOnlyWordList.doesWordMatchAllRules('ABC')).toBe(true);
     * expect(abcOnlyWordList.doesWordMatchAllRules('DEF')).toBe(false);
     * ```
     */
    doesWordMatchAllRules(word: string): boolean {
        const doesMatch =
            every(this.possibleLetters, (possibleLettersAtPos: string, position: number) => {
                const matchesPossibleLetters = possibleLettersAtPos.includes(word[position]);
                return matchesPossibleLetters;
            }) &&
            every(this.myLetterRules, (rule: LetterAtPositionInWordRule) => {
                const matchesLetterRules =
                    rule.required !== LetterAtPositionInWord.Misplaced || word.includes(rule.letter);
                return matchesLetterRules;
            });
        return doesMatch;
    }

    /**
     * Adds the given rule to the caller's ongoing list, and processes them by removing any entries from the
     * possible words list that do not match them as well as any letters in its alphabet that are no longer possible.
     *
     * @param lettersAtPositionsRules - the additional exclude and/or mandate rules
     * @see {@link LetterAtPositionInWordRule}
     */
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

    countLetters(): Record<string, number>[] {
        if (this.words.length <= 0) {
            throw new NoMoreGuessesError('Empty word list.');
        }
        const { length } = maxBy(this.words, 'length')!;

        const totalCount = Array.from({ length }).map((_val, position) =>
            // If the words are not of equal length, then the counts will tally the "missing" letters as 'undefined'.
            countBy(this.words.flatMap((word) => (position < word.length ? [word[position]] : [])))
        );
        return totalCount;
        // .map((letterCountAtPosition) =>
        //
        //     pickBy(letterCountAtPosition, (_value, key) => key !== 'undefined')
        // );
    }
}
