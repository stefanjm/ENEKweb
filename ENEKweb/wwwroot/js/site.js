// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Cycle through logo style when clicked on

var logoCount = 1;
$("#changeLogo").click(function () {
    if (logoCount < 6) {
        $("#logo_image").attr("src", "/images/logo/nr." + logoCount + ".png");
        logoCount++;
    }
    else {
        logoCount = 1;
    }
});

$("#changeLogoFirst").click(function () {
    if (logoCount < 6) {
        $("#imas").css("background", "url(/images/logo/nr." + logoCount + ".png) no-repeat scroll");
        $("#imas").css("background-position", "center");
        logoCount++;
    }
    else {
        logoCount = 1;
    }
});