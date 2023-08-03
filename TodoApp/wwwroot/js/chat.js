"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/TodoHub").build();




connection.on("NotifyUserAndRefreshDashboard", function (message) {
    loadDashboardData();
    document.querySelector(".signalRMessage").innerHTML = message;
    $('#SignalRNotification').modal('show');
});

connection.on("ShowRemainder", function () {

    loadRemainderNotification();
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});
