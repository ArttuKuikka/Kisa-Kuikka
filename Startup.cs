using DynamicAuthorization.Mvc.Core.Extensions;
using DynamicAuthorization.Mvc.JsonStore.Extensions;
using DynamicAuthorization.Mvc.Ui;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kipa_plus.Data;
using Kipa_plus.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace Kipa_plus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var DBHOST = Environment.GetEnvironmentVariable("DB_HOST");
            var DBPORT = Environment.GetEnvironmentVariable("DB_PORT");
            var DBNAME = Environment.GetEnvironmentVariable("DB_NAME");
            var DBUSER = Environment.GetEnvironmentVariable("DB_USER");
            var DBUSERPASSWD = Environment.GetEnvironmentVariable("DB_USER_PASSWORD");

            var connectionString = $"Server={DBHOST},{DBPORT};Database={DBNAME};User ID={DBUSER};Password={DBUSERPASSWD};TrustServerCertificate=True;MultipleActiveResultSets=true;";
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders()
               .AddDefaultUI();

            var mvcBuilder = services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddDynamicAuthorization<ApplicationDbContext>(options => options.DefaultAdminUser = "A@A.f")
                .AddJsonStore()
                .AddUi(mvcBuilder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".lang", "language");
            app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
