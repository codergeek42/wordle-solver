import { range } from 'lodash';
import { WordLength } from '../../../__data__/alphabet';
import generateAlphabetOfLength from '../../../src/lib/helpers/generateAlphabetOfLength';

describe(generateAlphabetOfLength, () => {
    it.each(range(0, WordLength + 1))('generates alphabet of length %i', (alphabetLength) => {
        const expected = String.fromCharCode(...range(0, alphabetLength).map((ord) => 'A'.charCodeAt(0) + ord));
        const result = generateAlphabetOfLength(alphabetLength);
        expect(result).toStrictEqual(expected);
    });
});
