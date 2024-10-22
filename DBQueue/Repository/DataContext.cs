using System.Diagnostics;
using DBQueue.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DBQueue.Repository;

public class DataContext : DbContext
{
    public DbSet<MessageHeader> MessageHeaders { get; set; } 
    public DbSet<MessageBody> MessageBodies { get; set; } 
    public DbSet<MessageJournal> MessageJournals { get; set; } 
 
    public DataContext() 
    : base() 
    { 

    }


    public DataContext(DbContextOptions<DataContext> options)
        :base(options)
    {
        
    }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


        IConfigurationRoot configuration = builder.Build();

        optionsBuilder
         //.UseLazyLoadingProxies()
         //.LogTo(Console.WriteLine)
         .UseSqlServer(configuration.GetConnectionString("DBQueueDatabase"));

        base.OnConfiguring(optionsBuilder);
         
    }
}