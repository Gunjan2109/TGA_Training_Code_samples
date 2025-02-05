using PaymentsSAGAService;
using Zeebe.Client;

var builder = WebApplication.CreateBuilder(args);
// Register Zeebe Client as Singleton
builder.Services.AddSingleton<IZeebeClient>(ZeebeClient.Builder()
    .UseGatewayAddress("127.0.0.1:26500") // Replace with broker address
    .UsePlainText()
    .Build());

builder.Services.AddHostedService<ZeebeWorkerService>();
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", () => "Zeebe Worker API is Running");

app.Run();

