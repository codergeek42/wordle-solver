import { WordLength } from '../../../__data__/alphabet';
import { WordleSolverTestError } from '../wordleSolverError';

export default function generateAlphabetWords(alphabet: string, wordLength: number = WordLength): string[] {
    const prefixLetters = Array.from(alphabet);
    if (wordLength <= 0) {
        throw new WordleSolverTestError('generateAlphabetWords: invalid size argument');
    }
    return wordLength == 1
        ? prefixLetters
        : generateAlphabetWords(alphabet, wordLength - 1).flatMap((word) =>
              prefixLetters.map((letter) => letter.concat(word))
          );
}
