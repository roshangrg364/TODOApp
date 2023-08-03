function loadDashboardData() {
    blockwindow();
    $.ajax({
        url: '/api/dashboard/',
        type: "get",
        success: function (data) {
            if (data) {
                console.log(data)
                const totalUserElm = $("#total-users")
                const totalTodosElm = $("#total-todos")
                const totalActiveTodos = $("#active-todos")
                const totalCompletedTodos = $("#completed-todos")
                const totalSharedTodos = $("#shared-todos")

                if (totalUserElm) totalUserElm.text(data.Data.TotalUser)
                if (totalTodosElm) totalTodosElm.text(data.Data.TotalTodoCount)
                if (totalActiveTodos) totalActiveTodos.text(data.Data.TotalActiveTodoCount)
                if (totalCompletedTodos) totalCompletedTodos.text(data.Data.TotalCompletedTodo)
                if (totalSharedTodos) totalSharedTodos.text(data.Data.TotalSharedTodoCount)

                //load piechart
                var ctx = document.getElementById("myPieChart");
                if (ctx) {
                    var myPieChart = new Chart(ctx, {
                        type: 'doughnut',
                        data: {
                            labels: ["Total", "Active", "Completed", "Shared"],
                            datasets: [{
                                data: [data.Data.TotalTodoCount, data.Data.TotalActiveTodoCount, data.Data.TotalCompletedTodo, data.Data.TotalSharedTodoCount],
                                backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                                hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                                hoverBorderColor: "rgba(234, 236, 244, 1)",
                            }],
                        },
                        options: {
                            maintainAspectRatio: false,
                            tooltips: {
                                backgroundColor: "rgb(255,255,255)",
                                bodyFontColor: "#858796",
                                borderColor: '#dddfeb',
                                borderWidth: 1,
                                xPadding: 15,
                                yPadding: 15,
                                displayColors: false,
                                caretPadding: 10,
                            },
                            legend: {
                                display: false
                            },
                            cutoutPercentage: 80,
                        },
                    });
                }



                // load chart
                var ctx = document.getElementById("myAreaChart");
                if (ctx) {
                    var myLineChart = new Chart(ctx, {
                        type: 'line',
                        data: {
                            labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                            datasets: [{
                                label: "Earnings",
                                lineTension: 0.3,
                                backgroundColor: "rgba(78, 115, 223, 0.05)",
                                borderColor: "rgba(78, 115, 223, 1)",
                                pointRadius: 3,
                                pointBackgroundColor: "rgba(78, 115, 223, 1)",
                                pointBorderColor: "rgba(78, 115, 223, 1)",
                                pointHoverRadius: 3,
                                pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
                                pointHoverBorderColor: "rgba(78, 115, 223, 1)",
                                pointHitRadius: 10,
                                pointBorderWidth: 2,
                                data: data.Data.TotalTodosEachMonth,
                            }],
                        },
                        options: {
                            maintainAspectRatio: false,
                            layout: {
                                padding: {
                                    left: 10,
                                    right: 25,
                                    top: 25,
                                    bottom: 0
                                }
                            },
                            scales: {
                                xAxes: [{
                                    time: {
                                        unit: 'date'
                                    },
                                    gridLines: {
                                        display: false,
                                        drawBorder: false
                                    },
                                    ticks: {
                                        maxTicksLimit: 7
                                    }
                                }],
                                yAxes: [{
                                    ticks: {
                                        maxTicksLimit: 5,
                                        padding: 10,
                                        // Include a dollar sign in the ticks
                                        callback: function (value, index, values) {
                                            return '$' + number_format(value);
                                        }
                                    },
                                    gridLines: {
                                        color: "rgb(234, 236, 244)",
                                        zeroLineColor: "rgb(234, 236, 244)",
                                        drawBorder: false,
                                        borderDash: [2],
                                        zeroLineBorderDash: [2]
                                    }
                                }],
                            },
                            legend: {
                                display: false
                            },
                            tooltips: {
                                backgroundColor: "rgb(255,255,255)",
                                bodyFontColor: "#858796",
                                titleMarginBottom: 10,
                                titleFontColor: '#6e707e',
                                titleFontSize: 14,
                                borderColor: '#dddfeb',
                                borderWidth: 1,
                                xPadding: 15,
                                yPadding: 15,
                                displayColors: false,
                                intersect: false,
                                mode: 'index',
                                caretPadding: 10,
                                callbacks: {
                                    label: function (tooltipItem, chart) {
                                        var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                                        return datasetLabel + ': $' + number_format(tooltipItem.yLabel);
                                    }
                                }
                            }
                        }
                    });
                }

            }
        },
        error: function (errRespons) {
            console.log(errRespons)
        }
    })
    unblockwindow();
}


function blockwindow() {
    document.querySelector(".loading").classList.remove("hidden")
}

function unblockwindow() {
    document.querySelector(".loading").classList.add("hidden")
}

function shownotification(status, message, title="Notification!") {
    Swal.fire({ 
        title: title,
        text: message,
        icon: status,
        showConfirmButton: false,
        timer: 1500
    });
}