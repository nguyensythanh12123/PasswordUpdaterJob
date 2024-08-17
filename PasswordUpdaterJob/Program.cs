using PasswordUpdaterJob;
using PasswordUpdaterJob.Dapper;
using PasswordUpdaterJob.Services.Interfaces;
using PasswordUpdaterJob.Services.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddHttpClient();
        services.AddTransient<IDapper, DapperRepository>(sp => new DapperRepository(configuration.GetConnectionString("DefaultConnection")));
        services.AddTransient<IUser, UserRepository>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
