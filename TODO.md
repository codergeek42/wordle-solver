Currently, only the basic solving algorithm is implemented alongside some guessing strategies (scoring metrics); but
future work planned is as follows:

-   [ ] create a simple guess-and-check text interface for testing and whatnot (i.e, print optimal guess and prompt for
        result);
-   [ ] implement an automated solver for the current daily puzzle using Playwright;
-   [ ] wrap the solver backend as a GraphQL microservice (e.g., ); and
-   [ ] publish this resulting GraphQL microservice within a Docker image; then
-   [ ] make the execute command for the Docker image be to solve the current puzzle in headless mode, with a sub-image
        for the TUI.

NB: Sure, it would certainly be a lot less complex to write this as just the solver library and some Jest/Playwright to
automate the puzzle -- just the second item on this list alone -- but a huge rationale for this project, like all of my
hobby projects, is to learn by doing. 🙃
