$(document).ready(function () {

    $.ajax({
        url: `http://${document.location.host}/api/cameras`,
        beforeSend: function (request) {
            request.setRequestHeader("Authorization", localStorage.getItem("Token"));
        },
        type: 'GET',
        success: function (xhr) {
            console.log(xhr)
            const tableBody = $('#cameraTable tbody');

            xhr.forEach(function (camera, index) {
                const row = $(`<tr data-id=${camera.id}>`);
                row.html(`
            <td style="display:none">${camera.id}</td>
            <td scope="row">${index + 1}</td>
            <td>${camera.name}</td>
            <td>${camera.zone}</td>
            <td>${camera.willRunAI}</td>
            <td>${camera.status}</td>
          `);
                tableBody.append(row);
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    })

    $('#cameraTable tbody').on('click', 'tr', function () {
        const cameraId = $(this).data('id');
        window.location.href = `/cameraDetail.html?id=${cameraId}`;
    });
});
