using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Context;

public class AwardsBotContextFactory : IDesignTimeDbContextFactory<AwardsBotContext>
{
    
    // dotnet ef migrations add name
    // dotnet ef database update
    public AwardsBotContext CreateDbContext(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("infsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder()
            .UseMySql(config["Default"],
                new MySqlServerVersion(new Version(8, 0, 27)));
            
        return new AwardsBotContext(optionsBuilder.Options);
            
    }
        
    static bool IsDebug ( )
    {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
    }


}