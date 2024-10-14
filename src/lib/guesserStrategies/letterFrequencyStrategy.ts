import { countBy, sum } from 'lodash';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';

export default class LetterFrequencyStrategy extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    scoreForGuess(guess: string): number {
        const countLettersAtPosition = (position: number): Record<string, number> =>
            countBy(this.wordList.words.flatMap((word) => [word[position]]));

        return sum(
            Array.from(guess).map((letter, position) => {
                const lettersAtPosition = countLettersAtPosition(position);
                return lettersAtPosition[letter] / this.wordList.possibleLetters[position].length;
            })
        );
    }
}
