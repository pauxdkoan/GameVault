using FixFlowApp.Source.Infrastructure.Seeds;
using GameVault.Source.Application;
using GameVault.Source.Infrastructure;
using GameVault.Source.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Application
builder.Services.AddApplicationLayerForWebApi();
// Infrastructure y DbContext
builder.Services.AddPersistenceInfrastucture(builder.Configuration);

//ExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AngularClient");



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



await app.Services.SeedIdentityAsync();
await app.RunAsync();
