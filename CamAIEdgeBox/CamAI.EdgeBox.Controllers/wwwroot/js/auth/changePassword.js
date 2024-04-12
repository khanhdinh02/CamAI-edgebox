$(document).ready(function () {
    $('#changePasswordForm').submit(function (event) {
        event.preventDefault();

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

        $.ajax({
            url: `http://${document.location.host}/api/auth/password`,
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
