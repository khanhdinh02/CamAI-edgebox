$(document).ready(function () {
    $('#deleteButton').click(function () {
        const urlParams = new URLSearchParams(window.location.search);
        const cameraId = urlParams.get('id');

        $.ajax({
            url: `http://${document.location.host}/api/cameras/${cameraId}`,
            beforeSend: function (request) {
                request.setRequestHeader("Authorization", localStorage.getItem("Token"));
            },
            type: 'DELETE',
            success: function (data, status, xhr) {
                window.location.href = `/cameraList.html`;
            },
            error: function (xhr, status, error) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                $('#errorMessage').text(errorMessage);
            }
        });
    });
});
