$(document).ready(function () {

    $.ajax({
        url: `http://${document.location.host}/api/brands`,
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        type: 'GET',
        success: function (xhr) {
            // console.log(xhr)
            $('#camName').html(xhr.name);
            $('#camPhone').html(xhr.phone);
            $('#camEmail').html(xhr.email);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching brand details:', error);
            $('#brandDetails').html('An error occurred while fetching brand details.');
        }
    })

    $('#cameraTable tbody').on('click', 'tr', function () {
        const cameraId = $(this).data('id');
        window.location.href = `/cameraDetail.html?id=${cameraId}`;
    });
});
