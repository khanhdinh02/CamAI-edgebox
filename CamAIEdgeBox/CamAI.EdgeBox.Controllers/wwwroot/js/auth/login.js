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
        // const formData = {
        //     "username": "admin",
        //     "password": "7a0f5308a6ee401d90c26f4d2c8b4b01"
        // }

        $.ajax({
            url: 'http://localhost:8080/api/auth/login',
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
