using UTSCitas_API.Services;
using UTSCITAS_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProfesionalService, ProfesionalService>();
builder.Services.AddScoped<ICitaService, CitaService>();
// Register HolidayService with IHttpClientFactory so HttpClient is managed by DI
builder.Services.AddHttpClient<IHolidayService, HolidayService>(client =>
{
    var baseUrl = builder.Configuration["Calendarific:BaseUrl"] ?? "https://calendarific.com/api/v2";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseSwagger();
app.UseSwaggerUI();

app.Run();