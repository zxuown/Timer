
//localStorage.removeItem('tableData');
// var tableData = localStorage.getItem('tableData');
// if (tableData) {
//     $("#tbodyId").html(tableData);
// }
var splide = new Splide('.splide', {
    perPage: 3,
    perMove: 3,
});

splide.mount();

var list = []
var temp = true
var timerInterval
var remainingTime = 0
let tokenUser = getToken()
$(document).on('click', '#add-date', e => {
    $("#dateModal").addClass("show")
})
function saveTimer(hours, minutes, seconds, description) {

    fetch("/Home/Create", {
        method: "post",
        headers: {
            'Content-Type': "application/json",
            'Authorization': `Bearer ${tokenUser}`
        },
        body: JSON.stringify({
            hours: hours,
            minutes: minutes,
            seconds: seconds,
            description: description,
        })
    }).then(r => r.text()).then(html => {
       updateLogedUserNavBar()
    })
}


function deleteTimer(id) {
    fetch(`/Home/Delete/${id}`, {
        method: "post",
    }).then(r => r.text()).then(html => {
       updateLogedUserNavBar()
    })
}

function StartTimer(totalTime) {
    clearInterval(timerInterval)
    remainingTime = totalTime
    timerInterval = setInterval(function () {
        var hours = Math.floor(remainingTime / 3600);
        var minutes = Math.floor((remainingTime % 3600) / 60);
        var seconds = remainingTime % 60;
        $(".divTimer").text(
            (hours < 10 ? "0" : "") + hours + ":" +
            (minutes < 10 ? "0" : "") + minutes + ":" +
            (seconds < 10 ? "0" : "") + seconds
        );
        remainingTime--
        if (remainingTime < 0) {
            clearInterval(timerInterval);
            $(".divTimer").text("00:00:00")
            $("#resultTime").text(showLastText())
            showResultModal()
        }
    }, 1000)
}
function showResultModal() {
    document.getElementById("resultModal").style.display = "flex"
}
function showLastText() {
    var resultHours = $("#hoursInput").val() || "00";
    var resultMinutes = $("#minutesInput").val() || "00";
    var resultSeconds = $("#secondsInput").val() || "00";

    resultHours = resultHours.padStart(2, "0");
    resultMinutes = resultMinutes.padStart(2, "0");
    resultSeconds = resultSeconds.padStart(2, "0");

    return resultHours + ":" + resultMinutes + ":" + resultSeconds;
}
$("#setTimerButton").on("click", function () {
    console.log($("#hoursInput").val())
    $("#timerModal").addClass("show")
});
//Work here
let dates = []
$("#saveDate").on("click", function () {
    bootstrap.Modal.getOrCreateInstance(document.getElementById("dateModal")).hide()
    let dateInput = $("#dateInput").val()
    let textInput = $("#dateTitleInput").val()
    

    fetch("/home/create-timers-to-dates", {
        method: "post",
        headers: {
            'Content-Type': "application/json",
            'Authorization': `Bearer ${getToken()}`
        },
        body: JSON.stringify({
            date: moment(dateInput).format('YYYY-MM-DD'),
            label: textInput

        })
    }).then(() => {
        getDateTimers()     
    })

   
});

$("#okButtonModal").on("click", function () {
    if ($('#flexRadioDefault2').is(":checked")) {
        StartTimer(getTotalTime())
    }
    document.getElementById("resultModal").style.display = "none"
});

$("#resetButton").on("click", function () {
    clearInterval(timerInterval)
    remainingTime = 0
    StartTimer(getTotalTime())
});

$("#mainStartButton").on("click", function () {
    if (temp) {
        clearInterval(timerInterval)
        $("#mainStartButton").removeClass("btn btn-danger")
        $("#mainStartButton").addClass("btn btn-success")
        $("#mainStartButton").text("Start")
        temp = false
    } else {
        StartTimer(remainingTime)
        $("#mainStartButton").removeClass("btn btn-success")
        $("#mainStartButton").addClass("btn btn-danger")
        $("#mainStartButton").text("Stop")
        temp = true
    }
});

$(".divModalGrey button.btn-success").on("click", function () {
    $("#timerModal").removeClass("show")
});

$(".divModalGrey button.btn-danger").on("click", function () {
    $("#timerModal").removeClass("show")
})

$("#startTimerButton").on("click", function () {
    var temp = showLastText()
    var parts = temp.split(":")
    if (parseInt(parts[0]) > 24 || parseInt(parts[0]) === 24
        || parts[1].length >= 3 || parts[2].length >= 3
        || parseInt(parts[1]) > 60 || parseInt(parts[2]) > 60)
    {
        return;
    }
    if (parseInt(parts[1]) == 60)
    {
        parts[0] = "01"
        parts[1] = "00"
    }
    if (parseInt(parts[2]) == 60) {
        parts[1] = "01"
        parts[2] = "00"
    }
    console.log(showLastText())
    saveTimer(parts[0], parts[1], parts[2], $("#textFromInput").val())
    if ($('#flexCheckChecked').is(":checked")) {
        var inputString = $("#textFromInput").val()
        $(".divTitle").text(inputString)
    } else {
        $(".divTitle").text("")
    }
    StartTimer(getTotalTime())
    if (remainingTime > 0) {
        $("#mainStartButton").removeClass("btn btn-success")
        $("#mainStartButton").addClass("btn btn-danger")
        $("#mainStartButton").text("Stop")
    }
});

function getTotalTime() {
    var hours = parseInt($("#hoursInput").val() || 0)
    var minutes = parseInt($("#minutesInput").val() || 0)
    var seconds = parseInt($("#secondsInput").val() || 0)

    return hours * 3600 + minutes * 60 + seconds
}
let modal = new bootstrap.Modal(document.getElementById("form-modal"), { keyboard: false })

function getToken() {
    let token = localStorage.getItem('TOKEN')
    return token
}
updateLogedUserNavBar()
function loadTimers() {
    let userToken = getToken()
    if (userToken !== null) {
        fetch("/home/timers", {
            method: "get",
            headers: {
                'Authorization': `Bearer ${userToken}`
            }
        }).then(r => r.text()).then(html => {
            $("#tbodyId").html(html)
        })
    }
}

function getDateTimers() {
    let userToken = getToken()
    if (userToken !== null) {
        fetch("/home/timers-to-dates", {
            method: "get",
            headers: {
                'Authorization': `Bearer ${userToken}`
            }
        }).then(r => r.json()).then(timers => {
            dates = timers
            renderDateTimers()
        })
    }
}

function renderDateTimers() {
    let list = $("#splide-list").empty()
    dates.forEach(x => {
        let days = moment(x.date).set({ hour: 1, minute: 0, second: 1 })
            .diff(moment().set({ hour: 0, minute: 0, second: 1 }), 'days')
        if (days < 0) {
            return
        }
        let dateStr = moment(x.date).locale('uk').format('L')
        let card = $(`<li class="splide__slide" data-id="${x.id}">
                            <div class="card text-white bg-dark mb-3" style="max-width: 18rem;">
                                <div class="card-header">${x.label}
                                <i class="date-delete fa-solid fa-x float-end"></i>
                                </div>
                                <div class="card-body">
                                    <h5 class="card-title divDays">${days}</h5>
                                    <p class="card-text">days to ${dateStr}</p>
                                </div>
                            </div>
                        </li>`)
        list.append(card)
    })
    list.append($(`<li class="splide__slide" id="add-new-date-card">
                            <div class="card text-white bg-dark mb-3" style="max-width: 18rem;">
                                <div class="card-header">...</div>
                                <div class="card-body">
                                    <h5 class="card-title"></h5>
                                    <p class="card-text card-new">
                                        <button type="button" data-bs-toggle="modal" data-bs-target="#dateModal" id="add-date" class="btn btn-primary">Add date</button>
                                    </p>
                                </div>
                            </div>
                        </li>`))
    splide.refresh()
}

function updateLogedUserNavBar() {
    let userToken = getToken()
    if (userToken == null) {
        fetch("/content/navbar", {
            method: "get",
        }).then(r => r.text()).then(html => {
            $("#navbar").html(html)
        })
        $("#tbodyId").empty()
        
        return
    }


    //auth
    fetch("/content/auth-navbar", {
        method: "get",
        headers: {
            'Authorization': `Bearer ${userToken}`
        }
    }).then(r => r.text()).then(html => {
        $("#navbar").html(html)
        loadTimers()

    })
}

//delete-timersToDates
$(document).on('click', "i.date-delete", function(e) {
    let userToken = getToken()
    let id = $(e.target).closest('li').data('id')
    fetch(`/home/delete-timers-to-dates/${id}`, {
        method: "Post",
        headers: {
            'Content-Type': "application/json",
            'Authorization': `Bearer ${userToken}`
        },
    }).then(() => {
      getDateTimers() 
    })
})
//logOut
$(document).on('click', '#logout-button', e => {
    let userToken = getToken()
    fetch("/api/auth/logout", {
        method: "Post",
        headers: {
            'Content-Type': "application/json",
            'Authorization': `Bearer ${userToken}`
        },
    }).then(() => {

        localStorage.removeItem("TOKEN")
        userToken = null
        updateLogedUserNavBar()
        dates = []
        renderDateTimers()
    })
})
//logIn
$(document).on('click', '.login-button', e => {

    fetch("/Content/Index").then(r => r.text()).then(html => {
        $("#form-modal div.modal-body").html(html)
        modal.show()
        $("#login-modal-button").on('click', function (e) {
            let email = $("#Email").val()
            let pass = $("#Password").val()
            fetch("/api/auth/login", {
                method: "Post",
                headers: {
                    'Content-Type': "application/json"
                },
                body: JSON.stringify({
                    email: email,
                    password: pass,
                })
            }).then(r => r.json()).then(json => {
                if (json.success) {
                    modal.hide()
                    localStorage.setItem('TOKEN', json.token)
                    updateLogedUserNavBar()
                    getDateTimers()
                } else {
                    $("#div-error").text(json.error)
                }
            })
        })
    })
})
//register
$(document).on('click', '.register-button', e => {
    fetch("/Content/Register").then(r => r.text()).then(html => {
        $("#form-modal div.modal-body").html(html)
        modal.show()
        $("#register-modal-button").on('click', function (e) {
            let userName = $("#UserName").val()
            let email = $("#Email").val()
            let phone = $("#Phone").val()
            let pass = $("#Password").val()
            fetch("/api/auth/register", {
                method: "Post",
                headers: {
                    'Content-Type': "application/json"
                },
                body: JSON.stringify({
                    username: userName,
                    phone: phone,
                    email: email,
                    password: pass,
                })
            }).then(r => r.json()).then(json => {
                if (json.success) {
                    modal.hide()
                    localStorage.setItem('TOKEN', json.token)
                    updateLogedUserNavBar()
                } else {
                    $("#div-error").text(json.error)
                }
            })
        })
    })
})
//delete Timer button
$(document).on('click', ".timer-delete", function (e) {
    let timerId = Number($(e.target).data("id"))
    deleteTimer(timerId)
})
getDateTimers()
