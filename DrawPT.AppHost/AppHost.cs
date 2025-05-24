var builder = DistributedApplication.CreateBuilder(args);

var weatherApi = builder.AddProject<Projects.DrawPT_MinimalApi>("weatherapi")
    .WithExternalHttpEndpoints();

builder.AddNpmApp("vue", "../DrawPT.Vue")
    .WithReference(weatherApi)
    .WaitFor(weatherApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
