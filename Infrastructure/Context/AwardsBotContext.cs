using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class AwardsBotContext : DbContext
{
    public AwardsBotContext(DbContextOptions options)
        : base(options)
    {
        
    }
    
    // dotnet ef migrations add name
    // dotnet ef database update
    
    public DbSet<KeysGift> KeyGifts { get; set; }
}