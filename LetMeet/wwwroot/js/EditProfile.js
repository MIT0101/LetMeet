//for disable button and make it saving
function DisplayprogressMessage(formId, ctl, msg) {
    event.preventDefault();
    $('#' + formId).validate();
    if ($('#' + formId).valid()) {
        $(ctl).prop("disabled", true).text(msg);
        //submit the f0rm.
        setTimeout(function () {
            $('#' + formId).submit();

        }, 1);

        return true;
    }
}


let cmpStrInput = document.getElementById("cmpStrInput");

$('.newbtn').bind("click", function () {
    $('#picInput').click();
});



//start new


//compress New
const compressImageNew = async (file, { quality = 1, type = file.type }) => {
    // Get as image data
    const imageBitmap = await createImageBitmap(file);

    // Draw to canvas
    const canvas = document.createElement('canvas');
    canvas.width = imageBitmap.width;
    canvas.height = imageBitmap.height;
    const ctx = canvas.getContext('2d');
    ctx.drawImage(imageBitmap, 0, 0);

    // Turn into Blob
    const blob = await new Promise((resolve) =>
        canvas.toBlob(resolve, type, quality)
    );

    // Turn Blob into File
    return new File([blob], file.name, {
        type: blob.type,
    });
};

// Get the selected file from the file input
const input = document.getElementById("picInput");
input.addEventListener('change', async (e) => {
    // Get the files
    const { files } = e.target;

    // No files selected
    if (!files.length) return;

    // We'll store the files in this data transfer object
    const dataTransfer = new DataTransfer();

    // For every file in the files list
    for (const file of files) {
        // We don't have to compress files that aren't images
        if (!file.type.startsWith('image')) {
            // Ignore this file, but do add it to our result
            dataTransfer.items.add(file);
            continue;
        }
        console.log("Size Befor " + (file.size / 1024) + " kb");


        // We compress the file by 50%
        const compressedFile = await compressImageNew(file, {
            quality: 0.7,
            type: 'image/jpeg',
        });

        console.log("Size After " + (compressedFile.size / 1024) + " kb");

        // Save back the compressed file instead of the original file
        dataTransfer.items.add(compressedFile);

        // Preview the compressed image
        const compressedUrl = URL.createObjectURL(compressedFile);

        document.getElementById('previewImage').src = compressedUrl;

    }

    // Set value of the file input to our new files list
    e.target.files = dataTransfer.files;
});

                                    // end new