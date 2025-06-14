using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

// Add Azure SQL Server
//var sql = builder.AddAzureSqlServer("sql")
//        .RunAsContainer();

var sql = builder.AddSqlServer("sql"); // temporary to save monies
var db = sql.AddDatabase("database");

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : builder.AddConnectionString("signalr");

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
    .RunAsEmulator()
    .AddBlobs("blobs");

// Add AI
var openai = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureOpenAI("openai")
    : builder.AddConnectionString("openai");
var gemini = builder.AddConnectionString("gemini");
var freepikKey = builder.AddParameter("FreepikApiKey", true);

var rabbitmq = builder.AddRabbitMQ("messaging");

var redis = builder.AddRedis("cache");

var insights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights("appinsights")
    : builder.AddConnectionString("appinsights", "ConnectionStrings:appinsights");

var supabaseUrl = builder.AddParameter("supabase-url");
var supabaseIssuer= builder.AddParameter("supabase-issuer");
var supabaseAnonKey = builder.AddParameter("supabase-anon-key");
var supabaseSecretKey = builder.AddParameter("supabase-secret-key", true);
var supabaseApiKey = builder.AddParameter("supabase-api-key", true);
// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("drawptapi")
    .WithReference(db)
    .WithReference(storage)
    .WithReference(openai)
    .WithReference(gemini)
    .WithReference(rabbitmq)
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
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithReference(insights)
    .WaitFor(rabbitmq)
    .WaitFor(redis);

builder.AddProject<Projects.DrawPT_GameEngine>("drawpt-gameengine")
    .WithReference(rabbitmq)
    .WithReference(storage)
    .WithReference(redis)
    .WithReference(db)
    .WithReference(openai)
    .WithReference(gemini)
    .WithReference(insights)
    .WithEnvironment("FreepikApiKey", freepikKey)
    .WaitFor(rabbitmq)
    .WaitFor(storage)
    .WaitFor(redis)
    .WaitFor(db)
    .WaitFor(api);


builder.Build().Run();
