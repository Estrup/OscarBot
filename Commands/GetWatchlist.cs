using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using Discord;

namespace OscarBot.Commands
{
    public class GetWatchlist : BaseDiscordCmd
    {
    }

    public class GetWatchlistHandler : MediatR.AsyncRequestHandler<GetWatchlist>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public GetWatchlistHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(GetWatchlist request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var list = await db.Movie.AsQueryable().Where(x => !x.Watched)
                .OrderBy(x => x.Title)
                .ToListAsync();
            var returnstring = new StringBuilder();
            //returnstring.AppendLine($"I found these titles");
            foreach (var item in list)
            {
                returnstring.AppendLine($"**{ item.Title }** *https://www.imdb.com/title/{ item.Id}*");
            }
            var eb = new EmbedBuilder() { Title = "To-watch list", Description = returnstring.ToString(), Color = Color.DarkBlue };
            await Context.Channel.SendMessageAsync(embed: eb.Build());

        }
    }
}