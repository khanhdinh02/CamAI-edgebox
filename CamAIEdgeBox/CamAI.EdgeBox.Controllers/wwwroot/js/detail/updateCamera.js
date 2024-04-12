$(document).ready(function () {
    $('#updateCameraForm').submit(function (event) {
        event.preventDefault();

        const urlParams = new URLSearchParams(window.location.search);
        const cameraId = urlParams.get('id');

        const zoneCheck = $('.updateZone:checked').map(function () {
            return $(this).val();
        }).get();

        const aiCheck = $("#updateWillRunAi").is(':checked', function () {
            $("#updateWillRunAi").attr('value', 'true');
        });

        const formData = {
            name: $('#updateName').val(),
            zone: Number(zoneCheck[0]),
            username: $('#updateUsername').val(),
            password: $('#updatePassword').val(),
            protocol: $('#updateProtocol').val(),
            port: $('#updatePort').val(),
            host: $('#updateHost').val(),
            path: $('#updatePath').val(),
            willRunAi: aiCheck,
        };

        console.log(formData)

        $.ajax({
            url: `http://${document.location.host}/api/cameras/${cameraId}`,
            beforeSend: function (request) {
                request.setRequestHeader("Authorization", localStorage.getItem("Token"));
            },
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (data, status, xhr) {
                console.log(data)
                alert("Camera Updated!")
                window.location.href = `/cameraDetail.html?id=${cameraId}`;
            },
            error: function (xhr, status, error) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                $('#errorMessage').text(errorMessage);
            }
        });
    });
});
