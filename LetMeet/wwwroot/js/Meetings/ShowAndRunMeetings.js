// must found an array called   'myMeetings' as this    let myMeetings = @Html.Raw(Json.Serialize(meetings));
//must found an object called   'paddingHours' as this  let paddingHours = '@_serviceOptions.Value.PaddingMeetHours';
//******************************-------JQuery is Required--------************
let meetingsMap = new Map();
//loop through myMeeting and add to map the key is the id and the value is the object
myMeetings.forEach(meeting => {
    meetingsMap.set(meeting.id, meeting);
});
/*****************************************************************------RIGISTER STUDENT PRESENSE ------**************************************************/

const studentPresentBtns = document.querySelectorAll(".studentPresentBtn");
studentPresentBtns.forEach((btn) => {
    btn.addEventListener("click", (e) => {
        // get the meeting id
        const meetingId = btn.getAttribute("data-meeting-id");
        const url = `/Meetings/api/RegisterStudentPresence/${meetingId}`;
        //send request to server
        CreateAndShowLoadingOverLay("Registering Student Presence ....");

        SendRequest({ url}, (data) => {
            hideLoadingOverlayIfShown();
            // check if the request is success
            if (data.isSuccess) {
                // show success message
                CreateAndshowMessagesModal(["Student Presence Registered Successfully"]);
                window.location.reload();
                return;
            }
            // show error message
            CreateAndshowMessagesModal(data.errors);

        }, (error) => {
            hideLoadingOverlayIfShown();
            // show error message
            CreateAndshowMessagesModal(["Error Happened Please Try Again"]);
        });


    });

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
    $('#meetingIdRun').text(meeting.id);
    $('#meetingId').val(meeting.id);
    $('#studentId').val(meeting.studentId);
    $('#supervisorId').val(meeting.supervisorId);
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
/*****************************************************************------COMPLETE MEETING------**************************************************/
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
        data.meetingTasks = tasks;
        data.isStudentPresent = isStudentPresent;

        CreateAndShowLoadingOverLay("Saving Meeting Complete ....");
        //get url from form action
        let url = runMeetFrm.getAttribute("action");

        console.log("Data to send", data);

        SendRequest({ url, data }, (data) => {
            hideLoadingOverlayIfShown();
            // check if the request is success
            if (data.isSuccess) {
                console.log("all response is ", data);
                // show success message
                CreateAndshowMessagesModal(["Meeting Completed Successfully"]);
                window.location.reload();
                return;
            }
            // show error message
            CreateAndshowMessagesModal(data.errors);

        }, (error) => {
            hideLoadingOverlayIfShown();
            // show error message
            CreateAndshowMessagesModal(["Error Happened Please Try Again"]);
        });

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
                CreateAndshowMessagesModal(["Meeting Removed Successfully"]);

                var meetingMainCard = btn.closest(".meeting-main-card");

                meetingMainCard.remove();
                // remove meeting from meetingsMap
                meetingsMap.delete(parseInt(meetingId));
                return;
            }
            // create and alerts from errors in data.errors
            if (data.errors && data.errors.length > 0) {
                CreateAndshowMessagesModal(data.errors);
            }

        }, (error) => {
            // Hide loading message
            hideLoadingOverlayIfShown();
            // show error alert above form
            CreateAndshowMessagesModal(["Error happen try again"]);
        });

    });
});

/*****************************************************************------SHARED FUNCTIONS------**************************************************/

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

    // Formatting the date to match the input format
    //const formattedDate = date.toISOString().substring(0, 19);
    return date.setHours(date.getHours() + parseInt(hours));;
}



