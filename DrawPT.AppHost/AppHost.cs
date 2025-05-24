using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

var signalR = builder.AddAzureSignalR("signalr", AzureSignalRServiceMode.Serverless)
                     .RunAsEmulator();

var migrationService = builder.AddProject<Projects.DrawPT_MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db);

// Add Azure Blob Storage
var blobs = builder.AddConnectionString("blobs");

// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("drawptapi")
    .WithReference(db)
    .WithReference(blobs)
    .WithReference(signalR)
    .WithExternalHttpEndpoints()
    .WaitFor(signalR)
    .WaitForCompletion(migrationService);

builder.AddNpmApp("vue", "../DrawPT.Vue")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
