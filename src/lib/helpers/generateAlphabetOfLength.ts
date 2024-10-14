import { range } from 'lodash';

export default function generateAlphabetOfLength(numLetters: number): string {
    return String.fromCharCode(...range(0, numLetters).map((ord) => 'A'.charCodeAt(0) + ord));
}
