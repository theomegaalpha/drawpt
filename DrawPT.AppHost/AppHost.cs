var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlServer = builder.AddSqlServer("sqldb");

// Add Azure Blob Storage
var blobs = builder.AddConnectionString("blobs");

// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("api")
    .WithReference(sqlServer)
    .WithReference(blobs)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("vue", "../DrawPT.Vue")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
