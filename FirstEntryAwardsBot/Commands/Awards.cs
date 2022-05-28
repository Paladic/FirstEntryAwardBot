using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Xml.Xsl;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using FirstEntryAwardsBot.Common;
using Infrastructure.Context;
using Infrastructure.DataAccessLayer;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace FirstEntryAwardsBot.Commands;

public class Awards : InteractionModuleBase<SocketInteractionContext>
{
    private readonly KeyGifts _keyGifts;
    public InteractiveService Interactive { get;}
    
    public Awards(InteractiveService interactive, KeyGifts keyGifts)
    {
        Interactive = interactive;
        _keyGifts = keyGifts;
    }

    [SlashCommand("награда-получить", "получить ключ за первое подключение (РАБОТАЕТ ОДИН РАЗ!!!)")]
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [EnabledInDm(false)]
    public async Task TakeAwardAsync()
    {
        if (await _keyGifts.CheckForTakedGiftAsync(Context.Guild.Id, Context.User.Id))
        {
            await DeferAsync(true);
            
            var key = await _keyGifts.GetActivatedKey(Context.Guild.Id, Context.User.Id);
            var embed = await Context.Channel.CreateBaseEmbed(Context.User, "Эй ты!", "Ты пытаешься украсть чужую награду, так делать не стоит. " +
                "Свой ключ тебе уже удалось ухватить, дай и другим возможность. \n" +
                $"Но, если вдруг забыл, то вот твой ключ: `{key?.Gift}`. Он был выдан: <t:{key?.ActivationAt}:F>");
            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        }
        else
        {
            await DeferAsync(false);

            Embed embed;
            var key = await _keyGifts.GetKeyFromServerIdAsync(Context.Guild.Id);
            if (key == null)
            {
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Уупс...",
                    "Кажется, у нас закончились ключи. " +
                    "Сообщи об этом администрации, и я тоже это сделаю.");
                await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
                
                var owner = Context.Guild.Owner;
                await owner.SendMessageAsync($"Бжжж!! Это `{Context.Guild.Name}` У нас закончились ключи.");
                
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Отсутствуют ключи",
                    $"Пользователь {Context.User.Mention}`({Context.User.Id})` пытается получить ключ, а они закончились");
            }
            else
            {
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Ключик!",
                    $"Спасибо что присоединился к нам! Вот твой ключ: `{key.Gift}`");
                try
                {
                    await Context.User.SendMessageAsync(embed: embed);
                }
                catch (Exception)
                {
                    embed = await Context.Channel.CreateBaseEmbed(Context.User, "Эй-эй-эй!!",
                        "У тебя закрыты личные сообщения. Открой их, или не получишь ключ!");
                    await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
                    return;
                }
                
                await _keyGifts.ModifyKeyAsync(key.Id, Context.User.Id);
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Ключ выдан!",
                    "Спасибо что пришел к нам, твой ключик ждет тебя в личных сообщениях!");
                await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
                
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Ключ получен",
                    $"Пользователь {Context.User.Mention}`({Context.User.Id})` получил ключ.");
            }
            
            
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            var channelId = config["LogChannelId"];
            if (channelId != null)
            {
                var channel = Context.Client.GetChannel(Convert.ToUInt64(channelId));
                await ((SocketTextChannel) channel).SendMessageAsync(embed: embed);
            }
        }
    }

    [SlashCommand("оп-награда-поиск", "посмотреть, выдавался ли пользователю ключ")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task CheckUserAwardsAsync([Summary("пользователь", "какого пользователя смотреть будем")] SocketGuildUser user)
    {
        await DeferAsync();
        Embed embed;
        var award = await _keyGifts.GetActivatedKey(Context.Guild.Id, user.Id);
        if (award == null)
        {
            embed = await Context.Channel.CreateBaseEmbed(Context.User, $"Просмотр награды: {user.Username}",
                $"у {user.Mention} еще нет полученной награды");
            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        }
        else
        {
            var page = $"● Ключ: {award.Gift}\n";
            page += $"● Добавил: <@{award.AddedBy}> (<t:{award.AddedAt}:F>)\n";
            page += $"● Получил: <@{award.ActivationBy}> (<t:{award.ActivationAt}:F>)";
            embed = await Context.Channel.CreateBaseEmbed(Context.User, $"Просмотр награды: {user.Username}", page);
            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        }
    }

    [SlashCommand("оп-награда-просмотр", "просмотреть все ключи на сервере", runMode: RunMode.Async)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task AllAwardsCheck()
    {
        await DeferAsync();
        
        var awards = await _keyGifts.GetAllKeysFromServer(Context.Guild.Id);
        if (awards.Count == 0)
        {
            var embed = await Context.Channel.CreateBaseEmbed(Context.User, "Наград не найдено.", "Упс, а ключиков то нет...");
            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
            return;
        }

        var pages = new List<string>();
        foreach (var award in awards)
        {
            var page = $"● Ключ: {award.Gift}\n";
            page += $"● Добавил: <@{award.AddedBy}> (<t:{award.AddedAt}:F>)\n";
            if (award.ActivationBy == 0)
            {
                page += "● Ключ еще не получен";
            }
            else
            {
                page += $"● Получил: <@{award.ActivationBy}> (<t:{award.ActivationAt}:F>)";
            }

            pages.Add(page);
        }

        await Context.Interaction.ModifyOriginalResponseAsync(x => x.Content = "** **");
        var paginator = await Extensions.CreatePaginatedEmbed($"Всего наград: {awards.Count} " +
                                                              $"(Доступных: {awards.Count(x => x.ActivationAt == 0)})", pages, Context.User);
        await Interactive.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));
    }
    
    [SlashCommand("оп-награда-добавить", "добавляет ключи" ,runMode: RunMode.Async)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task AddAwardsAsync()
    {
        await DeferAsync();
        var embed = await Context.Channel.CreateBaseEmbed(Context.User, "Добавление ключей", "а ну ка, кидай сюда все ключи. Только кидай их красиво, " +
            "чтобы каждый ключ - на своей строчке", "https://media.discordapp.net/attachments/890682513503162420/980127842396430386/unknown.png");
        
        await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);

        var msg = await Interactive.NextMessageAsync(x => x.Channel.Id == Context.Channel.Id &&
                                                          x.Author.Id == Context.User.Id,
            timeout: TimeSpan.FromMinutes(10));
        if (msg.IsSuccess)
        {
            if (msg.Value.Content != null)
            {
                var keys = msg.Value.Content.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "ура!", $"мы обработали {keys.Length} ключей. Добавляем их?");

                var buttons = new ComponentBuilder()
                    .WithButton("Да", "yes", ButtonStyle.Success)
                    .WithButton("Нет", "no", ButtonStyle.Danger)
                    .Build();
                var botMsg = await Context.Channel.SendMessageAsync(embed: embed, components: buttons);
                var but = await Interactive.NextMessageComponentAsync(x => x.Message.Id == botMsg.Id &&
                                                                           x.User.Id == Context.User.Id,
                    timeout: TimeSpan.FromMinutes(1));
                if (but.IsSuccess)
                {
                    if (but.Value.Data.CustomId == "yes")
                    {
                        foreach (var key in keys)
                        {
                            await _keyGifts.AddKeyAsync(Context.Guild.Id, key, Context.User.Id);
                        }
                        embed = await Context.Channel.CreateBaseEmbed(Context.User, "Ура!",
                            "Все, мы добавили коды");
                        await but.Value.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components = new ComponentBuilder().Build();
                        });
                    }
                    else if(but.Value.Data.CustomId == "no")
                    {
                        embed = await Context.Channel.CreateBaseEmbed(Context.User, "Ну воот...",
                            "Мы не добавим ни одного кода, Довольны?");
                        await but.Value.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components = new ComponentBuilder().Build();
                        });
                    }
                }
                else
                {
                    embed = await Context.Channel.CreateBaseEmbed(Context.User, "Увы: ошибка",
                        "Ты ничего не ввел.");
                    await botMsg.ModifyAsync(x =>
                    {
                        x.Embed = embed;
                        x.Components = new ComponentBuilder().Build();
                    });
                }
            }
            else
            {
                embed = await Context.Channel.CreateBaseEmbed(Context.User, "Увы: ошибка",
                    "Ты ничего не ввел.");
                await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
            }
        }
        else
        {
            embed = await Context.Channel.CreateBaseEmbed(Context.User, "Увы: ошибка",
                    "Тебе не удалось уложиться в 10 минут, какая жалость...");
            await Context.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
        }
    }

}