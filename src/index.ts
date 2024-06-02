// FIXME: I expect this to be caught by lint.
const port = 3000;

export async function helloWorld(): Promise<void> {
	console.log('Hello, World!');
}

export async function goodByeWorld(): Promise<void> {
	console.log('Goodbye, World! ...')
}

helloWorld().then(goodByeWorld);