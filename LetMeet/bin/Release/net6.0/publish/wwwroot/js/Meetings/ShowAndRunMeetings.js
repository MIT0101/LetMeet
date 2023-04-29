// must found an array called   'myMeetings' as this    let myMeetings = @Html.Raw(Json.Serialize(meetings));
//must found an object called   'paddingHours' as this  let paddingHours = '@_serviceOptions.Value.PaddingMeetHours';
//******************************-------JQuery is Required--------************
let meetingsMap = new Map();
//loop through myMeeting and add to map the key is the id and the value is the object
myMeetings.forEach(meeting => {
    meetingsMap.set(meeting.id, meeting);
});
/*****************************************************************------RUN MEETING------**************************************************/

// all the buttons that run the meeting
const runMeetingButtons = document.querySelectorAll('.runMeetingBtn');
runMeetingButtons.forEach((button) => {
    button.addEventListener('click', function () {
        const meetingCard = button.closest('.card');
        const cardTitle = meetingCard.querySelector('.card-title');
        const meetingId = cardTitle.getAttribute('data-meeting-id');
        ShowMeetingToRun(meetingId);
    });
});
//Function to show meeting To Run
function ShowMeetingToRun(meetingId) {
    const meeting = meetingsMap.get(parseInt(meetingId));

    $('#meetingId').val(meeting.id);
    $('#studentId').val(meeting.studentId);
    $('#supervisorId').val(meeting.studentId);
    $('#supervisorName').val(meeting.supervisorName);
    $('#studentName').val(meeting.studentName);
    $('#startAt').val(formatDate(meeting.date));
    $('#endAt').val(formatDate(addHoursToDate(meeting.date, parseInt(meeting.endHour) - parseInt(meeting.startHour))));
    $('#expireAt').val(formatDate(addHoursToDate(meeting.date, parseInt(meeting.endHour) - parseInt(meeting.startHour) + parseInt(paddingHours))));
    $('#meetingDescription').val(meeting.description);
    let totalTaskText = meeting.tasks.length > 0 ? meeting.tasks.length : 'No Tasks';
    $('#totalTaksToCheck').text(totalTaskText);
    $('#isStudentPresent').prop('checked', meeting.isStudentPresent);
    //generate checkbox element text

    // Clear previous tasks
    $('#tasks-table-body').empty();

    // Add tasks to the table
    meeting.tasks.forEach(task => {
        const completed = task.isCompleted;
        const taskRow = `<tr>
                                                               <td data-task-id='${task.id}'>${GenerateChechBoxText(completed)}</td>
                                                               <td>${task.title}</td>
                                                          </tr>`;
        $('#tasks-table-body').append(taskRow);
    });

    // if there no tasks hide the tasks table
    if (meeting.tasks.length == 0) {
        $('#meeting-tasks').hide();
    } else {
        $('#meeting-tasks').show();
    }

    $('#RunMeetingModal').modal('show');
}
//Function To Generate checkbox
function GenerateChechBoxText(isChecked) {

    let check = isChecked ? "checked" : "";

    let txt = `  <div class="form-check">
                         <label class="form-check-label">
                               <input  class="form-check-input" type="checkbox" value="" ${check}>
                               <span class="form-check-sign">
                                    <span class="check "></span>
                               </span>
                         </label>
                 </div>`

    return txt;
}
//send run meeting data
//get all buttons with completeMeetingBtn class
const completeMeetingBtns = document.querySelectorAll(".completeMeetingBtn");
// loop through all buttons
completeMeetingBtns.forEach((btn) => {
    // add click event to each button
    btn.addEventListener("click", (e) => {
        e.preventDefault();
        let data = {};
        let runMeetFrm = document.getElementById("runMeetFrm");
        let formData = new FormData(runMeetFrm);
        for (const [key, value] of formData) {
            data[key] = value;
        }
        const isStudentPresent = $('#isStudentPresent').is(':checked');
        const tasks = [];
        $('#tasks-table-body tr').each(function () {
            const taskId = $(this).find('td[data-task-id]').data('task-id');
            const isTaskComplete = $(this).find('.form-check-input').is(':checked');
            tasks.push({
                id: taskId,
                isCompleted: isTaskComplete,
            });
        });
        data.hasTasks = tasks.length > 0;
        data.tasks = tasks;
        data.isStudentPresent = isStudentPresent;

        console.log(data);

    });
});
/*****************************************************************------VIEW MEETING------**************************************************/
//button to show meeting to view
const viewMeetingBtns = document.querySelectorAll('.viewMeetingBtn');

viewMeetingBtns.forEach((button) => {
    button.addEventListener('click', function () {
        const meetingCard = button.closest('.card');
        const cardTitle = meetingCard.querySelector('.card-title');
        const meetingId = cardTitle.getAttribute('data-meeting-id');
        showMeetingToView(meetingId);
    });
});

// Function to show meeting details in the modal
function showMeetingToView(meetingId) {
    const meeting = meetingsMap.get(parseInt(meetingId));
    // Fill the modal with meeting data
    $('#meetingIdShow').text(meeting.id);
    $('#supervisorNameShow').val(meeting.supervisorName);
    $('#studentNameShow').val(meeting.studentName);
    $('#createdShow').val(formatDate(meeting.created));
    $('#startAtShow').val(formatDate(meeting.date));
    $('#totalHoursShow').val(meeting.totalTimeHoure + ' Hours');
    $('#meetingDescriptionShow').val(meeting.description);
    let totalTaskText = meeting.tasks.length > 0 ? meeting.tasks.length : 'No Tasks';
    $('#totalTaksShow').text(totalTaskText);
    $('#isStudentPresentShow').val(meeting.isStudentPresent ? 'Yes' : 'No');
    $('#isSupervisorPresentShow').val(meeting.isSupervisorPresent ? 'Yes' : 'No');
    // Clear previous tasks
    $('#tasks-table-bodyShow').empty();

    // Add tasks to the table
    meeting.tasks.forEach(task => {
        const completed = task.isCompleted ? 'Yes' : 'No';
        const taskRow = `<tr>
                                                                                                <td>${task.title}</td>
                                                                                                <td>${task.decription}</td>
                                                                                                <td>${completed}</td>
                                                                                             </tr>`;
        $('#tasks-table-bodyShow').append(taskRow);
    });

    // if there no tasks hide the tasks table
    if (meeting.tasks.length == 0) {
        $('#meeting-tasksShow').hide();
    } else {
        $('#meeting-tasksShow').show();
    }

    $('#ViewMeetingModal').modal('show');
}
/*****************************************************************------REMOVE MEETING------**************************************************/
// get all delete buttons
let deleteMeetingBtns = document.querySelectorAll("#deleteMeetingBtn");
deleteMeetingBtns.forEach(btn => {
    btn.addEventListener("click", (e) => {
        // get meeting id from data-action attribute
        let meetingId = btn.getAttribute("data-action").split("/").pop();
        if (!confirm(`Are you sure you want to delete meeting with id ${meetingId}?`)) {
            return;
        }
        //get meeting id from data-action attribute
        let url = btn.getAttribute("data-action");
        // Show loading message
        CreateAndShowLoadingOverLay("Deleting meeting, please wait...");
        // Send post request to delete meeting
        SendRequest({ url }, (data) => {
            hideLoadingOverlayIfShown();
            if (data.isSuccess) {
                // Hide loading message
                showMessagesModal(["Meeting Removed Successfully"]);

                var meetingMainCard = btn.closest(".meeting-main-card");

                meetingMainCard.remove();
                // remove meeting from meetingsMap
                meetingsMap.delete(parseInt(meetingId));
                return;
            }
            // create and alerts from errors in data.errors
            if (data.errors && data.errors.length > 0) {
                showMessagesModal(data.errors);
            }

        }, (error) => {
            // Hide loading message
            hideLoadingOverlayIfShown();
            // show error alert above form
            showMessagesModal(["Error happen try again"]);
        });

    });
});

/*****************************************************************------SHARED FUNCTIONS------**************************************************/
// OnSuccess function to handle success response
// OnFailed function to handle failed response
function SendRequest(AllData, OnSuccess, OnFailed) {
    let { url, data, method = "POST" } = AllData;
    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then(response => response.json())
        .then(data => { if (OnSuccess(data)) { OnSuccess(data) } })
        .catch(error => { if (OnFailed) { OnFailed(error) } });
}

//Function to hide Loading overlay if its shown
function hideLoadingOverlayIfShown() {
    let loadingOverlay = document.getElementById("loadingOverlay");
    if (loadingOverlay.style.display != "none") {
        let loadingMessage = document.getElementById("loadingMessage");
        loadingMessage.textContent = "";
        loadingOverlay.style.display = "none";
    }
}

// Function to create and show loading overlay
function CreateAndShowLoadingOverLay(message) {
    let loadingOverlay = document.getElementById("loadingOverlay");
    if (!loadingOverlay) {
        loadingOverlay = document.createElement("div");
        loadingOverlay.classList.add("loading-overlay");
        loadingOverlay.id = "loadingOverlay";
        document.body.appendChild(loadingOverlay);

    }
    // add the css to the loading overlay
    let loadingOverlayStyle = document.getElementById("loadingOverlayStyle");
    if (!loadingOverlayStyle) {
        loadingOverlayStyle = document.createElement("style");
        loadingOverlayStyle.id = "loadingOverlayStyle";
        loadingOverlayStyle.textContent = `
                                            .loading-overlay {
                                                position: fixed;
                                                top: 0;
                                                left: 0;
                                                width: 100%;
                                                height: 100%;
                                                background-color: rgba(0, 0, 0, 0.5);
                                                z-index: 9999;
                                                display: flex;
                                                align-items: center;
                                                justify-content: center;
                                            }`;
        document.head.appendChild(loadingOverlayStyle);
    }
    let loadingMessage = document.getElementById("loadingMessage");
    if (!loadingMessage) {
        loadingMessage = document.createElement("h3");
        loadingMessage.classList.add("loading-text", "text-center", "text-white");
        loadingMessage.id = "loadingMessage";
        loadingOverlay.appendChild(loadingMessage);
    }
    loadingMessage.textContent = message;
    loadingOverlay.style.display = "flex";
}

//Function to show messages in modal
function showMessagesModal(messages) {
    var modalBody = document.querySelector("#messagesModal .modal-body");
    modalBody.innerHTML = "";

    messages.forEach(function (message) {
        var messageSpan = document.createElement("span");
        messageSpan.textContent = message;
        messageSpan.classList.add("d-block", "mb-2");
        modalBody.appendChild(messageSpan);
    });

    $("#messagesModal").modal("show");
}
// Function to format date with AM/PM and day of the week
function formatDate(dateString) {
    const date = new Date(dateString);
    const options = { weekday: 'long', year: 'numeric', month: 'numeric', day: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true };
    return new Intl.DateTimeFormat('en-US', options).format(date);
}
// Function to add hours to date
function addHoursToDate(dateStr, hours) {
    const date = new Date(dateStr);
    // Adding hours to the date
    date.setHours(date.getHours() + hours);
    // Formatting the date to match the input format
    const formattedDate = date.toISOString().substring(0, 19);
    return formattedDate;
}



