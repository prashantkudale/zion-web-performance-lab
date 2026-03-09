$(document).ready(function () {
    //$("#frmNotify").validate();
    $("a[hrefx]").click(function () {
        var lnk = $(this).attr("hrefx");
        if (lnk !== window.location.pathname) {
            window.location = $(this).attr("hrefx");
        }
    });

    $("#frmNotify").on("submit", function (e) {
        e.preventDefault();
        var errorControls = $(".input-validation-error");
        if (errorControls.length > 0)
            return;
        var dt = {};
        dt.ru = $("#ru").val();
        dt.Name = $("#Name.notifyme-input").val();
        dt.Email = $("#Email.notifyme-input").val();
        dt.Phone = $("#Phone.notifyme-input").val();
        dt.NewReport = $("#chkNewRport").prop("checked");
        $.ajax({
            type: "post",
            url: "/notify",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dt),
            success: function (response) {
                if (response.isSuccess) {
                    $("#myModal").modal("hide");
                    $("#btnModal").attr("disabled", "disabled");
                }
            },
            error: function (response) {
                $("#btnModal").removeAttr("disabled");
            }
        });
    });
});