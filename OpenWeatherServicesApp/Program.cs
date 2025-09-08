using OpenWeatherServicesApp.DBModels;
using OpenWeatherServicesApp.Models.JsonOptions;
using OpenWeatherServicesApp.Services;
using OpenWeatherServicesApp.Services.JSON;
using OpenWeatherServicesApp.Services.Translators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<APIKeys>(
            builder.Configuration.GetSection("APIKeys")
            );


builder.Services.AddScoped<WeatherDBContext>();
builder.Services.AddScoped<mainClass>();
builder.Services.AddScoped<IWindDirection, WindDirection>();
builder.Services.AddScoped<IGetFromJSON, GetFromJSON>();
builder.Services.AddScoped<IDaysOfWeek, DaysOfWeek>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "CheckWeatherRoute",
    pattern: "{controller=Weather}/{action=Weather}/{id?}");

app.Run();