using DBQueue;
using DBQueue.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dequeue
{
    internal class Program
    {
        IServiceProvider _serviceProvider;
        ILogger<Program> _logger;
        IQueueProvider _queueProvider;

        static async Task<int> Main(string[] args)
        {
            Program p = new Program();


            if (args.Length > 0 && (args[0] == "-?" || args[0] == "--help" || args[0] == "-h"))
            {
                PrintHelp();
                return 0;
            }

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


            await p.DequeueAsyc(options);

            return 0;

        }

        private async Task DequeueAsyc(Options options)
        {
            IConsumer c = _queueProvider.GetQueueConsumer();

            int i = 0;
            while (i < options.NumberOfMessages || options.NumberOfMessages == -1)
            {
                Message? m = await c.GetMessageFromQueueAsync(options.Tag);

                if (m != null)
                {
                    // use the message body
                    // After the message has been dequeued, the body is no longer available
                    string text = "";
                    if (m.Body != null)
                        text = m.Body.Text.Length > 50 ? m.Body.Text.Substring(0, 50) + "..." : m.Body.Text;
                    
                    await c.DequeueAsync(m.Header.MessageId, options.KeepJournal);

                    _logger.LogInformation("Dequeued message id: { 0}, text: {1}", m.Header.MessageId, text);
                }
                else
                {
                    await Task.Delay(100);
                }


                i++;
            }
        }

        private static Options ParseArgs(string[] args)
        {
            Options options = new Options();
            int i = 0;
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

                    case "-j":
                        options.KeepJournal = args[i + 1].ToLower() == "true";
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
            Console.WriteLine("\tdequeue messages from the queue");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("\tDequeue [options]\n");
            Console.WriteLine();
            Console.WriteLine("Options:\n");
            Console.WriteLine("\t-t\ttag of the message");
            Console.WriteLine("\t-n\tnumber of messages to be dequeued (default infinity)");
            Console.WriteLine("\t-j\tTrue|False keep journal (default false)");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("\tDequeue -t \"foo\" -n 3");
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

        private IServiceCollection ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


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
}
