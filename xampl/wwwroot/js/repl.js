document.addEventListener('DOMContentLoaded', function () {
    const term = new Terminal({
        cursorBlink: true,
        convertEol: true
    });
    const terminalContainer = document.getElementById('terminal-container');
    term.open(terminalContainer);

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/consoleHub")
        .build();

    connection.on("ReceiveOutput", function (message) {
        term.writeln(message);
        term.write('$ ');
    });

    async function startSignalRConnection() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("SignalR connection error: ", err);
            term.writeln("Connection failed. Retrying...");
            setTimeout(startSignalRConnection, 5000);
        }
    };

    connection.onclose(async () => {
        term.writeln("Connection closed. Attempting to restart...");
        console.log("SignalR connection closed. Attempting to restart...");
        await startSignalRConnection();
    });

    startSignalRConnection();



    let currentCommand = '';
    term.onData(e => {
        switch (e) {
            case '\r': // Enter key pressed
                term.writeln('');
                const commandToSend = currentCommand.trim();
                if (commandToSend !== '') {
                    connection.invoke("SendCommand", commandToSend)
                        .catch(err => console.error("Error sending command:", err.toString()));
                }
                currentCommand = '';
                break;
            case '\x7F': // Backspace key pressed
                if (currentCommand.length > 0) {
                    term.write('\b \b');
                    currentCommand = currentCommand.slice(0, -1);
                }
                break;
            default:
                // Only accept printable characters
                if (e >= '\x20' && e <= '\x7E') {
                    term.write(e);
                    currentCommand += e;
                }
                break;
        }
    });
});