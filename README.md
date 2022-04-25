# Directions

This sample demonstrates an issue with [quartznet-RavenDB](https://github.com/ravendb/quartznet-RavenDB).

I did not reference the [Quartz.Impl.RavenDB](https://www.nuget.org/packages/Quartz.Impl.RavenDB/) because it's version 1.0.6 and only for .Net Framework. So I have copied the source and made 2 modifications:

1. I have integrated a [pull request](https://github.com/ravendb/quartznet-RavenDB/pull/17) to avoid an exception when the scheduler is not in the database already.
2. I have fixed an issue (not really necessary for this test but eh) to allow job groups, otherwise there would be an exception too. This is on line 131 of trigger.cs.

The app references test database http://live-test.ravendb.net. It uses an existing sample database named quartznet-ravendb. If it's not there anymore, create it in the RavenDb dashboard.

Run the program. It throws an exception complaining that "the job (DEFAULT.SampleJob) referenced by the trigger does not exist". It shows that it's not possible to configure a job early in builder.Services.AddQuartz. It should be possible to do so. It works with the RAM store.
To fix it, comment out the job setup and uncomment this line:

```cs
// app.Lifetime.ApplicationStarted.Register(OnStarted);
```

This defers the job creation just after the initial setup. Run the app. It now works and the job is functional.

## Another bug

There is also another bug that can be showcased. In SampleJob.cs, uncomment a line to enable the DisallowConcurrentExecution attribute. Run the app. The job will never be fired.
