using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Infraestructure.Identity.Configurations;
using MedAnalyzer.Infraestructure.Identity.Services;
using MedAnalyzer.Infraestructure.Persistences;
using MedAnalyzer.Infraestructure.Shared;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedAnalyzer API",
        Version = "v1",
        Description = "API REST para el sistema de análisis médico con inteligencia artificial."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT. Ejemplo: Bearer {token}"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            []
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddPersistenceDependencies(builder.Configuration);
builder.Services.AddIdentityLayerIocForWebApi(builder.Configuration);

builder.Services.AddApplicationLayer();
builder.Services.AddSharedLayer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/swagger/v1/swagger.json", async (ISwaggerProvider swaggerProvider) =>
    {
        var doc = swaggerProvider.GetSwagger("v1");
        var json = await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
        return Results.Content(json, "application/json");
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedAnalyzer API v1");
    });
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DomainValidationException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorResponse { Message = ex.Message });
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.RunSeedAsync();

app.Run();
