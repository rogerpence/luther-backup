using Cocona;
using Microsoft.Extensions.Configuration;
using LutherBackup.Models;
using LutherBackup.Logic;
using static LutherBackup.ConsoleHelpers;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();

var coconaBuilder = CoconaApp.CreateBuilder();

//coconaBuilder.Services.AddTransient()

var app = coconaBuilder.Build();

Console.Clear();

//string connectionString = config["ConnectionString:SqlServer"] ;
WriteSuccess("Luther 2.0 Backup");
WriteSuccess("-----------------");

app.AddCommand("RunBackup", async ([Option('d')] string device) =>
{
    BackupManager bm  = new BackupManager(config);

    await bm.Run(device);
});

app.AddCommand("ShowConfig", ([Option('d')] string device) =>
{
    BackupManager bm  = new BackupManager(config);

    bm.ShowConfig(device);
});

app.Run();