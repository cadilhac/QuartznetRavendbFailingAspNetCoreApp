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

## On Linux

Using the OnStarted handler to create jobs runs fine on my windows dev machine. But when I publish this app on a Linux server, I get this exception and the job is not created:

> Runtime error occurred in main trigger firing loop.
>      System.NotSupportedException: You can't query sync from an async session
>         at Raven.Client.Documents.Session.AsyncDocumentSession.Raven.Client.Documents.Linq.IDocumentQueryGenerator.Query[T](String indexName, String collectionName, Boolean isMapReduce) in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Session\AsyncDocumentSession.Query.cs:line 83
>         at Raven.Client.Documents.Linq.RavenQueryProviderProcessor`1.GetDocumentQueryFor(Expression expression, Action`1 customization) in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Linq\RavenQueryProviderProcessor.cs:line 3675
>         at Raven.Client.Documents.Linq.RavenQueryProviderProcessor`1.Execute(Expression expression) in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Linq\RavenQueryProviderProcessor.cs:line 3778
>         at Raven.Client.Documents.Linq.RavenQueryProvider`1.Execute(Expression expression) in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Linq\RavenQueryProvider.cs:line 128
>         at Raven.Client.Documents.Linq.RavenQueryProvider`1.System.Linq.IQueryProvider.Execute(Expression expression) in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Linq\RavenQueryProvider.cs:line 198
>         at Raven.Client.Documents.Linq.RavenQueryInspector`1.GetEnumerator() in C:\Builds\RavenDB-Stable-5.3\53009\src\Raven.Client\Documents\Linq\RavenQueryInspector.cs:line 82
>         at Quartz.Impl.RavenDB.RavenJobStore.TriggersFired(IReadOnlyCollection`1 triggers, CancellationToken cancellationToken)
>         at Quartz.Impl.RavenDB.RavenJobStore.TriggersFired(IReadOnlyCollection`1 triggers, CancellationToken cancellationToken) in K:\Develop\VisualHintMVC\VisualHintMVC 4.0 Core\Quartz.Impl.RavenDB\RavenJobStore.IJobStore.cs:line 898
>         at Quartz.Core.QuartzSchedulerThread.Run()

## Another bug

There is also another bug that can be showcased. In SampleJob.cs, uncomment a line to enable the DisallowConcurrentExecution attribute. Run the app. The job will never be fired.
