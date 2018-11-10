// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('.carousel .carousel-item').each(function () {
    var next = $(this).next();
    if (!next.length) {
        next = $(this).siblings(':first');
    }
    next.children(':first-child').clone().appendTo($(this));

   
});


// Set active img as main img
$('.carousel-control-next').click(function () {
    // main image
    var mainImage = $(this).closest('.mein-card').find('.mein-card-img').find('img');
    mainImage.prop('src', $(this).closest('.carousel').children('.carousel-inner')
        .children('.active').children('img').prop('src'));

});

$('.carousel-control-prev').click(function () {
    // main image
    var mainImage = $(this).closest('.mein-card').find('.mein-card-img').find('img');
    mainImage.prop('src', $(this).closest('.carousel').children('.carousel-inner')
        .children('.active').children('img').prop('src'));

});


//sidebar
$(document).ready(function () {

    $('#sidebarCollapse').on('click', function () {
        $('.sidebar').toggleClass('mobile-show');
    });

}); 