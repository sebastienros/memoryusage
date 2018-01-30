using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MemoryUsage
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/diagnostics", builder => builder.Run(async (context) =>
            {
                using (var process = Process.GetCurrentProcess())
                {
                    var MB = 1024 * 1024;
                    await context.Response.WriteAsync($"WorkingSet64: {process.WorkingSet64 / MB:##,#} \n");
                    await context.Response.WriteAsync($"PeakWorkingSet64: {process.PeakWorkingSet64 / MB:##,#} \n");
                    await context.Response.WriteAsync($"VirtualMemorySize64: {process.VirtualMemorySize64 / MB:##,#} \n");
                    await context.Response.WriteAsync($"PeakVirtualMemorySize64: {process.PeakVirtualMemorySize64 / MB:##,#} \n");
                    await context.Response.WriteAsync($"PagedSystemMemorySize64: {process.PagedSystemMemorySize64 / MB:##,#} \n");
                    await context.Response.WriteAsync($"PeakPagedMemorySize64: {process.PeakPagedMemorySize64 / MB:##,#} \n");
                }
            })
            );

            app.Map("/gc", builder => builder.Run((context) =>
            {
                GC.Collect();
                return Task.CompletedTask;
            })
            );

            app.Map("", builder => builder.Run(async (context) =>
            {
                var result = new String('x', 10 * 1024);
                await context.Response.WriteAsync(result);
            })
            );

        }
    }
}
