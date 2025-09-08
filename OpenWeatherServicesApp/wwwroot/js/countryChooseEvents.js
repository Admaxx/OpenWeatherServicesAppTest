$("#countryChoose").select2({
    ajax: {
        url: '/Weather/getCountries',
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return { term: params.term || "" };
        },
        processResults: function (data) {
            return {

                results: data.map(function (name, index) {
                    return { id: index, text: name };
                })
            };
        }
    },
    placeholder: "Wybierz kraj",
    minimumInputLength: 0,
    allowClear: true,
});