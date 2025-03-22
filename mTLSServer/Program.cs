using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;

var builder = WebApplication.CreateBuilder(args);
var rootCert = new X509Certificate2(@"C:\Users\Harsh\source\repos\TGA\mTLS-Auth\Certs\server.pfx", "1234"); // Adjust path and password

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        //listenOptions.UseHttps(@"C:\Users\Harsh\source\repos\TGA\mTLS-Auth\Certs\server.pfx", "1234"); // Adjust path and password
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.RequireCertificate;
            httpsOptions.AllowAnyClientCertificate(); // For development purposes
            httpsOptions.CheckCertificateRevocation = false;
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.ChainTrustValidationMode = X509ChainTrustMode.System;
        //options.CustomTrustStore = new X509Certificate2Collection { rootCert };
        options.RevocationMode = X509RevocationMode.NoCheck;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                if (context.ClientCertificate != null)
                {
                    context.Success();
                }
                else
                {
                    context.Fail("invalid cert");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                //context.Fail("invalid cert");
                context.Success();
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var t = context.Connection.ClientCertificate;
    var clientCertificate = await context.Connection.GetClientCertificateAsync();
    if (clientCertificate == null)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Client Certificate is required.");
        return;
    }
    Console.WriteLine($"Client Certificate: {clientCertificate.Subject}");
    await next();
});

app.MapGet("/", () => "Hello, world! mTLS authentication.");
app.Run();
