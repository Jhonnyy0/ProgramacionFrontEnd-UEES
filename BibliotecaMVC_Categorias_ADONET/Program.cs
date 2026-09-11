using BibliotecaMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== Inversión de Control / Inyección de Dependencias =====
// Se registra la relación entre la abstracción (IAutorService) y su
// implementación concreta, con ciclo de vida Scoped: una instancia por
// cada solicitud HTTP.
builder.Services.AddScoped<IAutorService, AutorService>();

// Actividad 5 (Reto) de la semana anterior: para cambiar de implementación
// basta con comentar la línea anterior y descomentar la siguiente.
// El AutoresController no cambia.
// builder.Services.AddScoped<IAutorService, AutorServiceNacional>();

// Mantenimiento de Categorías con ADO.NET sobre SQL Server.
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
