# wordle-solver

A clever algorithm and automated tool to solve the NYTimes daily Wordle puzzle game.

# What is Wordle?

Wordle is a word-based logic and puzzle game created by Josh Wardle and (since January 2022) published by the New York
Times. The system chooses a random 5-letter word, and it is the player's goal to guess that word within 6 attempts.
After each attempt, the system will color-code their guess at each position to reveal clues about the random word:

- Green: if the letter guessed at this position is correct;
- Yellow: if the letter guessed at this position is in the word, but not at this position; or
- Black: if the letter guessed at this position does not appear anywhere in the word.

(Note that this concept is similar to the classic "Mastermind" game originally made by Mordecai Meirowitz, only with
letters at each position and corresponding colors to demonstrate correctness instead of adjacent black and white key
pegs.)

If they player is able to successfully guess the word within these 6 attempts, then they win.

# Sample Gameplay

Suppose the randomly generated word is "SONIC". A potential first guess might be "STONE" because it includes the 1st
(E), 2nd (T), 4th (O), 6th (N), and 7th (E) most common letters in English texts (according to the letter frequency
chart on Wikipedia), making it a suitable guess to help eliminate or narrow down a lot of potential letters in one fell
swoop. This would result in the following output (where G, Y, and B below each of the guessed letters denote Green,
Yellow, and Black color-coded positions as described above):

```
S T O N E
G Y Y B B
```

This tells us that

1. Since S at the first position is Green, then it is in the word at the correct position;
2. Since O and N in the third and fourth positions are Yellow, then they are are in the word but not at their correct
   positions; and
3. Since T and E in the second and fifth positions are Black, then neither of them are in the word at any position.

So from this we know now that the word

- must begin with S;
- must contain both O and N but not as the third and fourth positions, respectively; and
- must not contain either T or E.

A candidate for the second guess could be "SONAR", as it satisfies the required conditions and allows us to not only
attempt O and N at other positions, but also to try additional letters, namely A and R. This would result in the
following output:

```
S O N A R
G G G B B
```

From these results we know that

4. since O and N at the second and third positions, respectively, are green, then those two are now in their correct
   positions of the word;
5. since A and R at the fourth and fifth positions, respectively, are Black, then they do not appear anywhere in the
   word.

The combination of these with the previous three criteria gives us the result that the word must start with "SON" and
must not contain any of A, E, R, or T. Using the built-in dictionary of a typical Linux or Unix system
(`/usr/share/dict/words`), we can see that this greatly reduces the potential words to guess, to only a proverbial
handful:

```
SONCY
SONDS
SONGO
SONGS
SONGY
SONIC
SONLY
SONNI
SONNY
SONSY
```

The player might then realize that many of these potentials include another S, N, or O in the last two letters. An
astute player might at this step decide against guessing any of these, in order to guess another word instead which
includes more yet-unguessed letters. This would give the player information about a broader selection of letters, more
than just S, O, and N. In this case, it would be advisable to try to first guess all of the vowels, so that those can be
ascertained. In this example, that means that SONGO, SONIC, SONNI would be reasonable next guesses -- except that SONGO
and SONNI both contain repeated letters (O and N, respectively), so SONIC would be a more informative guess because it
would reveal information about two unguesssed letters instead of one. So, the player guesses SONIC here, and gets the
successful result:

```
S O N I C
G G G G G
```

Congratulations, the player has won this game after a total of 3 guesses!

# Solver Algorithm

The basis of this solver algorithm is a single guess-and-check loop: Given a starting dictionary of all possible words,
and keeping track of which letters have already been guessed at each position in the word, and at each guess iteration,
which letter & position combinations are eliminated (i.e., cannot be in the word at that position or perhaps at all) or
mandated (i.e., must be in the word, perhaps at a specific position), follow the rules of play as optimally as possible:

1. For every guess opportunity, calculate a score for every word remaining in the dictionary, and make a guess based on
   whichever word has the highest score among them.
2. Process the result by adding new constraint rules to the ongoing list of letter & position combinations.
3. Remove from the dictionary any remaining words that do not match these new rules.
4. If there are no words left, then the correct word is not in the starting list.
5. If there is exactly one word left, then that is the correct word and the player has won.
6. If there is more than one word, then there is still more to be guessed.
    - If 6 guesses have already been made, the player has reached the guess limit.
7. Repeat until either the player has won, or the 6-guess limit has been reached.

## Guessing Strategies & Scoring

The core of the algorithm is straightforward: At each round, calculate a score for every possible remaining word, and
then use the highest-scoring word as the guess. But each word can be scored in various different ways, based on what is
considered optimal for the particular guessing strategy being used. There are four guessing strategies used to determine
the next optimal word choice:

### DistinctLettersStrategy

This metric is the simplest of the four. Given the current list of all guessed letters, ignoring their positions, the
score for a particular guess candidate is the number of unguessed unique letters in the candidate.

For example, if only "STONE" has already been guessed, then a candidate of "TONES" would score 0 (because each of E, N,
O, S, and T have already been guessed), whereas "STOPS" would score 1, and "STOMP" would score 2, because M and P have
not yet been guessed. This strategy prioritizes only gaining information about as many letters as possible without
regard for their position in the word, if any.

### LetterFrequencyStrategy

This metric scores a candidate guess based on the frequencies of each letter at their corresponding position,
calculating letter frequencies based on the starting dictionary of potential words: a guess candidate with letters that
are used more frequently will be scored higher than a candidate with infrequently-used letters.

For example, if the remaining dictionary of candidates is AUDIT, STONE, TONES, and KNOTS, then STONE and TONES will have
an equivalent score, and KNOTS will have a slightly lower score, because even though N, O, S, and T each occur in all
three words, E occurs more frequently than K; and AUDIT will have a much lower score, because it has only 1
frequently-used letter (T) rather than the 4 of KNOTS. This strategy prioritizes guessing letters that occur the most
frequently, and calculates the score as a sum of the ratios of, at each position, the frequency of the corresponding
candidate letter to the number of possible letters at that position.

### PerLetterEliminationStrategy

This metric scores a candidate guess based on how likely that guess is to eliminate the most number of letter & position
combinations. For a given guess candidate, this metric scores that guess based on how many additional letter & position
combinations this guess could potentially eliminate, effectively simulating that every letter in the guess is included
in the word at an incorrect position, then calculating the resulting score by determining how many additional letter &
position exclusions are created from that guess.

For example, if the remaining dictionary of candidates is AUDIT, STONE, TONES, and KNOTS, and NOTES has already been
guessed, then STONES will have a higher score than TONES, because STONES tries all 5 of the letters at different
positions, whereas NOTES only swaps N and T (2). If no other guesses had yet been made, AUDIT would score equivalently
to STONES because they both would try 5 new letter & position combinations: for AUDIT, this is each of the yet-unguessed
A, D, I, and U, alongside T at position 5 which had only been guessed at position 3 so far; and for STONE, this would be
all 5 of the previously-guessed E, N, O, S, and T but at their yet-unguessed positions 5, 4, 3, 1, and 2, respectively.

### RetryMisplacedLettersStrategy

This metric scores a candidate guess based on how letters that have already been guessed are being re-used at different
positions where they could potentially be correct. For a given guess candidate, this metric scores that guess based on
how many letters are currently known to be misplaced in the word -- that is, marked with Yellow result -- are in the
guess candidate at yet-unguessed locations.

For example, if the remaining dictionary of candidates is AUDIT, STONE, TONES, and KNOTS, and NOTES has already been
guessed with a result of all Yellows, then STONE will score the highest because it attempts to guess all 5 of these
letters in different positions, whereas AUDIT would score the lowest because it only attempts to re-guess T in a
different position, alongside the yet-unguessed A, D, I, and U. KNOTS would score between these, because it does attempt
to re-guess the N, O, and T, but keeps the already-guessed S at its current position and includes the yet-unguessed K.
This strategy prioritizes re-guessing existing Yellow-marked letters as much as possible.

### Remarks

Notice that in each strategy, the resulting score is a measure of roughly how many new letters & positions worth of
information that candidate would give about the word within the context of the guessing strategy being used.

Also of note is that, as more information about the word is obtained at each guess iteration, the comparative benefit of
each guessing strategy necessarily changes, based on how much information about the word is still required to obtain the
correct guess. In the [sample gameplay](#sample-gameplay) above, for instance, the first guess is "STONE" because the
most optimal guess choice to start with is the one that gives the most information about the word and eliminates as much
as possible from the (presumably very large) dictionary of candidates, and guessing a word with the most frequently-used
letters will be the most likely to reveal information about them as well as to create exclusion and/or mandate rules
that create the most constraint. After this, when trying to find the optimal second guess, it is best to build upon the
existing rules from the first guess result and try words that include the Yellow-marked letters at different positions,
because re-using those letters in different positions is likely more constraining to the guesses than simply guessing
entirely new letters at those positions. And lastly, at the third guess, when some of the known letters have been placed
in their correct positions and no other letters are known, it is likely best to try to get as many unique letters as
possible to determine the most additional constraining rules for the word. These roughly (but not exactly) correspond to
the `LetterFrequencyStrategy`, `RetryMisplacedLettersStrategy`, and `DistinctLetterStrategy` scoring metrics.

# Bibliography

1. [Wikipedia: Letter Frequency](https://en.wikipedia.org/wiki/Letter_frequency)
