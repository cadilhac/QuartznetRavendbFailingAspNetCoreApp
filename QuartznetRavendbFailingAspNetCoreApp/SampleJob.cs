using Quartz;

namespace QuartznetRavendbFailingAspNetCoreApp
{
    // Uncomment this line and this will showcase a bug where the job is not run at all
    //[DisallowConcurrentExecution]
    public class SampleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Running job");
        }
    }
}
