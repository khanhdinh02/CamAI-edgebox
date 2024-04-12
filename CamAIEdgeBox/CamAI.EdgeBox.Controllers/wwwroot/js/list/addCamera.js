$(document).ready(function () {
    $('#addCameraForm').submit(function (event) {
        event.preventDefault();

        const zoneCheck = $('.zone:checked').map(function () {
            return $(this).val();
        }).get();

        const aiCheck = $("#willRunAi").is(':checked', function(){
            $("#willRunAi").attr('value', 'true');
        });

        const formData = {
            name: $('#name').val(),
            zone: Number(zoneCheck[0]),
            username: $('#username').val(),
            password: $('#password').val(),
            protocol: $('#protocol').val(),
            port: $('#port').val(),
            host: $('#host').val(),
            path: $('#path').val(),
            willRunAi: aiCheck,
        };

        console.log(formData)

        $.ajax({
            url: `http://${document.location.host}/api/cameras`,
            beforeSend: function (request) {
                request.setRequestHeader("Authorization", localStorage.getItem("Token"));
            },
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (data, status, xhr) {
                console.log(data)
                alert("Camera Added!")
                window.location.href = "/cameraList.html"
            },
            error: function (xhr, status, error) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                $('#errorMessage').text(errorMessage); // Display error message
            }
        });
    });
});
