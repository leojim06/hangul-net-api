using FluentValidation;
using FluentValidation.AspNetCore;
using HangulApi.Configuration;
using HangulApi.Data;
using HangulApi.Endpoints;
using HangulApi.Extensions;
using HangulApi.Features.Jamos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Documentation
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<HangulDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    options.AddPolicy("AllowFronted", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// builder.WebHost.ConfigureKestrel(options =>
// {
//    options.ListenAnyIP(80); // Contenedor usar� este puerto
// });

// Services
//builder.Services
//    .AddSingleton<IFileStorageService, AzureBlobStorageService>();

// Configuration
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JamoTypeJsonConverter());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HangulDbContext>();
    context.Database.Migrate();
}

app.UseCors("AllowFronted");

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

app.Run();
