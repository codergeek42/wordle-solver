import { omit } from 'lodash';

export enum LetterAtPosition {
    Impossible,
    Possible,
    Mandatory
}
export type LetterRule = {
    letter: string;
    required: LetterAtPosition;
};

export type LetterAtPositionRule = LetterRule & {
    position: number;
};

export function doesLetterMatchRule(letter: string, rule: LetterRule): boolean {
    switch (rule.required) {
        case LetterAtPosition.Impossible:
            return letter != rule.letter;
        case LetterAtPosition.Possible:
            return true;
        case LetterAtPosition.Mandatory:
            return letter == rule.letter;
    }
}

export function doesWordMatchLetterAtPosition(word: string, rule: LetterAtPositionRule) {
    return doesLetterMatchRule(word[rule.position], omit(rule, 'position'));
}
