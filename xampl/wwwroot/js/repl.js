// 1. Initialize the Xterm.js terminal
const term = new Terminal({
    cursorBlink: true, // Make the cursor blink
    convertEol: true   // Convert line feeds to carriage return/line feeds for proper display
});

// 2. Get the HTML container for the terminal
const terminalContainer = document.getElementById('terminal-container');

// 3. Open the terminal in the container
term.open(terminalContainer);

// Optional: Initial message and prompt
term.writeln('Welcome to the Pseudo REPL!');
term.write('$ '); // Display a command prompt

// Variable to store the current command being typed
let currentCommand = '';

// 4. Handle terminal input (keyboard events)
term.onData(e => {
    switch (e) {
        case '\r': // Enter key pressed
            term.writeln(''); // Move to the next line after hitting Enter

            const commandToProcess = currentCommand.trim();

            if (commandToProcess === 'run-c --help') {
                term.writeln('Hello World!'); // Specific response for the command
            } else if (commandToProcess !== '') {
                term.writeln(`Unknown command: ${commandToProcess}`); // Generic response for other commands
            }

            currentCommand = ''; // Clear the command buffer
            term.write('$ '); // Display a new prompt
            break;

        case '\x7F': // Backspace key pressed (ASCII code 127)
            if (currentCommand.length > 0) {
                term.write('\b \b'); // Move cursor back, erase character, move cursor back again
                currentCommand = currentCommand.slice(0, -1); // Remove the last character from the buffer
            }
            break;

        default: // Any other character typed
            // Only accept printable characters (you might want to refine this)
            if (e >= '\x20' && e <= '\x7E') { // ASCII printable characters range
                term.write(e); // Echo the character to the terminal
                currentCommand += e; // Add character to the command buffer
            }
            break;
    }
});