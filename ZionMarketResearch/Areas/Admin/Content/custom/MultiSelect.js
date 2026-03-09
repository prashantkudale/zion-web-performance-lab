//This is custom plugin for Zion Market Research
//Author - Virendra Deshmukh
//Email - ms202301182200@gmail.com
//Description - This plugin developed for selecting multiple values from dropdown for parent and child select

//Example
//options = {
//    dataUrl: "url for getting data from server side",
//    sucess: successFunction, //function to execute when dataurl returns result.
//    error: errorFunction, //if data url return any error.
//    async: true, //true/false for getting data from data url asynchronosly.
//    sendValue: true,//true/false
//    sendText: false,//true/false
//    onRemove: removefunction,//call a function on removing the item.
//    onRemoveRefreshData: true, //regresh or get data. This function works only if dataUrl is set.
//    refreshUrl: 'mypage/mydata', //URL for getting data
//    showValue: [] //array of json object [{Text:'text', value:'value'}]
//};

var selectionValue = [];
var selectionText = [];
var numberOfDropdown = 0;
var _originalDataUrl = '';
var _sendText;
var _sendValue;
var _refreshUrl;
var _successFunction;
var _errorFunction;
(function ($) {
    $.fn.ZionMultiSelect = function (options) {
        numberOfDropdown += 1;
        refreshData = options.onRemoveRefreshData;
        $(this).attr('divId', 'multiSelectDiv' + numberOfDropdown);
        $(this).after('<div id="multiSelectDiv' + numberOfDropdown + '"></div>');
        _originalDataUrl = options.dataUrl != null ? options.dataUrl : "";
        _sendText = options.sendText != null ? options.sendText : false;
        _sendValue = options.sendValue != null ? options.sendValue : false;
        _refreshUrl = options.refreshUrl;
        _successFunction = options.success;
        _errorFunction = options.error;
        if (options.showValue != null && options.showValue != 'undefined' && options.showValue.length > 0) {
            for (var i = 0; i < options.showValue.length; i++) {
                selectionText.push(options.showValue[i].Text);
                selectionValue.push(options.showValue[i].Value);
            }
            jsShowSelection('#multiSelectDiv' + numberOfDropdown, options);
        }
        $(this).change(function (e) {
            if ($(this).val() != "") {
                selectionValue.push($(this).val());//push value to array
                if ($(this).val() != "")
                    selectionText.push($('option:selected', $(this)).text());//push text to array

                jsShowSelection('#multiSelectDiv' + numberOfDropdown, options);
                if (options != null) {
                    if (options.dataUrl != null && options.dataUrl != '') {
                        var sendTextOrValue = _sendValue && _sendText ?
                                $(this).val() + "/" + $(this).text() : options.sendValue && (options.sendText == false || options.sendText == null) ?
                                $(this).val() : options.sendText && (options.sendValue == false || options.sendValue == null) ?
                                $(this).text() : "";

                        options.dataUrl = sendTextOrValue != "" ? _originalDataUrl + "/" + sendTextOrValue : options.dataUrl;

                        if (sendTextOrValue != "" && (options.sendText || options.sendValue))
                            jsGetData(options.dataUrl, options.success, options.error);
                    }
                }
            }
        });
    };
}(jQuery));
var refreshData;
//show selection on html div
function jsShowSelection(divId, options) {
    var htm = '<ul>';
    var dvId = '';
    for (var i = 0; i < selectionText.length; i++) {
        htm += "<li>" + selectionText[i] + "<span class='fa fa-times remove' style='cursor: pointer;' title='Remove' ind='" + i + "'>Remove</span></li>";
    }
    htm += '</ul>';

    if (selectionText.length > 0) {
        $(divId).html(htm);
    } else {
        $(divId).html('');
    }
    $(".remove").on("click", function () {
        var ctl = $(this);
        jsDeleteItem(ctl.attr("ind"), divId);
        if (refreshData != null && refreshData) {
            var sendDt = _sendValue ? selectionValue[selectionValue.length - 1] : selectionText[selectionText.length - 1];
            if (sendDt != null && sendDt != 'undefined' && sendDt != "") {
                jsGetData(_originalDataUrl + "/" + sendDt, _successFunction, _errorFunction);
            } else {
                jsGetData(_refreshUrl, _successFunction, _errorFunction);
            }
        }
    });
}

//show selection after deletion
function jsDeleteItem(ind, divId) {
    for (var i = ind; i < selectionText.length; i++) {
        var numberOfElementToRemove = selectionText.length - ind;
        selectionText.splice(i, numberOfElementToRemove);
        selectionValue.splice(i, numberOfElementToRemove);
        jsShowSelection(divId);
        //removeFunction();
    }
}

function jsGetData(dataUrl, successFunction, errorFunction) {
    $.ajax({
        url: dataUrl,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunction != null && successFunction != '' ? successFunction : "",
        error: errorFunction != null && errorFunction != "" ? errorFunction : ""
    });
}