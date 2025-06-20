// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DrawPT.Data.Repositories;
using DrawPT.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<MainDbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddDbContextPool<ReferenceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database"), sqlOptions =>
        sqlOptions.MigrationsAssembly("DrawPT.MigrationService")
    ));
builder.EnrichSqlServerDbContext<ReferenceDbContext>();
builder.Services.AddDbContextPool<DailiesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database"), sqlOptions =>
        sqlOptions.MigrationsAssembly("DrawPT.MigrationService")
    ));
builder.EnrichSqlServerDbContext<DailiesDbContext>();

var app = builder.Build();

app.Run();
