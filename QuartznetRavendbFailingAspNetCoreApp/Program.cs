using Quartz;
using Quartz.Impl.RavenDB;
using QuartznetRavendbFailingAspNetCoreApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add the required Quartz.NET services
builder.Services.AddQuartz(q =>
{
    // Use a Scoped container to create jobs.
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.UsePersistentStore(storeOptions =>
    {
        storeOptions.UseRavenDb(options =>
        {
            options.Urls = new string[] { "http://live-test.ravendb.net" };
            options.Database = "quartznet-ravendb";
        });

        storeOptions.UseJsonSerializer();
    });

    // Comment out these lines and uncomment app.Lifetime.ApplicationStarted.Register(OnStarted); to make it work
    q.AddJob<SampleJob>(j => j.WithIdentity("SampleJob"));
    q.AddTrigger(t => t
        .WithIdentity("SampleTrigger")
        .ForJob("SampleJob")
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 0).InTimeZone(TimeZoneInfo.Utc))
    );
});

// Add the Quartz.NET hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//app.Lifetime.ApplicationStarted.Register(OnStarted);

void OnStarted()
{
    // Start a job to check exchange rates

    ISchedulerFactory factory = app.Services.GetRequiredService<ISchedulerFactory>();
    IJobDetail job = JobBuilder.Create<SampleJob>()
        .WithIdentity("SampleJob", "SampleJobGroup")
        .Build();

    ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity("SampleTrigger", "SampleTriggerGroup")
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 0).InTimeZone(TimeZoneInfo.Utc))
        .ForJob(job)
        .Build();

    Task.Run(async () => {
        IScheduler scheduler = await factory.GetScheduler();
        await scheduler.ScheduleJob(job, trigger);
    }).Wait();
}

app.Run();
