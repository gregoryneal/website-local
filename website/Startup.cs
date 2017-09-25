using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO;
using System;

namespace website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add router settings before adding the MVC services as per https://stackoverflow.com/questions/36358751/how-do-you-enforce-lowercase-routing-in-asp-net-core-mvc-6#comment71115410_39113342
            services.AddRouting(options => options.LowercaseUrls = true);

            // Add framework services.
            services.AddMvc();
            // This option ensures that the view engine searches
            services.Configure<RazorViewEngineOptions>(options => {
                options.ViewLocationExpanders.Add(new NonDefaultViewFolderExpander());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //routes
            app.UseMvc(routes =>
            {
                routes.MapRoute("auto", "projects/{*slug}", defaults: new { controller = "Auto", action = "Projects" });
                routes.MapRoute("default", "{action=Projects}/{*slug}", defaults: new { controller = "Auto" });
            });
        }
    }

    public class NonDefaultViewFolderExpander : IViewLocationExpander
    {
        //Automatically searches for views in all top level subfolders in Views/
        //except for Views/Home/ and Views/Shared

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string viewDir = Path.GetDirectoryName($"{Environment.CurrentDirectory}/Views/");
            string[] folders = Directory.GetDirectories(viewDir).Where(path => !path.EndsWith("Home") && !path.EndsWith("Shared")).ToArray(); //Don't search default directories

            string[] viewSearchFolders = new string[folders.Length];

            //Convert from absolute file path to relative file path for the view template engine
            for (int i = 0; i < folders.Length; i++) {
                viewSearchFolders[i] = Path.Combine(Path.GetRelativePath(Environment.CurrentDirectory, folders[i]), "{0}.cshtml");
            }

            return viewSearchFolders.Union(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(NonDefaultViewFolderExpander);
        }
    }
}
