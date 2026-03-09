$('.autocomplete').autocomplete({
    serviceUrl: '/autocomplete/' + $("#q").val(),
    onSelect: function (suggestion) {
        window.location = "/report/" + suggestion.data;
    }
});

function initCompany() {
    $("#Company").autocomplete({
        //serviceUrl: '/company/' + $("#Company").val(),
        serviceUrl: "https://autocomplete.clearbit.com/v1/companies/suggest",
        type: "GET",
        minChar: 4,
        deferRequestBy: 500,
        dataType: "json",
        transformResult: function (response, originalQuery) {
            return {
                suggestions: $.map(response, function (dataItem) {
                    return { value: dataItem.name, data: dataItem.name, logo: dataItem.logo };
                })
            };
        },
        formatResult: function (suggetions, currentValue) {
            return "<div>" + suggetions.value + "&nbsp;<img style='height: 32px;' src='" + suggetions.logo + "'/></div>";
        }
    });
}
