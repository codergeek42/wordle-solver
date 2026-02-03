# C#/.Net Port
- [x] Finish Solver library.
- [ ] Use Unity for dependency injection for filesystem and guesser strategies instead of having to explicitly populate
    them.

# Automated GUI Solver
- [x] Create GUI project. 
- [x] Add Playwright & browser packages.
- [x] Setup initial app to open Wordle homepage.
- [x] Click through Overview and "How to play" screens to get to puzzle entry.
- [ ] Implement the full guessers looping.
- [ ] Enable hard mode. (#151)

# CI/CD Pipeline
- [x] Add CI pipeline as Github Action.
- [x] Add CodeQL check.
- [ ] Add Daily solver to pipeline (after GUI portion implemented).

# Performance
- [x] Use PLINQ to calculate next highest-scoring word.
- [x] Mark a "should run" per strategy:
    * initial strategy should be LetterFrequency or PerLetterElimination; DistinctLetters is always highest at 5
    at start for words that have no repeated letters, but is not useful if no previous guesses have been made yet; and
    * no need to (re)calculate RetryMisplacedLetters if no guesses have been made that include one or more Misplaced
    letters.

# Aesthetic 
- [-] Make Main be invoked from command-line root command handler to only run when not given --version/--help options.
- [ ] If excluded words file provided, add words to it after marked as not a valid guess.
- [x] Make the CLI app give a nice message when it's solved.
- [ ] Add 6-guess limit.
- [ ] Add repeatability: When multiple scores identical for various words/strategies, use first in alphabetic order,
  or by strategy based on guess count/ShouldRun/etc.


# Usage
Currently, only the basic solving algorithm is implemented alongside some guessing strategies (scoring metrics); but
future work planned includes the following:
- [x] create a simple guess-and-check text interface for testing and whatnot (i.e, print optimal guess and prompt for
      result);
- [ ] implement a generator for the dictionary file to download & merge with existing word list(s);
- [ ] implement an automated solver for the current daily puzzle using Playwright;
- [ ] wrap the solver backend as a GraphQL microservice; and
- [ ] publish this resulting GraphQL microservice within a Docker image; then
- [ ] make the execute command for the Docker image be to solve the current puzzle in headless mode, with a sub-image
      for the TUI.

NB: Sure, it would certainly be a lot less complex to write this as just the solver library and some Playwright to
automate the puzzle -- just the second item on this list alone -- but a huge rationale for this project, like all of my
hobby projects, is to learn by doing. 🙃

# Tests
- [-] Add unit tests for most of the CLI utility methods.
- [-] Refactor test data and case naming for nicer coverage reporting.
- [ ] Make nicer reasons for Theory tests for clearer output visibility.
