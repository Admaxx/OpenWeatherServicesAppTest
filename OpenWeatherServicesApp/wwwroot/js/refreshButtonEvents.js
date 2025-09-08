$("#refreshButton").click(function (e) {
    e.preventDefault();
    let coords = $("#citySelect").select2('data')[0];
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
                lat: coords.lat,
                lon: coords.lon
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
        },
        error: function (err) {
            console.error(err);
        }
    });
});