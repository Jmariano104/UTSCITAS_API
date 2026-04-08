using UTSCITAS_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Register concrete Dapper-based services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProfesionalService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<CalendarificService>();

// Register CalendarificService with IHttpClientFactory
builder.Services.AddHttpClient<CalendarificService>(client =>
{
    var baseUrl = builder.Configuration["Calendarific:BaseUrl"] ?? "https://calendarific.com/api/v2";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

// Aplica la política CORS aquí
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseSwagger();
app.UseSwaggerUI();

app.Run();