$(document).ready(function () {

    // counter animation
    $('.count').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 8000,
            easing: 'swing',
            step: function (now) {
                $(this).text(Math.ceil(now));
            },
            complete: function () {
                if ($(this).hasClass("plus")) {
                    $(this).text($(this).text() + "+");
                }
            }
        });
    });

    // pricing responsive fix
    if ($(window).width() < 514) {
        $('#pricing .container ul').removeClass('three');
    } else {
        $('#pricing .container ul').addClass('three');
    }

});

function SimpleSearch() {
    window.location.href = "/simplesearch?q=" + encodeURIComponent($("#q").val());
}

function KeyUp(e) {
    if (e.keyCode === 13) {
        SimpleSearch();
    }
}