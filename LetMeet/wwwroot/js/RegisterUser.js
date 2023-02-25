let currentStage = document.getElementById("currentStage");
let stageSelections = document.querySelectorAll(".dropdown-item.stageItem");

const baiseButtonTitle = currentStage.textContent;


for (let i = 0; i < stageSelections.length; i++) {
    let item = stageSelections[i].addEventListener('click', (e) => {
        document.getElementById("stage").value = e.target.innerText;
        currentStage.textContent = `${baiseButtonTitle} : ${e.target.innerText}`
    });
}

let currentPostion = document.getElementById("currentPostion");
const baisePostionButtonTitle = currentPostion.textContent;
let postionSelections = document.querySelectorAll(".dropdown-item.postionItem");


for (let i = 0; i < postionSelections.length; i++) {

    let item = postionSelections[i].addEventListener('click', (e) => {
        document.getElementById("userRole").value = e.target.innerText;
        currentPostion.textContent = `${baisePostionButtonTitle} : ${e.target.innerText} `
    });
}

//for disable button and make it saving
function DisplayprogressMessage(ctl, msg) {
    event.preventDefault();
    $('#msform').validate();
    if ($('#msform').valid()) {
        $(ctl).prop("disabled", true).text(msg);
        //submit the f0rm.
        setTimeout(function () {
            $("#msform").submit();

        }, 1);

        return true;
    }
}


restStageAndPostionValues();

function restStageAndPostionValues() {
    currentStage.textContent = `${baiseButtonTitle} : ${stageSelections[0].innerText} `
    currentPostion.textContent = `${baisePostionButtonTitle} : ${postionSelections[0].innerText} `
}
