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

connection.on("RefreshTodoComment", function (message,notificationMessage,commentedTodoId) {
    const divToAppend = document.querySelector(".feed-activity-list");
    if (divToAppend) {
        const todoId = divToAppend.getAttribute('data-todoId')
        if (todoId == commentedTodoId) {
            divToAppend.innerHTML += message;
        }
    }
    loadNotificationCount();
    document.querySelector(".signalRMessage").innerHTML = notificationMessage;
    $('#SignalRNotification').modal('show');
    
});

connection.on("CompleteTodo", function (message, notificationMessage, commentedTodoId) {
    const divToAppend = document.querySelector("#main-content");
    if (divToAppend) {
        const todoId = divToAppend.getAttribute('data-todoId')
        if (todoId == commentedTodoId) {
            divToAppend.innerHTML = message;
            document.querySelector(".commentDiv").classList.add("hidden")
            document.querySelector(".reminderDiv").classList.add("hidden")
        }
    }
    loadNotificationCount();
    document.querySelector(".signalRMessage").innerHTML = notificationMessage;
    $('#SignalRNotification').modal('show');
   
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});
