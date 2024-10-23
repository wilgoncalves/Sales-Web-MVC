using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SalesWebMVC.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SalesWebMVCContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMvcContext")), builder => 
    builder.MigrationsAssembly("SalesWebMVC")));

builder.Services.AddScoped<SeedingService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapWhen(context =>
{
    // Aqui, estamos verificando em qual ambiente o aplicativo está rodando, se é "desenvolvimento" ou não.
    var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

    // Se o aplicativo estiver em ambiente de "desenvolvimento"...
    if (env.IsDevelopment())
    {
        // Aqui, estamos dizendo para o aplicativo pegar um serviço que semeia dados e chamar um método nele chamado "Seed".
        var seedingService = context.RequestServices.GetRequiredService<SeedingService>();
        seedingService.Seed();
    }

    // Aqui estamos dizendo que, independentemente de estar em ambiente de desenvolvimento ou não,
    // não vamos interromper o fluxo normal do aplicativo, então estamos retornando "false".
    return false;
}, app => { });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
