namespace PaymentsSAGAService
{
    using Zeebe.Client;
    using Zeebe.Client.Api.Responses;
    using Zeebe.Client;

    public class ZeebeWorkerService : IHostedService
    {
        private readonly IZeebeClient _zeebeClient;

        public ZeebeWorkerService(IZeebeClient zeebeClient)
        {
            _zeebeClient = zeebeClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Zeebe Worker...");

            _zeebeClient.NewWorker()
                .JobType("make-payment") // Match this with BPMN task job type
                .Handler(async (jobClient, job) =>
                {
                    Console.WriteLine($"Handling job with key: {job.Key}");

                    // Perform task logic here
                    var outputVariables = new { result = "Job completed successfully" };

                    await jobClient.NewCompleteJobCommand(job.Key)
                        .Variables("{\"orderStatus\":true}")
                        .Send();
          

                    Console.WriteLine("Job completed.");
                })
                .MaxJobsActive(5)
                .PollInterval(TimeSpan.FromSeconds(1))
                .Timeout(TimeSpan.FromSeconds(10))
                .Open();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping Zeebe Worker...");
            return Task.CompletedTask;
        }
    }

}
