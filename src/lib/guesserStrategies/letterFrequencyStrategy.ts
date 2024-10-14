import { countBy, sum } from 'lodash';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';

export default class LetterFrequencyStrategy extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    scoreForGuess(guess: string): number {
        // const previousLetterFrequencies = countBy(this.wordList.words.join(''));
        // const totalLettersCount = sum(values(previousLetterFrequencies));
        // // console.log('@@ '.concat(JSON.stringify({ totalLettersCount, previousLetterFrequencies }, null, 1)));
        // const guessScore = sum(
        //     values(mapValues(countBy(guess), (count, letter) => count * previousLetterFrequencies[letter]))
        // );
        // return guessScore / totalLettersCount;

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
