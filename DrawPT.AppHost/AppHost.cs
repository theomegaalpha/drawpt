using Aspire.Hosting;
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : builder.AddConnectionString("signalr");
//var signalr = builder.AddConnectionString("signalr");

var migrationService = builder.AddProject<Projects.DrawPT_MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db);

// Add Azure Blob Storage
var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator()
                     .AddBlobs("blobs");

//var openai = builder.ExecutionContext.IsPublishMode
//    ? builder.AddAzureOpenAI("openai")
//    : builder.AddConnectionString("openai");
var openai = builder.AddConnectionString("openai");

// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("drawptapi")
    .WithReference(db)
    .WithReference(storage)
    .WithReference(openai)
    .WithReference(signalr)
    .WithExternalHttpEndpoints()
    .WaitFor(signalr)
    .WaitFor(db)
    .WaitForCompletion(migrationService);

var supabaseUrl = builder.AddParameter("supabase-url");
var supabaseAnonKey = builder.AddParameter("supabase-anon-key");

builder.AddNpmApp("drawptui", "../DrawPT.Vue")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("VITE_SUPABASE_URL", supabaseUrl)
    .WithEnvironment("VITE_SUPABASE_ANON_KEY", supabaseAnonKey)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
