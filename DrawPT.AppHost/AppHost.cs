using Aspire.Hosting.Azure;

using Azure.Provisioning.ServiceBus;
using Azure.Provisioning.Sql;
using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

// Add Azure SQL Server
var sql = builder.AddAzureSqlServer("sql")
                .ConfigureInfrastructure(infra =>
                {
                    var resources = infra.GetProvisionableResources();
                    var dbRes = resources.OfType<SqlDatabase>().Single();
                    dbRes.FreeLimitExhaustionBehavior = FreeLimitExhaustionBehavior.BillOverUsage;
                })
                .RunAsContainer(c =>
                {
                    c.WithLifetime(ContainerLifetime.Persistent);
                    c.WithHostPort(52106);
                });
var db = sql.AddDatabase("database");

var signalr = builder.AddAzureSignalR("signalr", AzureSignalRServiceMode.Serverless)
                     .RunAsEmulator();

var serviceBus = builder.AddAzureServiceBus("service-bus")
    .ConfigureInfrastructure(infra =>
    {
        var serviceBusNamespace = infra.GetProvisionableResources()
                                       .OfType<ServiceBusNamespace>()
                                       .Single();

        serviceBusNamespace.Tags.Add("drawpt", "DrawPT Game Engine");
    })
    .RunAsEmulator();
serviceBus.AddServiceBusQueue("apiGlobal");
serviceBus.AddServiceBusQueue("apiGame");
serviceBus.AddServiceBusQueue("gameEngine");
serviceBus.AddServiceBusQueue("gameEngineRequest");
serviceBus.AddServiceBusQueue("gameEngineResponse");

var migrationService = builder.AddProject<Projects.DrawPT_MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db);

// Add Azure Blob Storage
var storage = builder.AddAzureStorage("storage")
    .ConfigureInfrastructure(infra =>
    {
        var storageAccount = infra.GetProvisionableResources()
                                  .OfType<StorageAccount>()
                                  .Single();

        storageAccount.AccessTier = StorageAccountAccessTier.Hot;
        storageAccount.AllowBlobPublicAccess = true;
    })
    .RunAsEmulator(azurite =>
                {
                    azurite
                        .WithBlobPort(51566)
                        .WithContainerName("images")
                        .WithLifetime(ContainerLifetime.Persistent);
                })
    .AddBlobs("blobs");

// Add AI
var openai = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureOpenAI("openai")
    : builder.AddConnectionString("openai");
var gemini = builder.AddConnectionString("gemini");
var freepikKey = builder.AddParameter("FreepikApiKey", true);

var redis = builder.AddRedis("cache");

var insights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights("appinsights")
    : builder.AddConnectionString("appinsights", "ConnectionStrings:appinsights");

var supabaseUrl = builder.AddParameter("supabase-url");
var supabaseIssuer = builder.AddParameter("supabase-issuer");
var supabaseAnonKey = builder.AddParameter("supabase-anon-key");
var supabaseSecretKey = builder.AddParameter("supabase-secret-key", true);
var supabaseApiKey = builder.AddParameter("supabase-api-key", true);
// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("drawptapi")
    .WithReference(db)
    .WithReference(storage)
    .WithReference(openai)
    .WithReference(gemini)
    .WithReference(serviceBus)
    .WithReference(signalr)
    .WithReference(redis)
    .WithReference(insights)
    .WithEnvironment("AuthenticationValidIssuer", supabaseIssuer)
    .WithEnvironment("AuthenticationSecretKey", supabaseSecretKey)
    .WithEnvironment("SupabaseUrl", supabaseUrl)
    .WithEnvironment("SupabaseApiKey", supabaseApiKey)
    .WithEnvironment("FreepikApiKey", freepikKey)
    .WithExternalHttpEndpoints()
    .WaitFor(signalr)
    .WaitFor(db)
    .WaitFor(serviceBus)
    .WaitForCompletion(migrationService);

var customDomain = builder.AddParameter("customDomain");
var certificateName = builder.AddParameter("certificateName");
var googleClientId = builder.AddParameter("google-client-id");

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


builder.AddProject<Projects.DrawPT_Matchmaking>("drawpt-matchmaking")
    .WithReference(redis)
    .WithReference(insights)
    .WithReference(serviceBus)
    .WaitFor(redis);

builder.AddProject<Projects.DrawPT_GameEngine>("drawpt-gameengine")
    .WithReference(serviceBus)
    .WithReference(storage)
    .WithReference(redis)
    .WithReference(db)
    .WithReference(openai)
    .WithReference(gemini)
    .WithReference(insights)
    .WithEnvironment("FreepikApiKey", freepikKey)
    .WithEnvironment("SupabaseUrl", supabaseUrl)
    .WithEnvironment("SupabaseApiKey", supabaseApiKey)
    .WaitFor(api);


builder.AddProject<Projects.DrawPT_ScheduledService>("drawpt-scheduledservice")
    .WithReference(storage)
    .WithReference(redis)
    .WithReference(db)
    .WithReference(openai)
    .WithEnvironment("FreepikApiKey", freepikKey)
    .WaitFor(api);


builder.Build().Run();
