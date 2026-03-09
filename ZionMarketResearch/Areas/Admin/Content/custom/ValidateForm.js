function jsFormValidation(form, beforesubmit, beforeserialize, aftersubmit) {
    //var options = {
    //    beforeSubmit: function () {
    //        if (beforesubmit != null) {
    //            beforesubmit();
    //        }
    //        $(form + " :input").attr("disabled", true);
    //        $(form + " :button").attr("disabled", true);
    //        return jsValidate(form);
    //    },
    //    beforeSerialize: beforeserialize,
    //    success: function (responseText, statusText, xhr, $form) {
            
    //        jsAfterSubmit(responseText, statusText, xhr, $form, aftersubmit);
    //        if (responseText != null && responseText.Error == false) {
    //            if (responseText != null && responseText.RedirectUrl != null && responseText.RedirectUrl != '') {
    //                debugger;
    //                jsLoadView(responseText.RedirectUrl);
    //            }
    //        }
    //        //Enable all inputs
    //        $(form + " :input").attr("disabled", false);
    //        $(form + " :button").attr("disabled", false);
    //    }
    //};
    //$(form).ajaxForm(options);
    $(form + " :input").attr("disabled", true);
    $(form + " :button").attr("disabled", true);
    return jsValidate(form);
}

function jsValidate(form) {
    $(form + " :input").attr("disabled", false);
    $(form + " :button").attr("disabled", false);
    var validInput = $(".valid");
    if (!$(form).valid()) {
        var errorInput = $(".input-validation-error");
        for (var i = 0; i < validInput.length; i++) {
            $(validInput[i]).parent().parent().removeClass("has-error");
        }
        for (var i = 0; i < errorInput.length; i++) {
            $(errorInput[i]).parent().parent().addClass("has-error");
            $(errorInput[i]).attr("placeholder", "Required");
            $(".field-validation-error").css("display", "none");
        }
        $(errorInput[0]).focus();
        return false;
    }
    for (var i = 0; i < validInput.length; i++) {
        $(validInput[i]).parent().parent().removeClass("has-error");
    }
    //$(form + " :input").attr("disabled", true);
    //$(form + " :button").attr("disabled", true);
    return true;
}

function jsAfterSubmit(responseText, statusText, xhr, $form, aftersubmit) {
    if (responseText != null && responseText.Error) {
        var errorMessageDiv = $("#errorMessage");
        $(errorMessageDiv).text(responseText.MessageText);
        $(errorMessageDiv).css("display", "");

        setTimeout(function () {   //calls click event after a certain time
            $(errorMessageDiv).css("display", "none");
        }, responseText.TimeToDisplay > 0 ? responseText.TimeToDisplay : 7000);
       
    }
    else if (responseText != null && !responseText.Error && (responseText.RedirectUrl == null || responseText.RedirectUrl == "")) {
        $("#myModal").modal("show");
        $("#modalMessage").html(responseText.MessageText);
        if (responseText != null && responseText.OnYes != null && responseText.OnYes != "") {
            $("#btnModelYes").off("click");
            $("#btnModelYes").on("click", function () {
                $("#myModal").modal("hide");
                $(".modal-backdrop.fade.in").removeClass("modal-backdrop fade in");
                window[responseText.OnYes]();
            });
        } else if (responseText != null && responseText.OnNo != null && responseText.OnNo != "") {
            $("#btnModelNo").off("click");
            $("#btnModelNo").on("click", function () {
                window[responseText.OnNo]();
            });
        }
    }
    else {
        if (aftersubmit != null) {
            aftersubmit();
        }
    }
}

function jsCallMethod(method, control, type, successfunction, errorfunction) {
    var curl = method + (control != null && control != "" ? "/" + (type == "val" ? $(control).val() : $(control).text()) : "");
    $.ajax({
        url: curl,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data != null && data.RedirectUrl != null && data.RedirectUrl != "") {
                jsLoadView(data.RedirectUrl);
            }
            if (successfunction != null) {
                successfunction(data);
            }
        },
        error: function (error) {
            alert(error.responseText);
            if (errorfunction != null) {
                errorfunction();
            }
        }
    });
}