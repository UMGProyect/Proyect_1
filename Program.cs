using Microsoft.AspNetCore.Identity;
using Proyect_1.Models;
using Proyect_1.Services;
using Sentry.Profiling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyect_1.ContextBD;




var builder = WebApplication.CreateBuilder(args);

// Configuración de Sentry
builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://199f73ab41ddbf34061bc95892ee5f0f@o4507875858579456.ingest.us.sentry.io/4507875883810816";
    o.Debug = true;
    o.TracesSampleRate = 1.0;
    o.ProfilesSampleRate = 1.0;
    o.AddIntegration(new ProfilingIntegration(
        TimeSpan.FromMilliseconds(500)
    ));
});

// Configuración de serviciosj
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Registrar el ReportService
builder.Services.AddSingleton<BlobService>(); // Asegúrate de que esta línea esté presente
builder.Services.AddSingleton<BD_User>();
builder.Services.AddSingleton<ReportService>();//Registrar el servicio correctamente

var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Almacenamiento de datos



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSentryTracing(); // Agregar middleware de Sentry para el trazado
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Registrar}/{id?}");

app.Run();


