$("#citySelect").select2({
    ajax: {
        url: '/Weather/getOneCiteBySelect',
        dataType: 'json',
        delay: 250,
        data: function (params) {
            let getCountry = $("#countryChoose").select2('data')[0];
            let setCountry;


            if (getCountry == null)
                setCountry = "PL";

            else
                setCountry = getCountry.text;

            return {
                term: params.term,
                country: setCountry,
                page: params.page || 1
            };
        },
        processResults: function (data, params) {
            params.page = params.page || 1;
            return {
                results: data.results,
                pagination: { more: data.results.length >= 10 }
            };
        }
    },
    placeholder: "Wpisz nazwę miasta",
    minimumInputLength: 3,
    allowClear: true,

    templateResult: function (data) {
        if (!data.id) { return data.text; }
        return $(
            `<div>
                        <strong>${data.text} </strong><br/>
                        <small>Lokalizacja: ${data.lat}, ${data.lon}</small>
                    </div>`
        );
    },
    templateSelection: function (data) {
        return data.name || data.text;
    }
});

$("#citySelect").on("select2:select", function (e) {
    let data = e.params.data;
    let unitMetric = $("#systemChoose").select2('data')[0];
    let setUnitSystem;


    if (unitMetric == null)
        setUnitSystem = "metric";

    else
        setUnitSystem = unitMetric.text;

    $.ajax({
        url: '/Weather/JsonObjectReturn',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({

            unitSystem: setUnitSystem,
            coord: {
                lat: data.lat,
                lon: data.lon
            }

        }),
        success: function (city) {
            $("#cityTable").show();
            $("#cityTable tbody").html(`
                    <tr>
                        <td>${city.humidity}</td>
                        <td>${city.windSpeed}</td>
                        <td>${city.windDeg}</td>
                        <td>${city.description}</td>
                        
                    </tr>
                `);
            $("#iconInfo p").html(`
                    <tr>
                    <td><img src="https://openweathermap.org/img/wn/${city.icon}.png" alt="${city.icon}" style="width:104px;height:142px;"></td>
                    <td><h2>${city.name}, ${city.temp}</h2></td>
                    </tr>
                `);

            $("#refreshButton").prop('disabled', false);
        },
        error: function (err) {
            console.error(err);
        }
    });
});