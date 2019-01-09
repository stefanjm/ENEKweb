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
            if (!nextImg.next("div").hasClass("card-image")) {
                $(this).hide();
            }
            if ($(this).prev('.img-prev').is(":hidden")) {
                $(this).prev('.img-prev').show();
            }
        }
        else {
            
        }
    });

    $('.img-prev').on('click', function () {
        var currentImg = $(this).closest("div").find("div.active").first("img");
        var prevImg = currentImg.prev("div").first("img");

        if (prevImg.length) {
            currentImg.removeClass('active');
            prevImg.addClass('active').show(1000);
            if (!prevImg.prev("div").hasClass("card-image")) {
                $(this).hide();
            }
            if ($(this).next('.img-next').is(":hidden")) {
                $(this).next('.img-next').show();
            }
        }
    });

    // Smoothly scroll to links
    var $root = $('html, body');

    $('a[href^="#"]').click(function () {
        var href = $.attr(this, 'href');

        $root.animate({
            scrollTop: $(href).offset().top
        }, 500, function () {
            window.location.hash = href;
        });

        return false;
    });

    // Scroll back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('#backToTopButton').fadeIn();
        } else {
            $('#backToTopButton').fadeOut();
        }
    });
    // scroll body to 0px on click
    $('#backToTopButton').click(function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
        return false;
    });
    $('#backToTopButton').tooltip();
}); 