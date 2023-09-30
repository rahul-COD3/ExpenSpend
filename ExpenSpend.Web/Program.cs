using ExpenSpend.Domain.Context;
using ExpenSpend.Web;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Register AutoMapper for DTO-entity mapping.
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    // Other options as needed
});

var configuration = builder.Configuration;

// Setup database context.
builder.Services.AddDbContextConfig(configuration);

// Setup Identity for authentication.
builder.Services.AddIdentityConfig();

// Setup JWT authentication.
builder.Services.AddJwtAuthentication(configuration);

// Setup email services.
builder.Services.AddEmailService(configuration);

// Register data repositories.
builder.Services.AddRepositories();

// Setup Swagger for API docs.
builder.Services.AddSwaggerConfig();

var app = builder.Build();

// Use Swagger in development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed Database
ExpenSpendDbInitializer.Seed(app);

app.Run();