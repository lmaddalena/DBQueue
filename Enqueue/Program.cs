using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DBQueue;
using System;
using Microsoft.EntityFrameworkCore;
using DBQueue.Model;


namespace Enqueue;
public class Program
{
    IServiceProvider _serviceProvider;
    ILogger<Program> _logger;
    IQueueProvider _queueProvider;

    static async Task<int> Main(string[] args)
    {
        Program p = new Program();

        if (args.Length < 1 || args[0] == "-?" || args[0] == "--help" || args[0] == "-h")
        {
            PrintHelp();
            return 0;
        }    

        string message = args[0];

        Options options;

        try
        {
            options = ParseArgs(args);
        }
        catch (Exception)
        {
            Console.WriteLine("Invalid options");
            PrintHelp();
            return -1;
        }

        await p.EnqueueAsyc(message, options);

        return 0;
    }

    private static Options ParseArgs(string[] args)
    {
        Options options = new Options();
        int i = 1;
        while (i < args.Length)
        {
            switch (args[i])
            {
                case "-n":
                    int n = 0;
                    bool b = int.TryParse(args[i + 1], out n);
                    if (b)
                    {
                        options.NumberOfMessages = n;
                    }
                    else
                        throw new ArgumentException("Invalid option n");
                    break;

                case "-t":
                    options.Tag = args[i + 1];
                    break;

                default:
                    throw new ArgumentException("Invalid option");

            }

            i += 2;
        }

        return options;
    }

    static void PrintHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Description:");
        Console.WriteLine("\tinsert messages into the queue");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("\tEnqueue <message> [options]\n");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("\tmessage: message to be inserted into the queue");
        Console.WriteLine();
        Console.WriteLine("Options:\n");
        Console.WriteLine("\t-t\ttag of the message");
        Console.WriteLine("\t-n\tnumber of messages to be inserted (default 1)");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("\tEnqueue \"My Message\" -t \"foo\" -n 3");
        Console.WriteLine();
    }


    public Program()
    {

        // create and configure the service container
        IServiceCollection serviceCollection = ConfigureServices();
        
        // build the service provider
        _serviceProvider = serviceCollection.BuildServiceProvider();

        // Get Logger
        _logger = _serviceProvider.GetRequiredService<ILogger<Program>>();

        // Get QueueProvider
        _queueProvider = _serviceProvider.GetRequiredService<IQueueProvider>();

    }

    private async Task EnqueueAsyc(string message, Options options)
    {
        
        IPublisher p = _queueProvider.GetQueuePublisher();

        TimeOnly t1 = TimeOnly.FromDateTime(DateTime.Now);

        for (int i = 0; i < options.NumberOfMessages; i++)
        {
            Message m = await p.EnqueueAsync(options.Tag, message);
        }


        _logger.LogInformation("Equeued {0} message/s", options.NumberOfMessages);

        TimeOnly t2 = TimeOnly.FromDateTime(DateTime.Now);

        _logger.LogDebug("Completed in { 0:N} mils", (t2 - t1).TotalMilliseconds);

    }

    private IServiceCollection ConfigureServices()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


        IConfigurationRoot configuration = builder.Build();

        IServiceCollection service = new ServiceCollection();

        service.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddConfiguration(configuration.GetSection("Logging"));
            })
            .AddTransient<IQueueProvider, QueueProvider>();

        return service;
    }
}