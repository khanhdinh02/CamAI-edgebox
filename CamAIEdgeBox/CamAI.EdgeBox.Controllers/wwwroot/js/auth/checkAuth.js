$(document).ready(function () {
    const token = localStorage.getItem('Token');

    // Check if token is available
    if (!token) {        
        window.location.href = '/index.html';
        alert('Please login first');
        return;
    }
})