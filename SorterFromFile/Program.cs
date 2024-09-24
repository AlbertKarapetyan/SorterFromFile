using Microsoft.Extensions.Configuration;
using SorterFromFile;

var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true);

IConfiguration configuration = builder.Build();

// Bind the "AppSettings" section to the AppSettings object
var appSettings = new AppSettings();
configuration.GetSection("AppSettings").Bind(appSettings);

if (args.Length == 2)
{
    appSettings.InputFilePath = args[0];
    appSettings.OutputFilePath = args[1];
}
else if (args.Length > 0 && args.Length != 2)
{
    Console.WriteLine("Usage: SorterFromFile $InputFilePath $OutputFilePath");
    return;
}

Console.WriteLine($"Start - {DateTime.Now.ToLongTimeString()}");

var sorter = new Sorter(appSettings);
sorter.Sort();

Console.WriteLine($"End - {DateTime.Now.ToLongTimeString()}");
Console.ReadKey();