$(document).ready(function () {

    $.ajax({
        url: `http://${document.location.host}/api/shops`,
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        type: 'GET',
        success: function (xhr) {
            console.log(xhr)
            $('#shopName').html(xhr.name);
            $('#shopPhone').html(xhr.phone);
            $('#shopEmail').html(xhr.email);
            $('#shopOpenTime').html(xhr.openTime);
            $('#shopCloseTime').html(xhr.closeTime);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching shop details:', error);
            $('#shopDetails').html('An error occurred while fetching shop details.');
        }
    })

    $('#cameraTable tbody').on('click', 'tr', function () {
        const cameraId = $(this).data('id');
        window.location.href = `/cameraDetail.html?id=${cameraId}`;
    });
});
