$(document).ready(function () {
    $('#loginForm').submit(function (event) {
        event.preventDefault();

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
