using EducationalInstitution.API.Extensions;
using EducationalInstitution.API.Middleware;
using EducationalInstitution.Infrastructure.Data;
using EducationalInstitution.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar JSON para evitar ciclos de referencia
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// SERVICIOS PERSONALIZADOS
// Database, AutoMapper, Repositories y Services
builder.Services.AddApplicationServices(builder.Configuration);

// JWT Autenticación
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger con soporte JWT
builder.Services.AddSwaggerWithJwt();

// CORS para desarrollo
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        try
        {
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EducationalContext>();
            return context.Database.CanConnect()
                ? HealthCheckResult.Healthy("Conexión a la base de datos correcta")
                : HealthCheckResult.Unhealthy("Error en la conexión con la base de datos");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Error en la comprobación de la base de datos: {ex.Message}");
        }
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// CONFIGURAR PIPELINE HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educational Institution API V1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz del proyecto
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
    });
}

// MIDDLEWARE PIPELINE (orden importante)
app.UseMiddleware<ErrorHandlingMiddleware>(); // Manejo centralizado de errores
app.UseHttpsRedirection();
app.UseCors(); // CORS antes de Autenticación
app.UseAuthentication(); // JWT Autenticación
app.UseAuthorization();  // Authorization después de Autenticación


app.MapHealthChecks("/health");


app.MapControllers();

// INICIALIZACIÓN AUTOMÁTICA DE BASE DE DATOS
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Iniciando Educational Institution API");
        logger.LogInformation("Versión: .NET 8");
        logger.LogInformation("Swagger disponible en: {BaseUrl}", app.Environment.IsDevelopment() ? "https://localhost:7000" : "Swagger deshabilitado en producción");

        // Inicializar base de datos automáticamente
        logger.LogInformation("Inicializando base de datos...");
        await DatabaseInitializer.InitializeAsync(scope.ServiceProvider);

        logger.LogInformation("Base de datos inicializada correctamente");
        logger.LogInformation("Usuario por defecto: admin@educational.com / Admin123!");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error crítico al inicializar la base de datos: {Message}", ex.Message);

    // En desarrollo, mostrar el error y continuar
    if (app.Environment.IsDevelopment())
    {
        logger.LogWarning("Continuando en modo desarrollo a pesar del error de BD");
        logger.LogWarning("Verifica la cadena de conexión en appsettings.json");
        logger.LogWarning("Asegúrate de que SQL Server esté ejecutándose");
    }
    else
    {
        logger.LogCritical("No se puede iniciar la aplicación sin base de datos en producción");
        throw;
    }
}

// INFORMACIÓN DE INICIO
var logger2 = app.Services.GetRequiredService<ILogger<Program>>();
logger2.LogInformation("Educational Institution API iniciada correctamente");
logger2.LogInformation("Endpoints disponibles:");
logger2.LogInformation("   POST /api/auth/login - Iniciar sesión");
logger2.LogInformation("   POST /api/auth/register - Registrar usuario");
logger2.LogInformation("   GET/POST/PUT/DELETE /api/students - CRUD Estudiantes");
logger2.LogInformation("   GET/POST/PUT/DELETE /api/professors - CRUD Profesores");
logger2.LogInformation("   GET/POST/PUT/DELETE /api/courses - CRUD Cursos");
logger2.LogInformation("   GET/POST/PUT/DELETE /api/qualifications - CRUD Calificaciones");
logger2.LogInformation("   GET /health - Health check");

app.Run();