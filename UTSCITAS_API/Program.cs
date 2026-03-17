using Microsoft.EntityFrameworkCore;
using UTSCitas_API.Services;
using UTSCITAS_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuración: ConnectionString en appsettings.json -> "ConnectionStrings:DefaultConnection"
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Servicios de aplicación
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProfesionalService, ProfesionalService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();

// MVC / Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: ajustar orígenes permitidos según entorno
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("https://localhost:7278"); // cambiar según orígenes permitidos
    });
});

// Autenticación / Autorización: configurar aquí (JWT, Cookies, etc.)
// builder.Services.AddAuthentication(...);
// builder.Services.AddAuthorization(...);

var app = builder.Build();

// Manejo de errores y entorno
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

// Si se usa autenticación: llamar antes de UseAuthorization
// app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();