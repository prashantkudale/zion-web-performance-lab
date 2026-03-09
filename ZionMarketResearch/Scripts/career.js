$(document).ready(function () {
    $("form").submit(function () {
        $("#btnSubmit").prop("disabled", true);
        var formData = new FormData($("form")[2]);
        $.ajax({
            url: this.action,
            enctype: "multipart/form-data",
            type: this.method,
            processData: false,
            contentType: false,
            data: formData,
            success: function (result) {
                if (!result.Success) {
                    $("#message").removeClass("alert-success");
                    $("#message").addClass("alert-danger");
                    $("#message").html(result.Message);
                } else {
                    $("#message").removeClass("alert-danger");
                    $("#message").addClass("alert-success");
                    $("#message").html("Your resume has been submitted.");
                }
                $("#btnSubmit").prop("disabled", false);
            }
        });
        return false;
    });
});