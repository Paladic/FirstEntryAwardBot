using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FirstEntryAwardsBot.Common;
using Serilog;
using IResult = Discord.Interactions.IResult;

namespace FirstEntryAwardsBot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, InteractionService commands, 
            IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync ( )
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;

            // Process the command execution results 
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted (ComponentCommandInfo arg1, IInteractionContext arg2, IResult result)
        {
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }
            
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted (ContextCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }
            
            return Task.CompletedTask;
        }
        
        private async Task SlashCommandExecuted (SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            // await arg2.Interaction.DeferAsync(false);
            string ermsg = "";
            if (!arg3.IsSuccess)
            {
                string error = "";
                switch (arg3.ErrorReason)
                {
                    case "The input text has too few parameters.":
                        error = "Вами не были указаны все параметры, проверьте и попробуйте еще раз.";
                        break;
                    case "The input text has too many parameters.":
                        error = "Вы указали слишком много параметров, проверьте и попробуйте еще раз.";
                        break;
                    case "Unknown command.":
                        error = "Неизвестная команда.";
                        break;
                    case "User not found.":
                        error = "Пользователь не найден.";
                        break;
                    case "User requires guild permission Administrator.":
                        error = "Для использования команды тебе необходимы права администратора.";
                        break;
                    case "Bot requires guild permission ManageRoles.":
                        error = "Для использования этой команды мне необходимы права на работу с ролями.";
                        break;
                    case "Command precondition group Администратор failed.":
                        error = "Для использования команды тебе необходимы права администратора.";
                        break;
                    case "Module precondition group Администратор failed.":
                        error = "Для использования команды тебе необходимы права администратора.";
                        break;
                    case "Command precondition group Модерация failed.":
                        error = "Для использования команды тебе необходимы права \"Отправить пользователя думать над своим поведением\"";
                        break;
                    default:
                        error = $"```\n{arg3.ErrorReason}```\n● Необработанная ошибка. Мои соболезнования`";

                        break;
                }

                Embed embed = await arg2.Channel.CreateErrorAsync(error);
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        embed = await arg2.Channel.CreateErrorAsync(error);
                        //await arg2.Interaction.RespondAsync(embed: embed);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        embed = await arg2.Channel.CreateErrorAsync(error);
                        //await arg2.Interaction.RespondAsync(embed: embed);
                        break;
                    case InteractionCommandError.BadArgs:
                        embed = await arg2.Channel.CreateErrorAsync(error);
                        //await arg2.Interaction.RespondAsync(embed: embed);
                        break;
                    case InteractionCommandError.Exception:
                        embed = await arg2.Channel.CreateErrorAsync(error);
                        //await arg2.Interaction.RespondAsync(embed: embed);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        embed = await arg2.Channel.CreateErrorAsync(error);
                        //await arg2.Interaction.RespondAsync(embed: embed);
                        break;
                }

                try
                {
                    await arg2.Interaction.RespondAsync(embed: embed);
                }
                catch (Exception)
                {
                    try
                    {
                        await arg2.Interaction.ModifyOriginalResponseAsync(x => x.Embed = embed);
                    }
                    catch (Exception)
                    {
                        await arg2.Channel.SendMessageAsync(embed: embed);
                    }
                }
                ermsg = $". Команда выдала ошибку: {arg3.ErrorReason}";
            }
            
            if(arg2.Guild != null)
            {
                Log.Information("{Username}#{Discriminator}({UserId}) использовал \"{CommandName}\" в {Guildname}({GuildId}) " +
                                "> #{ChannelName}({ChannelId}) {Error}", arg2.User.Username, arg2.User.Discriminator, arg2.User.Id,
                    arg1.Name, arg2.Guild.Name, arg2.Guild.Id, arg2.Channel.Name, arg2.Channel.Id, ermsg);
                // return Task.CompletedTask;
            }
            else
            {
                Log.Information("{Username}#{Discriminator}({UserId}) использовал \"{CommandName}\" в личных сообщениях " +
                                "> #{ChannelName}({ChannelId}) {Error}", arg2.User.Username, arg2.User.Discriminator, arg2.User.Id,
                    arg1.Name, arg2.Channel.Name, arg2.Channel.Id, ermsg);
            }
        }

        private async Task HandleInteraction (SocketInteraction command)
        {
            if (command.Type != InteractionType.ApplicationCommand)
            {
                return;
            }
            
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, command);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception)
            {
                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if(command.Type == InteractionType.ApplicationCommand)
                    await command.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}