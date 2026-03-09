$(document).ready(function () {
    $("#refresh").click(function () {
        $.ajax({
            type: 'get',
            url: '/refresh',
            dataType: 'image/png',
            success: function (data) {
                $('#captcha').prop('src', "data:image/png;base64," + jQuery.parseJSON(data.image).image);
            },
            error: function (data) {
                $('#captcha').prop('src', "data:image/png;base64," + jQuery.parseJSON(data.responseText).image);
            }
        });
    })
});