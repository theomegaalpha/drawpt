var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

// Add Azure Blob Storage
var blobs = builder.AddConnectionString("blobs");

// Add DrawPT.Database project to run migrations/seeding
var databaseSeeder = builder.AddProject<Projects.DrawPT_Database>("dbseed")
    .WithReference(db)
    .WaitFor(db);

// Add DrawPT.Api project to Aspire setup
var api = builder.AddProject<Projects.DrawPT_Api>("api")
    .WithReference(db)
    .WithReference(blobs)
    .WithExternalHttpEndpoints()
    .WaitFor(databaseSeeder);

builder.AddNpmApp("vue", "../DrawPT.Vue")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
