"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = false;

//Ação de receber a mensagem
connection.on("ReceiveMessage", function (user, receiver, message) {

    //User to User communication
    var userID = document.getElementById("userInput").value;
    //se o usuário for o usuário logado e a url condiser com o receiver mandar a mensagem
    if (userID == receiver){
        var li = document.createElement("li");
        document.getElementById("messageArea").appendChild(li)
        li.textContent = `${user} says ${message}`;
    }

    if (userID == user){
        var li = document.createElement("li");
        document.getElementById("messageArea").appendChild(li)
        li.textContent = `${user} says ${message}`;
    }

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var receiver = document.getElementById("UserId").value;
    connection.invoke("SendMessage", user, receiver, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});