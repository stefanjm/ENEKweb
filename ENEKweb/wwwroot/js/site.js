// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$(document).ready(function () {

    // sidebar
    $('#sidebarCollapse').on('click', function () {
        $('.sidebar').toggleClass('mobile-show');
    });

    // image slider
    $('.img-next').on('click', function () {
        var currentImg = $(this).closest("div").find("div.active").first("img");
        var nextImg = currentImg.next("div").first("img");

        if (nextImg.length) {
            currentImg.removeClass('active');
            nextImg.addClass('active').show(1000);
        }
    });

    $('.img-prev').on('click', function () {
        var currentImg = $(this).closest("div").find("div.active").first("img");
        var prevImg = currentImg.prev("div").first("img");

        if (prevImg.length) {
            currentImg.removeClass('active');
            prevImg.addClass('active').show(1000);
        }
    });

}); 