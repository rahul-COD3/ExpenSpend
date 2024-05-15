using Serilog;
using ExpenSpend.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ExpenSpend.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configure logger
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Register services
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllerConfig();
builder.Services.AddDbContextConfig(configuration);
builder.Services.AddIdentityConfig();
builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddJwtAuthorizationPolicy();
builder.Services.AddEmailService(configuration);
builder.Services.AddRepositories();
builder.Services.AddSwaggerConfig();
builder.Services.AddCorsPolicy(configuration);

var app = builder.Build();

// Middleware setup
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseMiddleware<SerilogRequestLogger>();

app.UseSwagger();
app.UseSwaggerUI();

app.ApplyMigrations();

app.MapControllers();

app.Run();