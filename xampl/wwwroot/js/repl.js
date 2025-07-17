document.addEventListener('DOMContentLoaded', function () {
    const term = new Terminal({
        cursorBlink: true,
        convertEol: true
    });
    const terminalContainer = document.getElementById('terminal-container');
    term.open(terminalContainer);

    // --- SignalR Setup ---
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/consoleHub") // This must match the path mapped in Program.cs
        .build();

    // Event handler for messages received from the server (our Hub)
    connection.on("ReceiveOutput", function (message) {
        term.writeln(message); // Write the received message to the terminal
        term.write('$ '); // Display prompt after server response
    });

    // Handle connection start and retry logic
    async function startSignalRConnection() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            // Initial message from the server on connection will be handled by "ReceiveOutput"
        } catch (err) {
            console.error("SignalR connection error: ", err);
            term.writeln("Connection failed. Retrying...");
            setTimeout(startSignalRConnection, 5000); // Retry after 5 seconds
        }
    };

    // If the connection unexpectedly closes, attempt to restart
    connection.onclose(async () => {
        term.writeln("Connection closed. Attempting to restart...");
        console.log("SignalR connection closed. Attempting to restart...");
        await startSignalRConnection();
    });

    // Start the connection when the page loads
    startSignalRConnection();
    // --- End SignalR Setup ---


    // --- Xterm.js Input Handling ---
    let currentCommand = '';
    term.onData(e => {
        switch (e) {
            case '\r': // Enter key pressed
                term.writeln(''); // Move to the next line after hitting Enter

                const commandToSend = currentCommand.trim();

                if (commandToSend !== '') {
                    // Send the command to the SignalR Hub
                    // The Hub will then process it and send output back via "ReceiveOutput"
                    connection.invoke("SendCommand", commandToSend)
                        .catch(err => console.error("Error sending command:", err.toString()));
                }

                currentCommand = ''; // Clear the command buffer
                // The prompt '$ ' will be rewritten by the 'ReceiveOutput' handler
                break;

            case '\x7F': // Backspace key pressed
                if (currentCommand.length > 0) {
                    term.write('\b \b'); // Move cursor back, erase character, move cursor back again
                    currentCommand = currentCommand.slice(0, -1); // Remove the last character from the buffer
                }
                break;

            default: // Any other character typed
                // Only accept printable characters
                if (e >= '\x20' && e <= '\x7E') {
                    term.write(e); // Echo the character to the terminal
                    currentCommand += e; // Add character to the command buffer
                }
                break;
        }
    });
});