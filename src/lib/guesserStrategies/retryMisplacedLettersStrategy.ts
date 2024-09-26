import { IStrategyScoreMethod, NextWordGuesserStrategyBase } from '../nextWordGuesserStrategy';
import { LetterAtPositionInWord, LetterWithPosition } from '../letterAtPosition';

export default class RetryMisplacedLettersStrategy extends NextWordGuesserStrategyBase implements IStrategyScoreMethod {
    scoreForGuess(guess: string): number {
        const previouslyMisplacedLetters = this.wordList.letterRules
            .filter((rule) => rule.required === LetterAtPositionInWord.Misplaced)
            .map(({ letter, position }) => ({ letter, position }) as LetterWithPosition);
        return Array.from(guess)
            .map((letter, position) => ({ letter, position }))
            .filter(({ letter, position }) =>
                previouslyMisplacedLetters.some(
                    (previouslyMisplaced) =>
                        previouslyMisplaced.letter === letter && previouslyMisplaced.position !== position
                )
            ).length;
    }
}
