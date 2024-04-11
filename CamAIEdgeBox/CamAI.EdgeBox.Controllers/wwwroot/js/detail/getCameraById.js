$(document).ready(function () {

    const urlParams = new URLSearchParams(window.location.search);
    const cameraId = urlParams.get('id');

    $.ajax({
        url: `http://${document.location.host}/api/cameras/${cameraId}`,
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        type: 'GET',
        success: function (data) {
            console.log(data)
            displayCameraDetails(data);
            initialCameraIntoForm(data)
        },
        error: function (xhr, status, error) {
            console.error('Error fetching camera details:', error);
            $('#cameraDetails').html('An error occurred while fetching camera details.');
        }
    });

    function displayCameraDetails(camera) {
        $('#name').html(camera.name);
        $('#zone').html(camera.zone);
        $('#protocol').html(camera.protocol);
        $('#status').html(camera.status);
        $('#host').html(camera.host);
        $('#port').html(camera.port);
        $('#path').html(camera.path);
        $('#willRunAI').html(camera.willRunAI);
    }

    function initialCameraIntoForm(camera) {
        $('#updateName').val(camera.name);
        $('#updateUsername').val(camera.username);
        $('#updatePassword').val(camera.password);
        $('#updateProtocol').val(camera.protocol);
        $('#updatePort').val(camera.port);
        $('#updateHost').val(camera.host);
        $('#updatePath').val(camera.path);
        $('#updateStatus').val(camera.status);
        $(`input[name='updateZone'][value='${camera.zone == "Cashier" ? "0" : "1"}']`).prop('checked', true);
        $('#updateWillRunAi').prop('checked', camera.willRunAI === true);
    }
});