const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (user, message, sentiment) => {
    const msgClass = getSentimentClass(sentiment);
    const msgElement = document.createElement("div");
    msgElement.className = `message ${msgClass} mb-2 p-2 rounded`;
    msgElement.innerHTML = `<strong>${user}:</strong> ${message} ${getSentimentIcon(sentiment)}`;
    document.getElementById("messages").appendChild(msgElement);
    scrollToBottom();
});

connection.start().catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", event => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    if (user && message) {
        connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
        document.getElementById("messageInput").value = "";

        // ---Focus on field after message sent---
        document.getElementById("messageInput").focus();
    }

    event.preventDefault();
});

function getSentimentClass(sentiment) {
    switch (sentiment) {
        case 'Positive': return 'positive';
        case 'Negative': return 'negative';
        default: return 'neutral';
    }
}

function getSentimentIcon(sentiment) {
    switch (sentiment) {
        case 'Positive': return '😊';
        case 'Negative': return '😠';
        default: return '😐';
    }
}

function scrollToBottom() {
    const messagesDiv = document.getElementById("messages");
    messagesDiv.scrollTop = messagesDiv.scrollHeight;
}