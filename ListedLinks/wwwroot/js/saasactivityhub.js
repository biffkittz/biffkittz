// TODO: Make it useful

"use strict";

window.signalRConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://biffkittz.com/saasactivityhub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function startSignalRConnection() {
    try {
        await window.signalRConnection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

window.signalRConnection.on("ReceiveMessage", function (user, message) {
    console.log("Received SignalR message: " + message);

    // Create the main card container
    const card = document.createElement('div');
    card.className = 'card';
    card.style.width = '1000px';
    card.style.marginBottom = '10px';

    // Create the card header
    const cardHeader = document.createElement('div');
    cardHeader.className = 'card-header';
    cardHeader.textContent = (new Date()).toUTCString();

    // Create the card body
    const cardBody = document.createElement('div');
    cardBody.className = 'card-body';

    // Create the card text paragraph
    const cardText = document.createElement('p');
    cardText.className = 'card-text';
    cardText.textContent = message;

    // Assemble the card
    cardBody.appendChild(cardText);
    card.appendChild(cardHeader);
    card.appendChild(cardBody);

    document.getElementsByTagName('main')[1].appendChild(card);
    window.scrollTo(0, document.body.scrollHeight);
    //document.getElementById("messagesList").appendChild(card);
});

window.signalRConnection.onclose(async () => {
    await startSignalRConnection();
});

startSignalRConnection()