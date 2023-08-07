$(document).on("click", ".comment-on-todo", function (e) {
    e.preventDefault()
    const todoId = $("#Id").val();
    const comment = $("#Comment").val();
    const data = {
        TodoId: todoId,
        Comment: comment
    };
    blockwindow()
    $.ajax({
        url: "/api/todo/comment",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data) {
            if (data.IsSuccess == true) {
                const divToAppend = document.querySelector(".feed-activity-list");
                divToAppend.innerHTML += data.Data;
                const comment = $("#Comment");
                comment.val("")
                shownotification(data.Status, data.Message)
            }
            else {
                shownotification(data.Status, data.Message)
            }
            unblockwindow();
        },
        error: function (response) {
            console.log(response)
            unblockwindow();
        }
    })
})

$(document).on("click", ".mark-as-complete", function (e) {
    e.preventDefault()
    const todoId = $("#Id").val();
    const comment = $("#Comment").val();
    const data = {
        TodoId: todoId,
        Comment: comment
    };
    blockwindow()
    $.ajax({
        url: "/api/todo/complete",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data) {
            if (data.IsSuccess == true) {
                $("#main-content").html(data.Data)
                document.querySelector(".commentDiv").classList.add("hidden")
                document.querySelector(".reminderDiv").classList.add("hidden")
                shownotification(data.Status, data.Message)
            }
            else {
                shownotification(data.Status, data.Message)
            }
            unblockwindow();
        },
        error: function (response) {

            unblockwindow();
        }
    })
})


$(document).on("click", ".delete-todo", function (e) {
    e.preventDefault()
    blockwindow()
    Swal.fire({
        title: 'Do you want to delete this todo?.Deleting this will delete all the todo references.',
        showCancelButton: true,
        confirmButtonText: 'Yes',
    }).then((result) => {
        if (result.isConfirmed) {
            const todoId = $("#Id").val()
            $.ajax({
                type: "delete",
                url: "/api/todo/" + todoId,
                success: function (response) {

                    if (response.IsSuccess == true) {
                        shownotification("success", "deleted Successfully")
                        window.location = "/Todo/Todo/Index";
                        unblockwindow()
                    }
                    else {
                        shownotification("info", response.Message)
                        unblockwindow()
                    }
                },
                error: function (error) {
                    unblockwindow()
                    shownotification("error", "something went wrong. please contact to administrator")
                }
            })
        }
        else {
            unblockwindow();
        }
    })

})



$(document).on("click", "#set-remainder", function (e) {
    e.preventDefault();
    const remainderDate = $("#RemainderDate").val();
    if (!remainderDate) {
        shownotification("info", "Please select a reminder date");
        return false;
    }
    blockwindow();
    const todoId = $(this).attr("data-todoId");
    const data = {
        TodoId: todoId,
        RemainderOn: remainderDate
    };
    $.ajax({
        url: "/api/todo/set-remainder",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data) {
            if (data.IsSuccess == true) {
                const unSetReminderDiv = document.querySelector(".unset-remainder");
                const setReminderDiv = document.querySelector(".set-reminder");
                unSetReminderDiv.classList.remove("hidden");
                setReminderDiv.classList.add("hidden");
                $("#RemainderModel").modal("hide")
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');
                unblockwindow();
                shownotification(data.Status, data.Message)
            }
            else {
                shownotification(data.Status, data.Message)
            }

        },
        error: function (response) {

            unblockwindow();
        }
    })
})



$(document).on("click", ".unset-remainder", function (e) {
    e.preventDefault();
    const todoId = $(this).attr("data-todoId");
    blockwindow()
    $.ajax({
        url: "/api/todo/unset-remainder/" + todoId,
        type: "put",
        success: function (data) {
            if (data.IsSuccess == true) {
                const unSetReminderDiv = document.querySelector(".unset-remainder");
                const setReminderDiv = document.querySelector(".set-reminder");
                unSetReminderDiv.classList.add("hidden");
                setReminderDiv.classList.remove("hidden");
                shownotification(data.Status, data.Message)
            }
            else {
                shownotification(data.Status, data.Message)
            }
            unblockwindow();
        },
        error: function (response) {
            shownotification("error", "something went wrong.");
            unblockwindow();
        }
    })
})