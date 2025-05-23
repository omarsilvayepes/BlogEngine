﻿using BlogEngine.Repositories;
using BlogEngine.Services;
using BlogEngine.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BlogEngine
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
            // claims

            services.AddAuthorization(options =>
            {
                options.AddPolicy("WriterOnly", policy => policy.RequireClaim("Role", "Writer"));
                options.AddPolicy("PublicOnly", policy => policy.RequireClaim("Role", "Public"));
                options.AddPolicy("EditorOnly", policy => policy.RequireClaim("Role", "Editor"));
            });


            // requires using Microsoft.Extensions.Options
            services.Configure<DatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            //añadir instancia de los servicio
            services.AddScoped<IPostRepository, PostService>();
            services.AddScoped<IUserRepository, UserService>();


            //add autenticaciones

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.ASCII.GetBytes(
                                Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime=true,
                        ClockSkew=TimeSpan.Zero
                    };
                }
                );

            //add cors
            services.AddCors(options => options.AddPolicy("AlloWebApp", builder => builder.AllowAnyOrigin()
                                                                                           .AllowAnyHeader()
                                                                                           .AllowAnyMethod()));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogEngine", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogEngine v1"));
            }
            app.UseCors("AlloWebApp");//add cors

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();// add autenticacion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
