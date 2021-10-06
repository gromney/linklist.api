using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using link_list.api;
using LinkList.api.Configurations;
using LinkList.api.Domain;
using LinkList.api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace LinkList.api
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
            services.AddCors(options =>
            {
                options.AddPolicy("LinkListApp", policy =>
                {
                    // policy.WithOrigins("https://localhost:4200", "http://localhost:4200");
                    // policy.WithHeaders("authorization");
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                });
            });

            var mongoOptions = Configuration.GetSection(nameof(MongoDbOptions));
            services.Configure<MongoDbOptions>(mongoOptions);

            services.AddSingleton<IMongoClient, MongoClient>(op => new MongoClient(Configuration.GetConnectionString("MongoDb")));
            services.AddTransient<ILinkListRepository, LinkListRepository>();

            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(options =>
                    {
                        options.Authority = "https://gromney-test.us.auth0.com/";
                        options.Audience = "https://link-list.api/";
                    });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("from:app", policy => policy.Requirements.Add(new HasIssuerRequirement("https://gromney-test.us.auth0.com/")));
            });
            services.AddSingleton<IAuthorizationHandler,HasIssuerHandler>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LinkList.api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("LinkListApp");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkList.api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
