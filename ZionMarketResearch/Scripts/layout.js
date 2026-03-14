$(document).ready(function () {

    // counter animation
    //$('.count').each(function () {
    //    $(this).prop('Counter', 0).animate({
    //        Counter: $(this).text()
    //    }, {
    //        duration: 8000,
    //        easing: 'swing',
    //        step: function (now) {
    //            $(this).text(Math.ceil(now));
    //        },
    //        complete: function () {
    //            if ($(this).hasClass("plus")) {
    //                $(this).text($(this).text() + "+");
    //            }
    //        }
    //    });
    //});

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {

                $(entry.target).prop('Counter', 0).animate({
                    Counter: $(entry.target).text()
                }, {
                    duration: 2000,
                    easing: 'swing',
                    step: function (now) {
                        $(entry.target).text(Math.ceil(now));
                    }
                });

                observer.unobserve(entry.target);
            }
        });
    });

    document.querySelectorAll('.count').forEach(el => observer.observe(el));

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