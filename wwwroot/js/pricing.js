// wwwroot/js/pricing.js
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/subcribe/priceinfo").build();

//Disable send button until connection is established
// document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("startStreaming").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("startStreaming").addEventListener("click", function (event) {
    var uic = document.getElementById("userInput").value;
    var assetType = document.getElementById("messageInput").value;
    // connection.invoke("SendMessage", user, message).catch(function (err) {
    //     return console.error(err.toString());
    // });
    connection.stream("Counter", uic, assetType)
        .subscribe({
            next: (item) => {
                var li = document.createElement("li");
                li.textContent = item;
                document.getElementById("messagesList").appendChild(li);
            },
            complete: () => {
                var li = document.createElement("li");
                li.textContent = "Stream completed";
                document.getElementById("messagesList").appendChild(li);
            },
            error: (err) => {
                var li = document.createElement("li");
                li.textContent = err;
                document.getElementById("messagesList").appendChild(li);
            },
        });
    event.preventDefault();
});