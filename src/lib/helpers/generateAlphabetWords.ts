import { chunk, join, padEnd } from 'lodash';
import { WordLength } from '../../../__data__/alphabet';

export default function generateAlphabetWords(alphabet: string): string[] {
    // Pad alphabet to a multiple of `WordLength` characters so it can be
    // evenly chunked; use itself as the pad to ensure the resulting
    // alphabet remains unchanged.
    const padLength = alphabet.length <= WordLength ? WordLength : alphabet.length % WordLength;
    const alphabetPadded = padEnd(alphabet, padLength, alphabet);
    return chunk(Array.from(alphabetPadded), WordLength).map((wordChars) => join(wordChars, ''));
}
