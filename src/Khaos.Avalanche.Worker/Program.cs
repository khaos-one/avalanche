using System.Text.Json;
using System.Text.Json.Serialization;

using Khaos.Avalanche.Hosting;
using Khaos.Avalanche.Worker;
using Khaos.Avalanche.Worker.Watchers;

using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(
    options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.SerializerOptions.Converters.Add(new WatchersConverter());
    });

var app = builder.Build();

var jobsDispatcher = new JobsDispatcher();

app.MapGet("/jobs", () => jobsDispatcher.GetStatuses());
app.MapGet("/jobs/{id:Guid}", (Guid id) => jobsDispatcher.GetJobWatchers(id));
app.MapPost(
    "/jobs",
    (SerializedJobStartInfo startInfo) =>
    {
        var jobHandle = JobHandle.FromSerializedJobStartInfo(startInfo);
        jobsDispatcher.StartNew(jobHandle);

        return new { Id = jobHandle.Id };
    });

app.Run();