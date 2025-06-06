using Aspire.Hosting.Docker;

var builder = DistributedApplication.CreateBuilder(args);

// Add Docker Compose publisher
builder.AddDockerComposePublisher();

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.PrivateMailboxNumber_ApiService>("apiservice");

builder.AddProject<Projects.PrivateMailboxNumber_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
