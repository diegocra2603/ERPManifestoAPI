using Application;
using Persistence;
using Services;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddServices(builder.Configuration)
    .AddWebApi(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ERPManifestoAPI v1");
        options.RoutePrefix = string.Empty; // Swagger UI en la raíz
    });
// }

app.UseCors();

app.UseExceptionHandler("/error");

// Railway maneja HTTPS en su proxy, no necesitamos redirigir internamente
// if (!app.Environment.IsDevelopment())
// {
//     app.UseHttpsRedirection();
// }

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseMiddleware<UserSessionDataMiddleware>();

app.MapControllers();

app.Run();
