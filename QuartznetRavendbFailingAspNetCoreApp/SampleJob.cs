using Quartz;

namespace QuartznetRavendbFailingAspNetCoreApp
{
    public class SampleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Running job");
        }
    }
}
