using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleAuthServer.API.Middleware;
using SimpleAuthServer.API.Models.Configuration;
using SimpleAuthServer.API.Services;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAuthServer.API
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleAuthServer.API", Version = "v1" });
            });


            //Load auth server configurations
            var cryptoConfiguration = new CryptoConfiguration();
            Configuration.Bind("CryptoConfiguration", cryptoConfiguration);
            var fileStorageConfiguration = new FileStorageConfiguration();
            Configuration.Bind("FileStorageConfiguration", fileStorageConfiguration);
            var tokenConfiguration = new TokenConfiguration();
            Configuration.Bind("TokenConfiguration", tokenConfiguration);


            //Add auth server services
            services.AddSimpleAuth(tokenConfiguration, fileStorageConfiguration, cryptoConfiguration);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleAuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMiddleware<RequestConsoleLoggingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
