$(document).ready(function () {
    $('#loginForm').submit(function (event) {
        event.preventDefault();

        let isReturn = false;

        if ($('#username').val().length == 0) {
            $('#errorUsernameMessage').text("Username is required");
            isReturn = true;
        } else {
            $('#errorUsernameMessage').text("");
            isReturn = false;
        }

        if ($('#password').val().length == 0) {
            $('#errorPasswordMessage').text("Password is required");
            isReturn = true;
        } else {
            $('#errorPasswordMessage').text("");
            isReturn = false;
        }

        if (isReturn) return;

        const formData = {
            username: $('#username').val(),
            password: $('#password').val()
        };

        $.ajax({
            url: `http://${document.location.host}/api/auth/login`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (data, status, xhr) {
                localStorage.setItem("Token", data.token)
                window.location.href = "/dashboard.html"
            },
            error: function (xhr, status, error) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                $('#errorMessage').text(errorMessage); // Display error message
            }
        });
    });
});
