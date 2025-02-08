using Asp.Versioning;
using DeveloperStore.Application;
using DeveloperStore.Infra.Data;
using DeveloperStore.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpoints();
builder.Services.AddApplication();
builder.Services.AddInfrastructureData(builder.Configuration);
builder.Services.AddProblemDetails();

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
                        .HasApiVersion(new ApiVersion(1))
                        .HasApiVersion(new ApiVersion(2))
                        .ReportApiVersions()
                        .Build();

RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(group);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();