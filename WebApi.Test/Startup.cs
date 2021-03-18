using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Test.Configuration;

namespace WebApi.Test
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
            services.AddControllers();

            var carConf = new CarConfiguration();
            Configuration.Bind("Car", carConf);
            services.AddSingleton(carConf);

            //both are same, addoptions method is more specific
            // services.Configure<CarConfiguration>(Configuration.GetSection("Car"));
            services.AddOptions<CarConfiguration>().Bind(Configuration.GetSection("Car"));
            //-----------------------------------------------

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.WithExposedHeaders("PageNumber", "PageSize", "PageCount", "TotalRecords"); //this makes mentioned headers' values accessible in client side
                });
                options.AddPolicy("RestrictedRequest", builder =>
                 {
                     builder.WithOrigins("http://localhost:8080");
                 }); //restricted Origin Cors Policy
                options.AddPolicy("RestrictedMethodAndHeader", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.WithMethods("Get");
                    builder.WithHeaders("Content-Type");
                }); // restricted Method and Header Cors Policy
                options.AddPolicy("PreflightRequestCache",builder=>
                {
                    builder.WithOrigins("http://localhost:8080").AllowAnyHeader().SetPreflightMaxAge(TimeSpan.FromMinutes(1));
                }); //added time for preflight request cache, after 1 minute there will be new preflight request to send server before actual request
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
