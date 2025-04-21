using Azure.Messaging.ServiceBus;

string connectionString = "<>";
string queueName = "order-service";

ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

processor.ProcessMessageAsync += async args =>
{
    var body = args.Message.Body.ToString();
    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(body);

    if (order.Product == "Rolex Day-Date 40")
    {
        // Simulate processing failure
        Console.WriteLine($"💥 Simulated failure for Order: {order.OrderId}");
        throw new Exception("❌ Simulated delivery label generation failure");
    }

    Console.WriteLine($"📦 Label created for Order: {order.OrderId}");
    await args.CompleteMessageAsync(args.Message);
};

processor.ProcessErrorAsync += args =>
{
    Console.WriteLine($"❌ Processing error: {args.Exception.Message}");
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();
Console.WriteLine("🚚 DeliveryService is running. Press any key to exit...");
Console.ReadKey();

await processor.StopProcessingAsync();
await processor.DisposeAsync();
await client.DisposeAsync();

record Order(string OrderId, string CustomerName, string Address, string Product);
