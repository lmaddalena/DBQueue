using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBQueue
{
    public class QueueProvider : IQueueProvider
    {
        private readonly ILogger<QueueProvider> _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueueProvider() 
        {

            // create and configure the service container
            IServiceCollection serviceCollection = ConfigureServices();

            // build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Get Logger
            _logger = _serviceProvider.GetRequiredService<ILogger<QueueProvider>>();

        }

        public QueueProvider(ILogger<QueueProvider> logger)
        {
            _logger = logger;

            // create and configure the service container
            IServiceCollection serviceCollection = ConfigureServices();

            // build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

        }
        public IConsumer GetQueueConsumer()
        { 
            IConsumer consumer = _serviceProvider.GetRequiredService<IConsumer>();
            return consumer;
        }

        public IPublisher GetQueuePublisher()
        {
            IPublisher publisher = _serviceProvider.GetRequiredService<IPublisher>();
            return publisher;
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
                .AddTransient<IConsumer, Consumer>()
                .AddTransient<IPublisher, Publisher>()
                .AddSingleton<IConfiguration>(configuration)
                ;


            return service;

        }

    }
}
