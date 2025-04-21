using Azure.Messaging.ServiceBus;

string connectionString = "<>";
string queueName = "order-service/$DeadLetterQueue";

ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusReceiver receiver = client.CreateReceiver(queueName);

Console.WriteLine("📥 Reading from DLQ...");

var message = await receiver.ReceiveMessageAsync();

if (message != null)
{
    var body = message.Body.ToString();
    Console.WriteLine($"🚨 DLQ Message Received: {body}");
    await receiver.CompleteMessageAsync(message);
}
else
{
    Console.WriteLine("✅ DLQ is empty.");
}

await receiver.DisposeAsync();
await client.DisposeAsync();
