using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppForStudy1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.EnvironmentName = "Production";
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                var options = new ExceptionHandlerOptions();
                options.ExceptionHandlingPath = new PathString("/exceptions/page{0}.html");
                options.AllowStatusCode404Response = true;
                options.ExceptionHandler = TestException;
                app.UseExceptionHandler(options);
            }

            app.UseWelcomePage("/welcome");

            //app.Map("/error", TestRun);

            /*
            app.Map("/test", test =>
            {
                test.Map("/run", TestRun);
            });
            */

            app.UseHealthChecks("/health");

            /*
            app.Use((context, next) =>
            {
                int b = 0;
                int x = 5 / b;
                return next();
            });
            */

            DefaultFilesOptions fileOptions = new DefaultFilesOptions();
            fileOptions.DefaultFileNames.Add("page.html");
            app.UseDefaultFiles(fileOptions);
            app.UseDirectoryBrowser();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello world!");
                });
                endpoints.MapGet("/env", async context =>
                {
                    await context.Response.WriteAsync($"{env.EnvironmentName}");
                });
            });
        }

        /*
        private void TestRun(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                string path = $@"{Directory.GetCurrentDirectory()}\wwwroot\exceptions\page{context.Response.StatusCode}.html";
                await context.Response.WriteAsync(File.ReadAllText(path));
            });
        }
        */

        private Task TestException(HttpContext httpContext)
        {
            string path = $@"{Directory.GetCurrentDirectory()}\wwwroot\exceptions\page{httpContext.Response.StatusCode}.html";
            return httpContext.Response.WriteAsync(File.ReadAllText(path));
        }
    }
}
