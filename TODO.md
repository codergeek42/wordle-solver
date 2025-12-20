# C#/.Net Port
- [x] Finish Solver library
- [ ] Use reflection to loop through all guesser strategies instead of having to explicitly populate them?
- [x] Use PLINQ to calculate next highest-scoring word

# CICD Pipeline
- [x] CodeQL
- [ ] Continuous Integration (build/test/coverage report/etc.)
- [ ] Daily solver (after implemented)

# Usage
Currently, only the basic solving algorithm is implemented alongside some guessing strategies (scoring metrics); but
future work planned includes the following:

- [ ] create a simple guess-and-check text interface for testing and whatnot (i.e, print optimal guess and prompt for
      result);
- [ ] implement an automated solver for the current daily puzzle using Playwright;
- [ ] wrap the solver backend as a GraphQL microservice; and
- [ ] publish this resulting GraphQL microservice within a Docker image; then
- [ ] make the execute command for the Docker image be to solve the current puzzle in headless mode, with a sub-image
      for the TUI.

NB: Sure, it would certainly be a lot less complex to write this as just the solver library and some Playwright to
automate the puzzle -- just the second item on this list alone -- but a huge rationale for this project, like all of my
hobby projects, is to learn by doing. 🙃


# Tests
- [ ] Refactor test data and case naming for nicer coverage reporting.
- [ ] Make nicer reasons for Theory tests for clearer output visibility.
