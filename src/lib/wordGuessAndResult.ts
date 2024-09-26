import { LetterAtPositionInWordRule } from './letterAtPosition';

export type WordGuessAndScore = {
    word: string;
    score: number;
};

export type WordGuessAndResult = {
    word: string;
    result: LetterAtPositionInWordRule[];
};
