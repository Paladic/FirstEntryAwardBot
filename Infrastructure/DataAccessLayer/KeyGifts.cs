using System.Linq;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.DataAccessLayer;

public class KeyGifts
{
    private readonly IDbContextFactory<AwardsBotContext> _contextFactory;

    public KeyGifts(IDbContextFactory<AwardsBotContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<KeysGift>> GetAllKeysFromServer(ulong serverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var gifts = await context.KeyGifts
            .Where(x => x.ServerId == serverId)
            .ToListAsync();

        return await Task.FromResult(gifts);
    }

    public async Task<KeysGift?> GetKeyFromServerIdAsync(ulong serverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var key = await context.KeyGifts
            .Where(x => x.ServerId == serverId && x.ActivationBy == 0)
            .FirstOrDefaultAsync();

        return await Task.FromResult(key);
    }

    public async Task<bool> CheckForTakedGiftAsync(ulong serverId, ulong userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var key = await context.KeyGifts
            .Where(x => x.ServerId == serverId && x.ActivationBy == userId)
            .FirstOrDefaultAsync();
        if (key != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task AddKeyAsync(ulong serverId, string gift, ulong addedBy)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Add(new KeysGift {ServerId = serverId, Gift = gift, 
            AddedBy = addedBy, AddedAt = (ulong) DateTimeOffset.Now.ToUnixTimeSeconds(),
            ActivationAt = 0, ActivationBy = 0});
        await context.SaveChangesAsync();
    }

    public async Task<KeysGift?> GetActivatedKey(ulong serverId, ulong userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var key = await context.KeyGifts
            .Where(x => x.ServerId == serverId && x.ActivationBy == userId)
            .FirstOrDefaultAsync();
        
        return await Task.FromResult(key);
    }

    public async Task ModifyKeyAsync(uint curKeyId, ulong userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var key = await context.KeyGifts
            .Where(x => x.Id == curKeyId)
            .FirstOrDefaultAsync();

        if (key != null)
        {
            key.ActivationBy = userId;
            key.ActivationAt = (ulong) DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        
        await context.SaveChangesAsync();
    }
}