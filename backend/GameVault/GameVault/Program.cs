using FixFlowApp.Source.Infrastructure.Seeds;
using GameVault.Source.Application;
using GameVault.Source.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddApplicationLayerForWebApi();
//Contexts
builder.Services.AddPersistenceInfrastucture(builder.Configuration);




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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedIdentityAsync();
await app.RunAsync();
