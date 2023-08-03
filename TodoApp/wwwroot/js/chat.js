"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/TodoHub").build();




connection.on("NotifyUserAndRefreshDashboard", function (message) {
    loadDashboardData();
    document.querySelector(".signalRMessage").innerHTML = message;
    $('#SignalRNotification').modal('show');
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});