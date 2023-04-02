using GlobalExceptionFilter;
using GlobalExceptionFilter.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

//builder.Services.Configure<MvcOptions>(option =>
//{
//    option.Filters.Add<MyExceptionFilter>();
//});

builder.Services.Configure<MvcOptions>(option =>
{
    option.Filters.Add<RateLimitActionFilter>();
    //option.Filters.Add<MyActionFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception is not null)
        {
            ErrorMessage error = new()
            {
                Message = exception.Error.Message,
                Stacktrace = exception.Error.StackTrace
            };
            string errorOjb = JsonSerializer.Serialize(error);

            await context.Response.WriteAsync(errorOjb).ConfigureAwait(false);
        }
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
