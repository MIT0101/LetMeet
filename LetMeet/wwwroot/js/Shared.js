
/***********************************************----------JQuery is required-------------*******************************************/

/***********************************************----------Show Message Modal -------------*******************************************/
//Function to Create And show messages in modal
function CreateAndshowMessagesModal(messages) {
    // get modal by id
    let messagesModal = document.querySelector("#messagesModal");
    if (!messagesModal) {
        messagesModal = document.createElement("div");
        messagesModal.classList.add("modal", "fade");
        messagesModal.id = "messagesModal";
        messagesModal.tabIndex = "-1";
        messagesModal.setAttribute("role", "dialog");
        messagesModal.setAttribute("aria-labelledby", "messagesModalLabel");
        messagesModal.setAttribute("aria-hidden", "true");
        messagesModal.innerHTML = `<div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title" id="messagesModalLabel">Messages</h5> 
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
            <span aria-hidden="true">&times;</span>
            </button>
            </div>
            <div class="modal-body">

            </div>
             <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
            </div>
            </div>`;
        document.body.appendChild(messagesModal);
    }
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



// OnSuccess function to handle success response
// OnFailed function to handle failed response
//AllData object that contains url, data, method
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
    if (loadingOverlay && loadingOverlay.style.display != "none") {
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