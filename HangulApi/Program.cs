using FluentValidation;
using FluentValidation.AspNetCore;
using HangulApi.Configuration;
using HangulApi.Data;
using HangulApi.Endpoints;
using HangulApi.Extensions;
using HangulApi.Features.Jamos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Documentation
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[DbContext] Connection string: {connectionString}");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
}

builder.Services.AddDbContext<HangulDbContext>(options =>
    options.UseSqlServer(connectionString));

// Mediatr
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Validations
builder.Services
    .AddValidatorsFromAssemblyContaining<CreateJamo.CreateJamoValidator>(ServiceLifetime.Scoped)
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuration
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JamoTypeJsonConverter());
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

var app = builder.Build();

// Puerto para Azure App Service
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

//using (var scope = app.Services.CreateScope())
//{
//    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//    try
//    {
//        var context = scope.ServiceProvider.GetRequiredService<HangulDbContext>();
//        logger.LogInformation("Starting DB migration...");
//        context.Database.Migrate();
//        logger.LogInformation("Database migration completed.");
//    }
//    catch (Exception ex)
//    {
//        logger.LogError(ex, "Database migration failed.");
//        // Aquí puedes decidir si parar la aplicación:
//        throw;
//    }
//}

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapJamoEndpoints();
app.MapApiEndpoints();
app.MapGeneralEndpoints();

app.Run();
