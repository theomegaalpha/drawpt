using Aspire.Hosting;
using Aspire.Hosting.Azure;
using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sql = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSqlServer("sql")
    : builder.AddAzureSqlServer("sql")
        .RunAsContainer();

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

var openai = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureOpenAI("openai")
    : builder.AddConnectionString("openai");
var gemini = builder.AddConnectionString("gemini");

// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("drawptapi")
    .WithReference(db)
    .WithReference(storage)
    .WithReference(openai)
    .WithReference(gemini)
    .WithReference(signalr)
    .WithExternalHttpEndpoints()
    .WaitFor(signalr)
    .WaitFor(db)
    .WaitForCompletion(migrationService);

var supabaseUrl = builder.AddParameter("supabase-url");
var supabaseAnonKey = builder.AddParameter("supabase-anon-key");
var googleClientId = builder.AddParameter("google-client-id");

var customDomain = builder.AddParameter("customDomain");
var certificateName = builder.AddParameter("certificateName");

builder.AddNpmApp("drawptui", "../DrawPT.Vue")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT", port: builder.ExecutionContext.IsPublishMode ? 80 : 5173)
    .WithEnvironment("VITE_SUPABASE_URL", supabaseUrl)
    .WithEnvironment("VITE_SUPABASE_ANON_KEY", supabaseAnonKey)
    .WithEnvironment("VITE_GOOGLE_CLIENT_ID", googleClientId)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile(c =>
        c.WithBuildArg("VITE_SUPABASE_URL", supabaseUrl)
         .WithBuildArg("VITE_SUPABASE_ANON_KEY", supabaseAnonKey)
         .WithBuildArg("VITE_GOOGLE_CLIENT_ID", googleClientId))
    .PublishAsAzureContainerApp((infra, app) =>
    {
        app.ConfigureCustomDomain(customDomain, certificateName);
    });


builder.Build().Run();
