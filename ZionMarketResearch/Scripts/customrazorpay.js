
var _paymentButton = {};
$(document).ready(function () {
    //_paymentButton = $("#btnPayNow");
    $("body").append('<div class="overlay"></div>');
    //addClientClickToButton();
});

function addClientClickToButton() {
    removeClientClickToButton();
    $("#btnPayNow").on("click", function () {
        var _this = $(this);
        if ($("#bform").valid()) {
            processing(_this);
            $.ajax({
                "type": "POST",
                "url": '/buynow/CreateOrder',
                "data": JSON.stringify($("#bform").serializeFormJSON()),
                "contentType": "application/json; charset=utf-8",
                "success": function (res) {
                    if (res === undefined || res.id === undefined) {
                        changePaymentTypeAndSubmitForm(2);
                        return;
                    }

                    var razorpay = new Razorpay({
                        key: res.ClientKey,
                        currency: 'USD',
                        order_id: res.id,
                        amount: res.Amount,
                        description: res.ReportTitle,
                        name: 'Zion Market Research',
                        prefill: {
                            name: $("#Name").val(),
                            email: $("#Email").val(),
                            contact: $("#ContactNumber").val()
                        },
                        handler: function (res) {
                            $.ajax({
                                "type": 'post',
                                "url": '/buynow/RazorPaymentSuccess',
                                "data": JSON.stringify(res),
                                "contentType": "application/json;charset=utf-8",
                                "success": function (data) {
                                    if (data.Success) {
                                        window.location.href = "/process";
                                    }
                                }, error: function (err) {
                                    stopProcessing(_this);
                                }
                            })

                        },
                        modal: {
                            ondismiss: function () {
                                stopProcessing(_paymentButton);
                            }
                        }
                    });

                    razorpay.open();
                },
                "error": function (err) {
                    stopProcessing(_this);
                }
            })
        }

    });
}

function processing(ctl) {
    $("div.overlay").addClass("show");
    $(ctl).text("Processing....");
}

function stopProcessing(ctl) {
    $("div.overlay").removeClass("show");
    //$(ctl).text("Submit");
}

function jsChangePaymentMethod() {
    let checkedPaymentType = $("input[type=radio][name=pt]:checked").val();
    if (checkedPaymentType === "2")
        $("a[href=#finish]").text("Submit");
    else
        $("a[href=#finish]").text("Pay Now");
}

function removeClientClickToButton() {
    $("#btnPayNow").off("click");
}

function changePaymentTypeAndSubmitForm(paymentType) {
    $("#PaymentType").val(paymentType);
    $("#bform").submit();
}

function RazorPayPayment(btn) {
    var _this = btn != null ? $(btn) : null;
    if ($("#bform").valid()) {
        //processing(_this);
        $.ajax({
            "type": "POST",
            "url": '/buynow/CreateOrder',
            "data": JSON.stringify($("#bform").serializeFormJSON()),
            "contentType": "application/json; charset=utf-8",
            "success": function (res) {
                if (res === undefined || res.id === undefined) {
                    changePaymentTypeAndSubmitForm(2);
                    return;
                }

                var razorpay = new Razorpay({
                    key: res.ClientKey,
                    currency: 'USD',
                    order_id: res.id,
                    amount: res.Amount,
                    description: res.ReportTitle,
                    name: 'Zion Market Research',
                    prefill: {
                        name: $("#Name").val(),
                        email: $("#Email").val(),
                        contact: $("#ContactNumber").val()
                    },
                    handler: function (res) {
                        $.ajax({
                            "type": 'post',
                            "url": '/buynow/RazorPaymentSuccess',
                            "data": JSON.stringify(res),
                            "contentType": "application/json;charset=utf-8",
                            "success": function (data) {
                                debugger;
                                if (data.Success) {
                                    window.location.href = "/process";
                                }
                                alert(data.Message);
                            }, error: function (err) {
                                //stopProcessing(_this);
                            }
                        })

                    },
                    modal: {
                        ondismiss: function () {
                            stopProcessing(_paymentButton);
                        }
                    }
                });

                razorpay.open();
            },
            "error": function (err) {
                stopProcessing(_this);
            }
        })
    }
}