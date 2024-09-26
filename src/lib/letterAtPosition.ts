export enum LetterAtPositionInWord {
    // Right letter, right position.
    Mandatory = 'Mandatory',
    // Right letter, wrong position.
    Misplaced = 'Misplaced',
    // Wrong letter entirely (not in word).
    Impossible = 'Impossible'
}

export function letterAtPositionInWordRuleComparator(
    posRuleA: LetterAtPositionInWordRule,
    posRuleB: LetterAtPositionInWordRule
): number {
    if (posRuleA.required === LetterAtPositionInWord.Mandatory) {
        return posRuleB.required === LetterAtPositionInWord.Mandatory ? 0 : +1;
    }
    return posRuleB.required == LetterAtPositionInWord.Mandatory ? -1 : 0;
}

export type LetterRule = {
    letter: string;
    required: LetterAtPositionInWord;
};

export type LetterAtPositionInWordRule = LetterRule & {
    position?: number;
};

export type LetterWithPosition = Required<Omit<LetterAtPositionInWordRule, 'required'>>;
