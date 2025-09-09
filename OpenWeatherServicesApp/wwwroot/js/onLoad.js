$(document).ready(function () {
    $("#waitingIcon").hide();
    $("#refreshButton").prop('disabled', true);
    $("#refreshButton").css('background-color', 'grey');

    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(function (position) {
            $("#waitingIcon").show();
            var latitude = position.coords.latitude;
            var longitude = position.coords.longitude;

            let unitMetric = $("#systemChoose").select2('data')[0];
            let setUnitSystem;

            if (unitMetric == null)
                setUnitSystem = "metric";

            else
                setUnitSystem = unitMetric.text;

            $.ajax({
                url: '/Weather/JsonObjectReturn',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({

                    unitSystem: setUnitSystem,
                    coord: {
                        lat: position.coords.latitude.toString(),
                        lon: position.coords.longitude.toString()
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
                    if ($('#mySelect2').select2('data') != null) {
                        $("#refreshButton").prop('disabled', false);
                        $("#refreshButton").css('background-color', 'green');
                    }
                    $("#waitingIcon").hide();
                },
                error: function (err) {
                    console.error(err);
                    $("#waitingIcon").hide();
                },

            });
        });
    }
});
