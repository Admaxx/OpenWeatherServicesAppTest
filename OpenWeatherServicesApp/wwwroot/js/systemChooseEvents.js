$("#systemChoose").select2({
    ajax: {
        url: '/Weather/getSystems',
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
    placeholder: "Wybierz system",
    minimumInputLength: 0,
    allowClear: true,
});
$("#systemChoose").on("select2:select", function (e) {
    let data = e.params.data;
    let coords = $("#citySelect").select2('data')[0];
    $.ajax({
        url: '/Weather/JsonObjectReturn',
        type: 'POST',
        contentType: 'application/json',

        data: JSON.stringify({
            coord: {
                lat: coords.lat,
                lon: coords.lon
            },
            unitSystem: data.text

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
        },
        error: function (err) {
            console.error(err);
        }
    });
});