using Discord;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace FirstEntryAwardsBot.Common
{
    public static class Extensions
    {
        public static async Task<Embed> CreateBaseEmbed(this IMessageChannel channel, IUser user, string title, string description, string url = null)
        {
            var embed = new EmbedBuilder()
                .WithAuthor(await CreateAuthorEmbed(user))
                .WithColor(new Color(51, 181, 255))
                .WithTitle(title)
                .WithDescription(description)
                .WithImageUrl(url)
                .Build();
            
            return embed;
        }
        
        public static Task<Embed> CreateErrorAsync(this IMessageChannel channel,
            string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 100, 100))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                        .WithIconUrl(
                            "https://media.discordapp.net/attachments/890682513503162420/980144116832825435/1-62835-128.png")
                        .WithName("Произошла ошибка:");
                })
                .Build();
            
            return Task.FromResult(embed);
        }
        
        public static Task<EmbedAuthorBuilder> CreateAuthorEmbed(IUser user)
        {
            

            var authorBuilder = new EmbedAuthorBuilder()
                .WithName(user.Username + "#" + user.Discriminator)
                .WithIconUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

            return Task.FromResult(authorBuilder);
            // await Extensions.createAuthorEmbed(Context.User)
        }
        
        public static Task<Paginator> CreatePaginatedEmbed(string title, List<string> pages, IUser user, string url = null)
        {
            var paginator = new LazyPaginatorBuilder()
                .AddUser(user)
                .WithPageFactory(GeneratePageAsync) // Создание страиц
                .WithMaxPageIndex(pages.Count-1) // Максимум страниц
                .AddOption(new Emoji("⏪"), PaginatorAction.SkipToStart) // Use different emojis and option order.
                .AddOption(new Emoji("◀"), PaginatorAction.Backward)
                .AddOption(new Emoji("▶"), PaginatorAction.Forward)
                .AddOption(new Emoji("⏩"), PaginatorAction.SkipToEnd)
                .WithCacheLoadedPages(false) // The lazy paginator caches generated pages by default but it's possible to disable this.
                .WithActionOnCancellation(ActionOnStop.DeleteInput) // Delete the message after pressing the stop emoji.
                .WithFooter(PaginatorFooter.None)
                .Build();

            async Task<PageBuilder> GeneratePageAsync(int index)
            {
                var page = new PageBuilder()
                    .WithAuthor(await CreateAuthorEmbed(user))
                    .WithTitle(title)
                    .WithDescription(pages[index])
                    .WithImageUrl(url ?? string.Empty)
                    .WithFooter($"Страница {index + 1} из {pages.Count}")
                    .WithColor(new Color(33, 100, 222));

                return await Task.FromResult(page);
            }
            return Task.FromResult<Paginator>(paginator);
        }
    }
}