using System.Reflection;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using RiverBooks.Books;
using RiverBooks.EmailSending;
using RiverBooks.OrderProcessing;
using RiverBooks.Reporting;
using RiverBooks.SharedKernel;
using RiverBooks.Users;
using RiverBooks.Users.UseCases.Cart.AddItem;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host.");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddFastEndpoints()
    .AddJWTBearerAuth(builder.Configuration["Auth:JwtSecret"]!)
    .AddAuthorization()
    .SwaggerDocument();

// Add Module Services
List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddBookModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddEmailSendingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddReportingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddUserModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddOrderProcessingModuleServices(builder.Configuration, logger, mediatRAssemblies);

// Set up MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray());
});
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();
// you can pick any type, but it makes sense if you pick validator type.
builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();

//Add MediatR Domain Event Dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseAuthentication()
        .UseAuthorization();
}

/*
* when configuring auth/authen, if you use `app.UseHttpsRedirection()`, that will cause you problems once you start doing 
* authorization headers, because if it redirects from HTTP to HTTPS, it will not forward the authorization headers, and
* you'll be scratching your head about why the token isn't working.
*/
//app.UseHttpsRedirection();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { } //needed for tests
