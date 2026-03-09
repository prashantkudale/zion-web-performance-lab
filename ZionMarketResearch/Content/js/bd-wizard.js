//Wizard Init
//stepsOrientation: "horizontal",
var alreadyClickedFinishButton = false;
$("#wizard").steps({
    headerTag: "h3",
    bodyTag: "section",
    transitionEffect: "none",
    stepsOrientation: "vertical",
    titleTemplate: '<span class="number">#index#</span>',
    labels: {
        finish: 'Pay Now'
    },
    onStepChanging: function (event, currentIndex, newIndex) {

        if (currentIndex == 1) {
            debugger;
            if ($("#bform").valid()) {
                if (ValidateCaptcha($("#Captcha").val())) {
                    return true;
                } else {
                    var captchaError = $("span[data-valmsg-for=Captcha]");
                    $(captchaError).removeClass("field-validation-valid");
                    $(captchaError).addClass("field-validation-error");
                    $(captchaError).text("Invalid Captcha");
                }
            }
            return false;
        }
        return true;
    },
    onFinished: function (event, currentIndex) {
        if (!alreadyClickedFinishButton) {
            alreadyClickedFinishButton = true;
            let checkedPaymentType = $("input[type=radio][name=pt]:checked").val();
            $("#l").val($("input[type=radio][name=lic_type]:checked").val());
            $("#PaymentType").val(checkedPaymentType);
            if (checkedPaymentType === '5') {
                RazorPayPayment(null);
            } else {
                $("input[type=hidden][name=PaymentType]").val($("input[type=radio][name=pt]:checked").val());
                $("#bform").submit();
            }
        }
    }
});

function ValidateCaptcha(captcha) {
    var result = false;
    $.ajax({
        type: 'POST',
        url: '/captcha/validate',
        async: false,
        dataType: 'json',
        data: { "captcha": captcha },
        success: function (res) {
            if (res == null || !res.Success) {
                result = false;
            } else {
                result = true;
            }
        },
        error: function (err) {
            console.log(err);
        }
    });

    return result;
}
