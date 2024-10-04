using Cityinfo.API.DbContexts;
using Cityinfo.API.Services;
using Cityinfo.API.Services.Repositories;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

namespace Cityinfo.API;

public static class Configuration
{
    public static void SetupConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddScoped<ITokenManager, TokenManager>();
        builder.Services.AddScoped<IMailService, LocalMailService>();

        builder.Services.AddDbContext<CityInfoDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration["ConnectionStrings:CityConnectionString"]);
        });

        builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddAuthentication("Bearer").AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Authentication:Audience"],
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]!)
                )
            };
        });

        builder.Services.AddApiVersioning(setup =>
            {
                setup.ReportApiVersions = true;
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            }).AddMvc()
            .AddApiExplorer(setup => { setup.SubstituteApiVersionInUrl = true; });

        var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider()
            .GetRequiredService<IApiVersionDescriptionProvider>();

        builder.Services.AddSwaggerGen(setup =>
        {
            foreach (var description in
                     apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                setup.SwaggerDoc($"{description.GroupName}",
                    new OpenApiInfo()
                    {
                        Title = "City Info API",
                        Version = description.ApiVersion.ToString(),
                        Description = "Through this api you can access cities and their points of interest"
                    }
                );
            }


            var xmlCommentsFile = "Cityinfo.API.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

            setup.IncludeXmlComments(xmlCommentsFullPath);

            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };


            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new()
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
    }

    public static void SetupMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                var descriptions = app.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    setup.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant()
                    );
                }
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}
