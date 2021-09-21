using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OkToWake.Models;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.EntityFrameworkCore;
using OkToWake.Areas.Identity.Data;
using OkToWake.Services;
using OkToWake.Grid;
using System.IO;

namespace OkToWake
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var dataFolder = $"Data Source={System.IO.Directory.GetCurrentDirectory()}/Data/{Configuration.GetConnectionString("OkToWakeContextConnection")}";
            services.AddDbContext<OkToWakeContext>(options =>
                options.UseSqlite(dataFolder));

            // pager
            services.AddScoped<IPageHelper, PageHelper>();

            // filters
            services.AddScoped<IScheduleFilters, GridControls>();

            // query adapter (applies filter to schedule request).
            services.AddScoped<GridQueryAdapter>();

            // service to communicate success on edit between pages
            services.AddScoped<EditSuccess>();

            services.AddDefaultIdentity<OkToWakeUser>()
                .AddEntityFrameworkStores<OkToWakeContext>();

            services.AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true; // optional
            }).AddBootstrapProviders().AddFontAwesomeIcons();

            services.AddHttpContextAccessor();
            services.AddOptions();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<IBackgroundScheduleProcessor, BackgroundScheduleProcessor>();
            services.AddHostedService<Worker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            if (Configuration.GetValue<bool>("UseSSL"))
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
