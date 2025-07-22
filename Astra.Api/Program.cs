using Astra.Data.SqlServer.Extensions;
using Astra.Data.SqlServer;
using Astra.Data.GraphQl.Queries;
using Astra.Data.GraphQl.Extensions;
using MediatR;
using System.Runtime.CompilerServices;
using System.Reflection;
using Astra.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Astra.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                }).AddNewtonsoftJson();
            builder.Services.AddScoped<Manager.CountryManager>();
            builder.Services.AddScoped<Manager.CityManager>();
            builder.Services.AddSqlServerRepositories((config, options) =>
            {
                var connection = config.GetConnectionString("DefaultConnection");
                if (connection is not null)
                    options.SqlConnectionString = connection;
            });

            builder.Services.AddMediatR((m) =>
            {
                m.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });
            builder.Services.AddDbSetQueryableFactories<ApplicationDbContext>();
            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddFiltering()
                .AddSorting()
                .AddProjections();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()   // ou .WithOrigins("https://meusite.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOutputCache();
            var key = builder.Configuration["ApplicationJwtKey"] ?? "165sa1d65f165a41f89a4d8f4afhg8jurkitutyurt4hrgsdrfas";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    };
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<CircuitBreakerMiddleware>();

            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.MapGraphQL();

            app.MapControllers();

            app.UseOutputCache();

            app.Run();
        }
}
    }
