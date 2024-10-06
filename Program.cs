using Sentry.Profiling;

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

// Configuración de servicios
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSentryTracing(); // Agregar middleware de Sentry para el trazado
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=MenuPrincipal}/{id?}");

app.Run();

