using RefreshToken.API.Constants;
using RefreshToken.API.DependencyInjection;
using RefreshToken.API.Filters;
using RefreshToken.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers(options => options.Filters.AddService<NotificationFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDependencyInjection(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDependencyInjection();
}
else
{
    app.UseMiddleware<UnexpectedErrorMiddleware>();
}

app.UseCors(CorsPoliciesNamesConstants.CorsPolicy);
app.MigrateDatabase();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MigrateDatabase();

app.Run();
