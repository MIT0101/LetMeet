

/*******************************---------REQUIRED FILEDS [isSupervisor,isStudent,isAdmin] as boolean ---------************************* */
const endDateSearch = document.getElementById("endDateSearch");
const startDateSearch = document.getElementById("startDateSearch");
const createMeetingBtn = document.getElementById("createMeetingBtn")

SetCurrentStartAndEndDate();

if (isSupervisor) {
    SetCurrentSelect("studentId", "studentIdSearch");
    SetCurrentSelect("studentId", "studentIdCreate");
}
if (isAdmin) {
    SetCurrentSelect("supervisionId", "supervisionsSearch");
}
const searchBtn = document.getElementById("searchBtn");
searchBtn.addEventListener("click", (e) => {
    if (isStudent) {
        SearchMeetingForStudent();
    }
    if (isSupervisor) {
        SearchMeetingForSupervisor();
    }
    if (isAdmin) {
        AdminSearch();
    }
});

/*Admins Search*/
function AdminSearch() {
    let startDateValue;
    let endDateValue;
    let canGetStartAndEnd = GetStartDateAndEndDate((startDateValueResult, endDateValueResult) => {
        startDateValue = startDateValueResult;
        endDateValue = endDateValueResult;
    });
    if (!canGetStartAndEnd) {
        return;
    }
    const queryParams = new URLSearchParams(window.location.search);
    SetStartAndEndDate(queryParams, startDateValue, endDateValue);

    //get supervisionId from select
    const supervisionId = document.getElementById("supervisionsSearch").value;
    //check if supervisionId is empty if true return message
    if (!supervisionId) {
        CreateAndshowMessagesModal(["Supervision Is Required"]);
        return;
    }
    SetSupervisionId(queryParams, supervisionId);
    StartSearchByParams(queryParams);

}
/*Supervisors Search*/
function SearchMeetingForSupervisor() {
    let startDateValue;
    let endDateValue;
    let canGetStartAndEnd = GetStartDateAndEndDate((startDateValueResult, endDateValueResult) => {
        startDateValue = startDateValueResult;
        endDateValue = endDateValueResult;
    });
    if (!canGetStartAndEnd) {
        return;
    }
    //get the selected student id
    const studentId = document.getElementById("studentIdSearch").value;
    // build the new query string with the updated start and end date values
    const queryParams = new URLSearchParams(window.location.search);
    //update start and end date
    SetStartAndEndDate(queryParams, startDateValue, endDateValue);
    SetStudentId(queryParams, studentId);
    // start search
    StartSearchByParams(queryParams);

}

/*Students Search*/
function SearchMeetingForStudent() {
    let startDateValue;
    let endDateValue;
    let canGetStartAndEnd = GetStartDateAndEndDate((startDateValueResult, endDateValueResult) => {
        startDateValue = startDateValueResult;
        endDateValue = endDateValueResult;
    });
    if (!canGetStartAndEnd) {
        return;
    }
    // build the new query string with the updated start and end date values
    const queryParams = new URLSearchParams(window.location.search);
    //update start and end date
    SetStartAndEndDate(queryParams, startDateValue, endDateValue);
    // start search
    StartSearchByParams(queryParams);

}
/*********************************************************---------CREATE MEETING----------*********************************/
// to show create meeting modal
createMeetingBtn.addEventListener("click", (e) => {
    //get student id from select
    const studentId = document.getElementById("studentIdCreate").value;
    if (!studentId) {
        //show message if student id is empty
        CreateAndshowMessagesModal(["Student Is Required"]);
        return;
    }
    let CreateMeetingUrl = `/Meeting/Create?studentId=${studentId}`;
    //redirect to create meeting page
    window.location.href = CreateMeetingUrl;

});

/***********************************************------------SHARED SEARCH FUNCTIONS-------------******************************/
function StartSearchByParams(queryParams) {
    // get the current URL without the query string
    const currentUrl = window.location.origin + window.location.pathname;
    // create a new URL with the updated query string
    const newUrl = currentUrl + "?" + queryParams.toString();
    // reload the page with the new URL
    window.location.href = newUrl;
}

function SetStartAndEndDate(queryParams, startDateValue, endDateValue) {
    queryParams.set("startDate", new Date(startDateValue).toISOString().slice(0, 10));
    queryParams.set("endDate", new Date(endDateValue).toISOString().slice(0, 10));
}
function SetStudentId(queryParams, studentId) {
    queryParams.set("studentId", studentId);
}
function SetSupervisionId(queryParams, supervsionId) {
    queryParams.set("supervisionId", supervsionId);
}
function GetStartDateAndEndDate(OnResult) {
    //get satrt date
    const startDateInput = document.getElementById("startDateSearch");
    const endDateInput = document.getElementById("endDateSearch");

    // get the start date and end date values and format them
    const startDateValue = startDateInput.value;
    const endDateValue = endDateInput.value;

    if (!startDateValue) {
        CreateAndshowMessagesModal(["Start Date Is Required"]);
        return false;
    }
    if (!endDateValue) {
        CreateAndshowMessagesModal(["End Date Is Required"]);
        return false;

    }
    if (OnResult) {
        OnResult(startDateValue, endDateValue);

    }
    return true;
}

/*Set Current Start And End Date From Query*/
function SetCurrentStartAndEndDate() {
    const queryParams = new URLSearchParams(window.location.search);
    const startDate = queryParams.get("startDate");
    const endDate = queryParams.get("endDate");
    if (startDate) {
        startDateSearch.value = startDate;
    }
    if (endDate) {
        endDateSearch.value = endDate;
    }
}
/*Set Current select Field Value*/
function SetCurrentSelect(fieldName, inputId) {
    const selectInput = document.getElementById(inputId);
    // get the current query string parameters
    const urlParams = new URLSearchParams(window.location.search);

    // set the student ID input value to the query string value, if present, or the first option otherwise
    if (urlParams.has(fieldName)) {
        let fieldValue = urlParams.get(fieldName);
        selectInput.value = fieldValue;
    }
    const fieldValue = urlParams.get(fieldName);
    // select the corresponding option based on the query string value
    const option = selectInput.querySelector(`option[value="${fieldValue}"]`);
    if (option) {
        option.selected = true;
    }
    else {
        // select the first option by default
        selectInput.selectedIndex = 0;
    }

}
