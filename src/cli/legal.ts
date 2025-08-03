import { CopyleftInformation, DisclaimerOfWarranty, WelcomeBanner } from '../../__data__/gpl-sections.json';

export function displayWelcomeBanner(): void {
    console.log(`Peter's Wordle Solver\nCopyright (C) 2025 Peter Gordon <codergeek42@gmail.com>\n`);
    console.log(WelcomeBanner);
}

export function displayNoWarrantyInformation(): void {
    console.log(DisclaimerOfWarranty);
}

export function displayCopyleftInformation(): void {
    console.log(CopyleftInformation);
}
