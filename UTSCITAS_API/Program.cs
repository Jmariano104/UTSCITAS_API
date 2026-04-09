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

// Agregar logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

// Controllers con validación automática
builder.Services.AddControllers();

// Configuración de CORS - MEJORADA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200", "http://localhost:4201")
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware en orden correcto
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// CORS DEBE IR ANTES DE AUTH
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();