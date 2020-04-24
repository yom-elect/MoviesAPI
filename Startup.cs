using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesAPI.Filters;
using MoviesAPI.Models;
using AutoMapper;
using MoviesAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MoviesAPI
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
            services.AddDbContext<movies_apiContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors();

            services.AddDataProtection();

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<IFileStorageService, AzureStorageService>();
            services.AddTransient<IHostedService, MovieInTheatersService>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<movies_apiContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer =false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey =true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    }
                );

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
            })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", 
                    Title = "MoviesAPI" ,
                    Description = "This is a Web API for Movies Operations",
                    TermsOfService = new Uri ("https://moviesworld.com/info")
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI");

            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
 
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(builder => 
            builder.WithOrigins().WithMethods("GET","POST").AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
