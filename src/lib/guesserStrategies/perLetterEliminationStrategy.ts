import { sumBy } from 'lodash';
import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';
import WordList from '../wordList';
import { LetterAtPositionInWord, LetterAtPositionInWordRule } from '../letterAtPosition';

export default class PerLetterEliminationStrategy extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    constructor(...params: ConstructorParameters<typeof NextWordGuesserStrategyBase>) {
        super(...params);
    }

    static totalPossibleLettersInWordList(wordList: WordList): number {
        return sumBy(wordList.possibleLetters, 'length');
    }

    static generateGuessPositionLetterRules(guess: string): LetterAtPositionInWordRule[] {
        return Array.from(guess).map((letter, position) => ({
            position,
            letter,
            required: LetterAtPositionInWord.Misplaced
        }));
    }

    scoreForGuess(guess: string): number {
        const previousScore = PerLetterEliminationStrategy.totalPossibleLettersInWordList(this.wordList);
        const guessWordList = this.wordList.withPositionLetterRules(
            PerLetterEliminationStrategy.generateGuessPositionLetterRules(guess)
        );
        const guessScore = PerLetterEliminationStrategy.totalPossibleLettersInWordList(guessWordList);
        return previousScore - guessScore;
    }
}
