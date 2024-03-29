using Kisa_Kuikka.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.StaticFiles;

using Kisa_Kuikka.Auth;
using Kisa_Kuikka.Services;
using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Filters;
using Kisa_Kuikka.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Kisa_Kuikka.Controllers;
using System.Reflection;
//using static NPOI.XSSF.UserModel.Charts.XSSFLineChartData<Tx, Ty>;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
var DBHOST = Environment.GetEnvironmentVariable("DB_HOST");
var DBPORT = Environment.GetEnvironmentVariable("DB_PORT");
var DBNAME = Environment.GetEnvironmentVariable("DB_NAME");
var DBUSER = Environment.GetEnvironmentVariable("DB_USER");
var DBUSERPASSWD = Environment.GetEnvironmentVariable("DB_USER_PASSWORD");

var connectionString = $"Server={DBHOST},{DBPORT};Database={DBNAME};User ID={DBUSER};Password={DBUSERPASSWD};TrustServerCertificate=True;MultipleActiveResultSets=true;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { 
    //aseta salasana ja kirjautumis säännöt
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
})
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddErrorDescriber<CustomIdentityErrorDescriber>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();


var mvcbuilder = builder.Services.AddControllersWithViews();
builder.Services.AddDynamicAuthorization<ApplicationDbContext>(options => options.DefaultAdminUser = Environment.GetEnvironmentVariable("DefaultAdminUser"))
.AddSqlServerStore(options => options.ConnectionString = connectionString)
.AddUi(mvcbuilder);

builder.Services.AddRazorPages();
builder.Services.AddScoped<IilmoitusService, IlmoitusService>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");



app.MapRazorPages();
app.MapControllers();

app.Run();
