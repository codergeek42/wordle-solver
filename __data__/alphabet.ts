// export const Alphabet = [...'ABCDEFGHIJKLMNOPQRSTUVWXYZ'];
/**
 * TODO: Solve styles:
 * 1. guess word by eliminating highest count of distinct letters (e.g., BREAD (5) > BREED (4) = BLEED (4) > PAPAS (3))
 * 2. guess word by highest count of additional letter+position exclusions (total # eliminations at each position maximized);
 * 3. guess word by highest count of re-used misplaced letters; (if known TSE__ misplaced, try ST__E, etc.)
 * 4. guess word by highest count by most frequent letters (calculate frequency list  of each position, eliminate letters highest on that, total # = metric)
 * then pick from among these which removes the most amount of words total from the candidate list.
 */
