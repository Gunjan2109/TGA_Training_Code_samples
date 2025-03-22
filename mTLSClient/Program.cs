using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static async Task Main()
    {
        // Load the client certificate (PFX with password)
        var clientCertificate = new X509Certificate2(@"C:\Users\Harsh\source\repos\TGA\mTLS-Auth\Certs\client.pfx", "1234");
        if (!clientCertificate.HasPrivateKey)
            throw new ApplicationException("Cert doesn't contain private key");

        var clientHandler = new HttpClientHandler();
        clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
        clientHandler.ClientCertificates.Add(clientCertificate);
        clientHandler.SslProtocols = SslProtocols.Tls12;

        var client = new HttpClient(clientHandler);
        var response = await client.GetAsync("https://localhost:5001/");
        Console.WriteLine($"Status: {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {content}");
        Console.ReadKey();
    }
}