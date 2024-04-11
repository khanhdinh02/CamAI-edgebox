$(document).ready(function () {
    $('#changePasswordForm').submit(function (event) {
        event.preventDefault();

        let isReturn = false;

        if ($('#oldPassword').val().length == 0) {
            $('#errorOldPasswordMessage').text("Old Password is required");
            isReturn = true;
        } else {
            $('#errorOldPasswordMessage').text("");
            isReturn = false;
        }

        if ($('#newPassword').val().length == 0) {
            $('#errorNewPasswordMessage').text("New Password is required");
            isReturn = true;
        } else {
            $('#errorNewPasswordMessage').text("");
            isReturn = false;
        }

        if ($('#oldPassword').val() !== $('#newPassword').val()) {
            $('#errorMessage').text("The passwords are not match")
            isReturn = true;
        } else {
            $('#errorMessage').text("")
            isReturn = false;
        }

        if (isReturn) return;

        const formData = {
            oldPassword: $('#oldPassword').val(),
            newPassword: $('#newPassword').val()
        };
        // const formData = {
        //     "username": "admin",
        //     "password": "7a0f5308a6ee401d90c26f4d2c8b4b01"
        // }

        $.ajax({
            url: 'http://localhost:8080/api/auth/password',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (data, status, xhr) {
                alert("Change password successful, please login again")
                localStorage.removeItem("Token")
                window.location.href = "/index.html"
            },
            error: function (xhr, status, error) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                $('#errorMessage').text(errorMessage); // Display error message
            }
        });
    });
});
