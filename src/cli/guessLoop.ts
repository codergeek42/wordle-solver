import { mapValues } from 'lodash';
import EventEmitter from 'node:events';
import DistinctLettersStrategy from '../lib/guesserStrategies/distinctLettersStrategy';
import LetterFrequencyStrategy from '../lib/guesserStrategies/letterFrequencyStrategy';
import PerLetterEliminationStrategy from '../lib/guesserStrategies/perLetterEliminationStrategy';
import RetryMisplacedLettersStrategy from '../lib/guesserStrategies/retryMisplacedLettersStrategy';
import NextWordGuesserStrategyBase, {
    GuesserStrategyEvent,
    GuesserStrategyEventPayload
} from '../lib/nextWordGuesserStrategy';
import WordList from '../lib/wordList';

export async function runGuessLoop(wordList: WordList): Promise<void> {
    const progressEvents = new EventEmitter();
    progressEvents.on(
        GuesserStrategyEvent.CalculateScores,
        ({ percent }: GuesserStrategyEventPayload[GuesserStrategyEvent.CalculateScores]) => {
            console.log(`${percent}...`);
        }
    );

    const guesserStrategies: Record<string, NextWordGuesserStrategyBase> = Object.fromEntries(
        [
            DistinctLettersStrategy,
            LetterFrequencyStrategy,
            PerLetterEliminationStrategy,
            RetryMisplacedLettersStrategy
        ].map((guesserStrategyType) => [guesserStrategyType.name, new guesserStrategyType(wordList, progressEvents)])
    );
    console.log(mapValues(guesserStrategies, (guesserStrategy) => guesserStrategy.guessNextWordAndScore()));
}
