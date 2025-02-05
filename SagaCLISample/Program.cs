using Zeebe.Client;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace SagaCLISample
{
    internal class Program
    {
        private static readonly string DemoProcessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "demo-process.bpmn");
        private static readonly string ZeebeUrl = "127.0.0.1:26500";
        private static readonly string ProcessInstanceVariables = "{\"a\":\"123\"}";
        private static readonly string JobType = "order-placed";
        private static readonly string WorkerName = Environment.MachineName;
        private static readonly long WorkCount = 100L;
        public static async Task Main(string[] args)
        {

            Console.WriteLine("Hello, World!");
            // create zeebe client
            var client = ZeebeClient.Builder()
                .UseGatewayAddress(ZeebeUrl)
                .UsePlainText()
                .Build();

            var topology = await client.TopologyRequest()
                .Send();
            Console.WriteLine(topology);

            using (var signal = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                client.NewWorker()
                      .JobType(JobType)
                      .Handler(HandleJob)
                      .MaxJobsActive(5)
                      .Name(WorkerName)
                      .AutoCompletion()
                      .PollInterval(TimeSpan.FromSeconds(1))
                      .Timeout(TimeSpan.FromSeconds(10))
                      .Open();

                // blocks main thread, so that worker can run
                signal.WaitOne();
            }

        }

        private static void HandleJob(IJobClient jobClient, IJob job)
        {
            // business logic
            var jobKey = job.Key;
            Console.WriteLine("Handling job: " + job);
            jobClient.NewCompleteJobCommand(jobKey)
                .Variables("{\"orderStatus\":true}")
                .Send()
                .GetAwaiter()
                .GetResult();

            // enable for randomness in SAGAS
            //if (jobKey % 3 == 0)
            //{
            //    jobClient.NewCompleteJobCommand(jobKey)
            //        .Variables("{\"foo\":2}")
            //        .Send()
            //        .GetAwaiter()
            //        .GetResult();
            //}
            //else if (jobKey % 2 == 0)
            //{
            //    jobClient.NewFailCommand(jobKey)
            //        .Retries(job.Retries - 1)
            //        .ErrorMessage("Example fail")
            //        .Send()
            //        .GetAwaiter()
            //        .GetResult();
            //}
            //else
            //{
            //    // auto completion
            //}
        }
    }
}

