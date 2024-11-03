using Serpent.Server;
using Serpent.Server.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

#region Configure Services

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddGatewayServices();

//services.AddSignalR();

services
    .AddCorsDefaultPolicy(configuration)
    .AddControllers();

#endregion

//todo implement global ex middleware to catch everything
//todo implement exception filter to catch know ex.

var app = builder.Build();

#region Configure

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

//map hubs there

#endregion



app.Run();