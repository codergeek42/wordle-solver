import { difference, uniq } from 'lodash';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';

export default class DistinctLettersStrategy extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    scoreForGuess(guess: string): number {
        const prev = this.getAlreadyGuessedLetters();
        const score = uniq(difference(Array.from(guess), prev)).length;
        return score;
    }
}
