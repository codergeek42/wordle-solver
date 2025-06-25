import TextMenuBuilder from '../lib/cli/textMenu';

import GPLSections from '../../__data__/gpl-sections.json';

function displayWelcomeBanner(): void {
    console.log(`Peter's Wordle Solver\nCopyright (C) 2025 Peter Gordon <codergeek42@gmail.com>\n`);
    console.log(GPLSections.WelcomeBanner);
}

function displayNoWarrantyInformation(): void {
    console.log(GPLSections.DisclaimerOfWarranty);
}

function displayCopyleftInformation(): void {
    console.log(GPLSections.CopyleftInformation);
}

const MainMenu = new TextMenuBuilder()
    .withTitle('===== Main Menu =====')
    .addEntry({
        text: '(No) Warranty Information',
        callback: displayNoWarrantyInformation
    })
    .addEntry({ text: 'Copyleft Information', callback: displayCopyleftInformation })
    .addEntry({ text: 'Exit', callback: () => process.exit(0) })
    .withPrompt('? ')
    .build();

async function main(): Promise<void> {
    displayWelcomeBanner();
    while (true) {
        await MainMenu.promptAndExecute();
    }
}

if (require.main === module) {
    (async () => await main())();
}
